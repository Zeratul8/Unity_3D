using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [SerializeField]
    Texture texture;
    Vector3[] vertices = new Vector3[]
    {
        new Vector3(-1f, 1f, 0f), new Vector3(1f, 1f, 0f),
        new Vector3(1f, -1f, 0f), new Vector3(-1f, -1f, 0f)
    };
    int[] triangles = new int[]
    {
        0, 1, 2,
        0, 2, 3
    };
    Vector2[] uvs = new Vector2[] 
    { 
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        new Vector2(1f, 0f), new Vector2(0f, 0f)
    };
    Mesh mesh;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "my mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        var mfilter = gameObject.AddComponent<MeshFilter>();
        var mRenderer = gameObject.AddComponent<MeshRenderer>();
        mfilter.mesh = mesh;
        mat = new Material(Shader.Find("Unlit/Texture"));
        mat.SetTexture("_MainTex", texture);
        mRenderer.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
