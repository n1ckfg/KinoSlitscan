using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderDepthPass : MonoBehaviour {

    private Recorder rec;
    private Camera cam;

    private void Awake() {
        rec = GameObject.FindGameObjectWithTag("Recorder").GetComponent<Recorder>();
        cam = GetComponent<Camera>();
	}
    
    private void OnRenderImage(RenderTexture source, RenderTexture dest) {
        // depthMat material contains shader that reads the destination RenderTexture
        if (rec.depthMode && rec.depthMat) {
            cam.depthTextureMode = DepthTextureMode.Depth;
            Graphics.Blit(source, dest, rec.depthMat);
        } else {
            cam.depthTextureMode = DepthTextureMode.None;
            Graphics.Blit(source,dest);
        }
    }

}
