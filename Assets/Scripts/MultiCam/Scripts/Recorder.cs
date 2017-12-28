﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Recorder : MonoBehaviour {

    public enum CaptureMode { SINGLE_CAM, CAM_ARRAY_STILL, CAM_ARRAY_VIDEO };
    public CaptureMode captureMode = CaptureMode.SINGLE_CAM;
    public enum FxMode { NONE, DEPTH, SLITSCAN };
    public FxMode fxMode = FxMode.NONE;
    public enum ImageFormat { FLOAT, ALPHA };
    public ImageFormat imageFormat = ImageFormat.ALPHA;
	public string fileName = "frame";
	public string filePath = "Frames";
	public int resWidth = 1920;
	public int resHeight = 1080;
	public float fps = 24.0f;

	public int counter = 0;
	public int counterLimit = 100;
	
	public int superSample = 1;
	public int zeroPadding = 4;

    public CameraArray camArray;
	public GameObject mayaCamera;
	public float mayaCameraAngleOfView = 54.43f;

	[System.Serializable]
	public struct AnimatorInfo {
		public Animator animator;
		public float animatorSpeed;
	}

	public Material depthMat;
    public AnimatorInfo[] animatorInfo;

    private float timeScaleOrig; // track timescale to freeze animation between frames
	private string uniqueFilePath = "";
	private bool activate = false;

    private void Awake() {
        if (captureMode == CaptureMode.SINGLE_CAM) {
            if (fxMode == FxMode.DEPTH) {
                Camera.main.gameObject.AddComponent<RecorderDepthPass>();
            } else if (fxMode == FxMode.SLITSCAN) {
                ;
                Camera.main.gameObject.AddComponent<Kino.Slitscan>();
            }
        } else {
            if (fxMode == FxMode.DEPTH) {
                for (int i = 0; i < camArray.cams.Count; i++) {
                    camArray.cams[i].gameObject.AddComponent<RecorderDepthPass>();
                }
            } else if (fxMode == FxMode.SLITSCAN) {
                for (int i = 0; i < camArray.cams.Count; i++) {
                    camArray.cams[i].gameObject.AddComponent<Kino.Slitscan>();
                }
            }
        }
    }

    private void Start() { 
        if (mayaCamera != null) mayaCameraCorrect(Camera.main);
		Application.runInBackground = true;
		Time.fixedDeltaTime = 1.0f/fps;
		Time.captureFramerate = (int) fps;
		uniqueFilePath = filePath;
		int inc = 1;
		while (Directory.Exists(uniqueFilePath)) {
			uniqueFilePath = filePath + inc;
			inc++;
		}
		Directory.CreateDirectory(uniqueFilePath);  

        if (captureMode == CaptureMode.CAM_ARRAY_VIDEO) {
            for (int i=0; i<camArray.cams.Count; i++) {
                String subUrl = Path.Combine(uniqueFilePath, String.Format("Cam" + "{1:D0" + zeroPadding + "}", uniqueFilePath, i));
                Debug.Log(subUrl);
                Directory.CreateDirectory(subUrl);
            }
        }

		timeScaleOrig = Time.timeScale;

		for (int i=0; i < animatorInfo.Length; i++) {
            Debug.Log(animatorInfo[i].animator.speed);
			animatorInfo[i].animator.speed = animatorInfo[i].animatorSpeed; 
		}

		activate = true;
	}
	
	private void LateUpdate() {
		if (activate) {
			StartCoroutine(Capture());
		}
	}
	
	private IEnumerator Capture() {
		if (counter < counterLimit) {
			int inc;
			if (counter == 0) {
				inc = 1;
			} else {
				inc = counter;
			}
			Time.timeScale = 0;
			yield return new WaitForEndOfFrame();

            if (captureMode == CaptureMode.CAM_ARRAY_VIDEO) {
                for (int i = 0; i < camArray.cams.Count; i++) {
                    String path1 = String.Format("{0}/" + "Cam" + "{1:D0" + zeroPadding + "}", uniqueFilePath, i);
                    String path2 = String.Format(fileName + "{1:D0" + zeroPadding + "}.png", uniqueFilePath, inc);
                    String path3 = Path.Combine(path1, path2);
                    captureHandler(camArray.cams[i], path3, resWidth, resHeight, superSample);
                }
            } else if (captureMode == CaptureMode.CAM_ARRAY_STILL) {
                for (int i = 0; i < camArray.cams.Count; i++) {
                    string path = String.Format("{0}/" + fileName + "{1:D0" + zeroPadding + "}.png", uniqueFilePath, i);
                    captureHandler(camArray.cams[i], path, resWidth, resHeight, superSample);
                }
            } else {
                string path = String.Format("{0}/" + fileName + "{1:D0" + zeroPadding + "}.png", uniqueFilePath, inc);
                captureHandler(Camera.main, path, resWidth, resHeight, superSample);
            }

			Time.timeScale = timeScaleOrig;
			Debug.Log("saved " + counter);
			counter++;
		} else {
			activate = false;
			counter = 0;
			Debug.Log("finished");
		}
	}

    private void captureHandler(Camera cam, string path, int resWidth, int resHeight, int superSample) {
        resWidth *= superSample;
        resHeight *= superSample;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        cam.targetTexture = rt;

        Texture2D screenShot;
        if (imageFormat == ImageFormat.FLOAT) {
            screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBAFloat, false);
        } else {
            screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
        }

		cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		cam.targetTexture = null;
		RenderTexture.active = null; 
		Destroy(rt);
		byte[] bytes = screenShot.EncodeToPNG();
		
		System.IO.File.WriteAllBytes(path, bytes);
	}

	private void mayaCameraCorrect(Camera cam) {
		cam.transform.position = mayaCamera.transform.position;
		cam.transform.rotation = mayaCamera.transform.rotation * Quaternion.Euler(0,180,0);
		cam.transform.parent = mayaCamera.transform;
		
		if (mayaCameraAngleOfView > 0.0f) {
			cam.fieldOfView = mayaCameraAngleOfView * 0.596f;    
		}
	}

}
