using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArray : MonoBehaviour {

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    private void Start() {
        for (int i = 0; i < vertices.Length; i++) {
            Camera cam = new GameObject().AddComponent<Camera>();
            cam.name = "NewCamera" + i;
            cam.transform.parent = transform;
            cam.transform.position = vertices[i];
            cam.transform.LookAt(transform);
        }
    }
	
	void Update () {
		
	}

}
