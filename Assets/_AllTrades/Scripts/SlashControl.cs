using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashControl : MonoBehaviour
{

    public Transform pommel;
    public Transform hilt;
    public float startLength = 1f;
    public float endLength = 3f;
    public int hangFrames = 20;
    public float detectionThreshold = 0.04f;
    public float endThreshold = 0.02f;
    public int detectionFrames = 3;

    private struct v3
    {
        public float x;
        public float y;
        public float z;
    }

    private int detectionCount;
    private int durationCount;
    private bool isSlashing;
    private Vector3 lastHiltPos;
    private MeshFilter mf;
    private Vector3[] vertices;
    private int[] tris;

    // Start is called before the first frame update
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        GetComponent<MeshCollider>().sharedMesh = mf.mesh;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isSlashing)
        {
            durationCount++;
            if (durationCount > hangFrames)
            {
                // Reset mesh
                mf.mesh.triangles = new int[0];
                mf.mesh.vertices = new Vector3[0];
            }

            // Debug.Log((lastHiltPos - (hilt.position - hilt.parent.parent.parent.position)).magnitude.ToString("F5"));
            if ((lastHiltPos - (hilt.position - hilt.parent.parent.parent.position)).magnitude > detectionThreshold)
            {
                detectionCount++;
                if (detectionCount >= detectionFrames)
                {
                    // Reset mesh
                    mf.mesh.triangles = new int[0];
                    mf.mesh.vertices = new Vector3[0];

                    // Start the slash
                    isSlashing = true;
                    durationCount = 0;
                    detectionCount = 0;
                }
            }
            else
            {
                detectionCount = 0;
            }
        }
        else
        {
            if ((lastHiltPos - (hilt.position - hilt.parent.parent.parent.position)).magnitude < detectionThreshold)
            {
                detectionCount++;
                if (detectionCount >= detectionFrames)
                {
                    // End the slash
                    isSlashing = false;
                    detectionCount = 0;
                }
            }

            // Create vertices
            vertices = new Vector3[mf.mesh.vertices.Length + 2];
            Array.Copy(mf.mesh.vertices, vertices, mf.mesh.vertices.Length);
            vertices[mf.mesh.vertices.Length] = (hilt.position - pommel.position).normalized * startLength + pommel.position;
            vertices[mf.mesh.vertices.Length + 1] = (hilt.position - pommel.position).normalized * endLength + pommel.position;
            mf.mesh.vertices = vertices;

            if (mf.mesh.vertices.Length >= 4)
            {
                // Create triangles
                tris = new int[mf.mesh.triangles.Length + 6];
                Array.Copy(mf.mesh.triangles, tris, mf.mesh.triangles.Length);
                tris[mf.mesh.triangles.Length] = mf.mesh.vertices.Length - 1;
                tris[mf.mesh.triangles.Length + 1] = mf.mesh.vertices.Length - 3;
                tris[mf.mesh.triangles.Length + 2] = mf.mesh.vertices.Length - 4;
                tris[mf.mesh.triangles.Length + 3] = mf.mesh.vertices.Length - 2;
                tris[mf.mesh.triangles.Length + 4] = mf.mesh.vertices.Length - 1;
                tris[mf.mesh.triangles.Length + 5] = mf.mesh.vertices.Length - 4;
                mf.mesh.triangles = tris;
            }
        }

        lastHiltPos = hilt.position - hilt.parent.parent.parent.position;
    }


}
