using UnityEngine;

public struct Vertex
{
    public Vector3 Position;
    public float Value;

    public Vertex(Vector3 position, float value)
    {
        Position = position;
        Value = value;
    }
}