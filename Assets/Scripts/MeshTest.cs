using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour {

    public float amp = 1f;
    public float speed = 0.1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] vertTargets;

	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        vertTargets = new Vector3[vertices.Length];
        for (int i=0; i<vertTargets.Length; i++) {
            vertTargets[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
        }
	}
	
	void Update () {
        //vertices = mesh.vertices;
        for (int i=0; i<vertices.Length; i++) {
            if (Vector3.Distance(vertices[i], vertTargets[i]) < 0.01f) {
                vertTargets[i] = new Vector3(vertTargets[i].x, Random.Range(-amp, amp), vertTargets[i].z);
            } else {
                vertices[i] = Vector3.Lerp(vertices[i], vertTargets[i], speed);
            }
        }
        mesh.vertices = vertices;
        //mesh.RecalculateBounds();
	}
}
