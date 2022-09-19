using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    public event Action OnMeshGenerated = delegate { };

    [Header("When the cubes go marching in")]
    [SerializeField]
    private float _spacing = 0.1f;

    [SerializeField]
    private Vector3 _size;

    [SerializeField]
    private float _surfaceLevel = 1;

    [SerializeField]
    private Texture2D _texture;

    [SerializeField]
    private Material[] mats;

    [Header("Debug")]
    [SerializeField]
    private bool _gizmos;

    [SerializeField]
    private bool _runInUpdate;

    private GameObject _gm;
    private MeshFilter _meshFilter;

    private List<Cell> _cells = new List<Cell>();
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _tris = new List<int>();

    private Vertex[,,] _vertexes;

    private float _timer = 0;

    public Vector2 Bounds
    {
        get
        {
            return new Vector2(_size.x, _size.z);
        }
    }

    private void Awake()
    {
        _gm = new GameObject();
        _meshFilter = _gm.AddComponent<MeshFilter>();
        MeshRenderer rend = _gm.AddComponent<MeshRenderer>();
        rend.materials = mats;
    }

    private void Start()
    {
        GeneratePoints();

        GenerateCells();

        March();

        RemoveDuplicates();

        GenerateMesh();

        OnMeshGenerated();
    }

    private void RemoveDuplicates()
    {
        Dictionary<Vector3, int> vertexIndexDictionary = new Dictionary<Vector3, int>();
        List<Vector3> processedVertices = new List<Vector3>();
        List<int> processedTriangles = new List<int>();
        int vertexIndex = 0;

        for (int i = 0; i < _vertices.Count; i++)
        {
            if (vertexIndexDictionary.TryGetValue(_vertices[i], out int sharedVertexIndex))
            {
                processedTriangles.Add(sharedVertexIndex);
            }
            else
            {
                vertexIndexDictionary.Add(_vertices[i], vertexIndex);
                processedVertices.Add(_vertices[i]);
                processedTriangles.Add(vertexIndex);
                vertexIndex++;
            }
        }

        for (int i = 0; i < processedTriangles.Count; i+= 3)
        {
            _tris.Add(processedTriangles[i]);
            _tris.Add(processedTriangles[i + 2]);
            _tris.Add(processedTriangles[i + 1]);
        }

        _vertices = processedVertices;
    }

    private void Update()
    {
        if (!_runInUpdate)
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > 1)
        {
            _timer = 0;
            GeneratePoints();

            GenerateCells();

            March();

            RemoveDuplicates();

            GenerateMesh();

        }
    }

    private void GenerateMesh()
    { 
        Mesh mesh = new Mesh();
        _meshFilter.mesh = mesh;

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _tris.ToArray();

        mesh.RecalculateNormals();
        mesh.subMeshCount = 2;

        NativeArray<Vector3> vertics = new NativeArray<Vector3>(_vertices.ToArray(), Allocator.TempJob);
        NativeArray<int> triangls = new NativeArray<int>(_tris.ToArray(), Allocator.TempJob);

        int count = Mathf.FloorToInt((float)_tris.Count / 3.0f);
        bool[] boolArray = new bool[count];
        NativeArray<bool> grassBools = new NativeArray<bool>(boolArray, Allocator.TempJob);

        MeshJob job = new MeshJob
        {
            Vertices = vertics,
            Triangles = triangls,
            bools = grassBools
        };

        int sixth = count / 2;
        JobHandle handle = job.Schedule(count, sixth);
        handle.Complete();

        List<int> grassTris = new List<int>();
        List<int> notTris = new List<int>();

        for (int i = 0; i < grassBools.Length; i++)
        {
            int triangleIndex = i * 3;

            if (grassBools[i])
            {
                grassTris.Add(_tris[triangleIndex]);
                grassTris.Add(_tris[triangleIndex + 1]);
                grassTris.Add(_tris[triangleIndex + 2]);
            }
            else
            {
                notTris.Add(_tris[triangleIndex]);
                notTris.Add(_tris[triangleIndex + 1]);
                notTris.Add(_tris[triangleIndex + 2]);
            }
        }

        grassBools.Dispose();

        mesh.SetTriangles(grassTris.ToArray(), 0);
        mesh.SetTriangles(notTris.ToArray(), 1);

        if (_gm.TryGetComponent<MeshCollider>(out MeshCollider col))
        {
            Destroy(col);
        }

        MeshCollider collider = _gm.AddComponent<MeshCollider>();   
        collider.sharedMesh = mesh;
    }

    private void March()
    {
        _tris.Clear();
        _vertices.Clear();

        for (int i = 0; i < _cells.Count; i++)
        {
            // Get Index
            int cubeIndex = 0;
            for (int g = 0; g < 8; g++)
            {
                if (_cells[i].Vertexes[g].Value < _surfaceLevel)
                {
                    cubeIndex += 1 << g;
                }
            }

            // Get something 
            List<int> triangulation = new List<int>();
            for (int g = 0; g < Table.TriangleConnectionTable.GetLength(1); g++)
            {
                int value = Table.TriangleConnectionTable[cubeIndex, g];
                triangulation.Add(value);
            }

            int n = 0;
            for (int g = 0; g < triangulation.Count; g++)
            {
                if (triangulation[g] < 0)
                {
                    continue;
                }
                n++;

                int indexA = Table.CornerIndexFromEdge[triangulation[g], 0];
                int indexB = Table.CornerIndexFromEdge[triangulation[g], 1];

                //float percent = (Mathf.Abs(_cells[i].Vertexes[indexA].Value) + Mathf.Abs(_cells[i].Vertexes[indexB].Value)) / Mathf.Abs(_cells[i].Vertexes[indexA].Value);
                Vector3 vertexPos = Vector3.Lerp(_cells[i].Vertexes[indexA].Position, _cells[i].Vertexes[indexB].Position, 0.5f);

                _vertices.Add(vertexPos);
            }
        } 
    }

    private void GenerateCells()
    {
        _cells.Clear();

        for (int z = 0; z < _vertexes.GetLength(2) - 1; z += 1)
        {
            for (int y = 0; y < _vertexes.GetLength(1) - 1; y += 1)
            {
                for (int x = 0; x < _vertexes.GetLength(0) - 1; x += 1)
                {
                    Vertex[] verts = new Vertex[] {
                        _vertexes[x, y, z],
                        _vertexes[x + 1, y, z],
                        _vertexes[x + 1, y + 1, z],
                        _vertexes[x, y + 1, z],
                        _vertexes[x, y, z + 1],
                        _vertexes[x + 1, y, z + 1],
                        _vertexes[x + 1, y + 1, z + 1],
                        _vertexes[x, y + 1, z + 1] 
                    };
                    Cell cell = new Cell(verts);
                    _cells.Add(cell);
                }
            }
        }
    }

    private void GeneratePoints()
    {
        int width = Mathf.RoundToInt(_size.x / _spacing);
        int height = Mathf.RoundToInt(_size.y / _spacing);
        int depth = Mathf.RoundToInt(_size.z / _spacing);
        _vertexes = new Vertex[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 pos = new Vector3((float)x * _spacing, (float)y * _spacing, (float)z * _spacing);

                    float value = GenerateValue(width, depth, x, z, pos);
                    _vertexes[x, y, z] = new Vertex(transform.position + pos, value);
                }
            }
        }
    }

    private float GenerateValue(int width, int depth, int x, int z, Vector3 pos)
    {
        float value = (pos.y * 6) + (float)NoiseS3D.Noise(pos.x, pos.y, pos.z);

        int xPixel = Mathf.RoundToInt((float)x * (_texture.width / (float)width));
        int yPixel = Mathf.RoundToInt((float)z * (_texture.height / (float)depth));
        value += (1.0f - _texture.GetPixel(xPixel, yPixel).a * 0.5f) * 4;

        return value;
    }

    public void IncreaseAtPosition(Vector3 position, float radius, float strength, float yStretch)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            for (int g = 0; g < _cells[i].Vertexes.Length; g++)
            {
                Vector3 vPos = _cells[i].Vertexes[g].Position;
                float distance = Distance(position, vPos, yStretch);
                //print(dist == _distance);
                if (distance <= radius)
                {
                    float percent = (radius - distance) / radius;
                    _cells[i].Vertexes[g].Value += strength * percent;
                }
            }
        }

        March();
        RemoveDuplicates();
        GenerateMesh();
    }

    public float Distance(Vector3 from, Vector3 to, float yStrength)
    {
        float distance = Mathf.Sqrt(MathF.Pow(from.x - to.x, 2) + MathF.Pow(from.y - to.y, 2) * yStrength + MathF.Pow(from.z - to.z, 2));
        return distance;
    }

    private void OnDrawGizmos() 
    {
        if (!_gizmos)
        {
            return;
        }

        if (_vertexes == null)
        { 
            return;
        }

        for (int x = 0; x < _vertexes.GetLength(0); x++)
        {
            for (int y = 0; y < _vertexes.GetLength(1); y++)
            {
                for (int z = 0; z < _vertexes.GetLength(2); z++)
                {
                    Color color = new Color(_vertexes[x, y, z].Value, _vertexes[x, y, z].Value, _vertexes[x, y, z].Value, 1);

                    Gizmos.color = color;
                    Gizmos.DrawSphere(_vertexes[x, y, z].Position, 0.1f);
                }
            }
        }
    }
}

[BurstCompile]
public struct MeshJob : IJobParallelFor
{
    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<Vector3> Vertices;

    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<int> Triangles;

    public NativeArray<bool> bools;

    public void Execute(int index)
    {
        int triangleIndex = index * 3;
        Vector3 side1 = Vertices[Triangles[triangleIndex + 1]] - Vertices[Triangles[triangleIndex]];
        Vector3 side2 = Vertices[Triangles[triangleIndex + 2]] - Vertices[Triangles[triangleIndex]];
        Vector3 normal = Vector3.Cross(side1, side2).normalized;
        float dot = Vector3.Dot(normal, Vector3.up);

        bools[index] = dot > 0.8f;
    }
}

public struct Cell
{
    public Vertex[] Vertexes; // Should be 8

    public Cell(Vertex[] vertexes)
    {
        Vertexes = vertexes;
    }
}
