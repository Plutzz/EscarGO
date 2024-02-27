using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredClouds : MonoBehaviour
{
    public int horizontalStackSize = 20;
    public float cloudHeight;
    public Mesh quadMesh;
    public Material cloudMaterial;
    float offset;

    public int layer;
    public Camera camera;
    private Matrix4x4 matrix;

    void Start()
    {
        cloudMaterial.SetFloat("_MidYValue", transform.position.y);
        cloudMaterial.SetFloat("_CloudWidth", cloudHeight);

        offset = cloudHeight / horizontalStackSize / 2f;
        Vector3 startPosition = transform.position + (Vector3.up * (offset * horizontalStackSize / 2f));
        for (int i = 0; i < horizontalStackSize; i++)
        {
            matrix = Matrix4x4.TRS(startPosition - (Vector3.up * offset * i), transform.rotation, transform.localScale);
            Graphics.DrawMesh(quadMesh, matrix, cloudMaterial, layer, camera, 0, null, true, false, false);
        }
    }
}
