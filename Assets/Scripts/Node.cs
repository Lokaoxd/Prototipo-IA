using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Node
{
    public string Label { get; }
    public Transform Obj {  get; }
    public Node(string label)
    {
        Label = label;
        Obj = GameObject.Find($"Node {label}").transform;
    }

    public Node(string label, Transform obj)
    {
        Label = label;
        Obj = obj;
    }

    readonly List<Edge> _edges = new();
    public IEnumerable<Edge> Edges => _edges;

    public IEnumerable<NeighborhoodInfo> Neighbors =>
        from edge in Edges
        select new NeighborhoodInfo(
            edge.Node1 == this ? edge.Node2 : edge.Node1,
            edge.Value
            );


    private void Assign(Edge edge)
    {
        _edges.Add(edge);
    }

    public void ConnectTo(Node other)
    {
        float connectionValue = CalcularDistanciaXZ(Obj, other.Obj);
        Edge.Create(connectionValue, this, other);
    }

    public struct NeighborhoodInfo
    {
        public Node Node { get; }
        public float WeightToNode { get; }

        public NeighborhoodInfo(Node node, float weightToNode)
        {
            Node = node;
            WeightToNode = weightToNode;
        }
    }

    public class Edge
    {
        public float Value { get; }
        public Node Node1 { get; }
        public Node Node2 { get; }

        public Edge(float value, Node node1, Node node2)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Edge value needs to be positive.");
            }
            Value = value;
            Node1 = node1;
            node1.Assign(this);
            Node2 = node2;
            node2.Assign(this);
        }

        public static Edge Create(float value, Node node1, Node node2)
        {
            return new Edge(value, node1, node2);
        }
    }

    float CalcularDistanciaXZ(Transform obj1, Transform obj2)
    {
        Vector2 pos1 = new(obj1.position.x, obj1.position.z);
        Vector2 pos2 = new(obj2.position.x, obj2.position.z);

        return Vector2.Distance(pos1, pos2);
    }
}