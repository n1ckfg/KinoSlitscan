using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArray : MonoBehaviour {

    private Mesh mesh;
    private Vector3[] vertices;
    private List<Camera> cams;
    private float distLimit = 0.1f;

    private void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        cams = new List<Camera>();
    }

    private void Start() {
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 p = transform.TransformPoint(vertices[i]);
            bool makeNewCam = true;
            for (int j=0; j < cams.Count; j++) {
                if (cams[j].transform.position == p) {
                    makeNewCam = false;
                    break;
                }
            }
            if (makeNewCam) {
                Camera cam = new GameObject().AddComponent<Camera>();
                cam.name = "NewCamera" + i;
                cam.transform.parent = transform;
                cam.transform.position = p;
                cam.transform.LookAt(transform);
                cams.Add(cam);
            }
        }
    }
	
	void Update () {
		
	}

}
