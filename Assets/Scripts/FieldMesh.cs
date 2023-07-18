using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldMesh : MonoBehaviour
{

<<<<<<< Updated upstream
=======
    static Color color1 = new Color(1f, 0f, 0f);
    static Color color2 = new Color(0f, 1f, 0f);
    static Color color3 = new Color(0f, 0f, 1f);
>>>>>>> Stashed changes
    private Mesh _fieldMesh;
    private MeshCollider _meshCollider;
    private MeshFilter _filter;

    [NonSerialized] private List<Vector3> _vertices;
    [NonSerialized] private List<int> _triangles;
    [NonSerialized] private List<Color> _colors;
    [NonSerialized] private List<Vector2> _uvs;

    public MeshFilter Filter
    {
        get {
            if (_filter == null) {
                _filter = GetComponent<MeshFilter>();
            }
            return _filter;
        }
    }

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _fieldMesh = new Mesh();
        _fieldMesh.name = "Field Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _uvs = new List<Vector2>();
    }

    public void Clear()
    {
        _fieldMesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();
    }

    public void Apply()
    {
        _fieldMesh.SetVertices(_vertices);
        _fieldMesh.SetTriangles(_triangles, 0);
        _fieldMesh.SetUVs(0, _uvs);
        _fieldMesh.RecalculateNormals();
    }

    public Mesh ApplyNormalNoise(Mesh mesh)
    {

        var vertices = mesh.vertices;
        var normals = mesh.normals;
        for (int i = 0, n = mesh.vertexCount; i < n; i++)
        {
            vertices[i] = vertices[i] + normals[i] * UnityEngine.Random.value * 0.55f;
        }
        mesh.vertices = vertices;

        return mesh;
    }

    public void Smooth()
    {
        Filter.mesh = MeshSmoothing.HCFilter(Filter.mesh, 5, 0.13f, 0.2f);
        //Debug.Log("aaa");
        //_fieldMesh = MeshSmoothing.LaplacianFilter(_fieldMesh, 10);
        //_fieldMesh.RecalculateNormals();
    }

    public void AddVertice(Vector3 point)
    {
        _vertices.Add(point);
    }

    public void AddTriangle(int point1, int point2, int point3)
    {
        _triangles.Add(point1);
        _triangles.Add(point2);
        _triangles.Add(point3);
    }

    public void AddUVs(List<Vector2> uvs)
    {
        _uvs = uvs;
    }


    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }
}
