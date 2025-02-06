using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Agente : MonoBehaviour
{
    [SerializeField] Text pathText;
    [SerializeField] Dropdown partida, chegada;
    [SerializeField] Material partidaMat, chegadaMat, percusoMat, defaultMat;
    [SerializeField] LayerMask layerMask;

    Node[] path;
    public Node[] nodes;

    private void Start()
    {
        var objects = FindObjectsByType<Manager>(FindObjectsSortMode.None);
        nodes = new Node[objects.Length];

        foreach (var item in objects)
        {
            string label = item.name[^2..].Trim();
            Manager manager = item.GetComponent<Manager>();

            var node = manager.CriarNode(label, layerMask);
            if (label.Equals("A")) manager.Ativar();

            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i] == null)
                {
                    nodes[i] = node;
                    break;
                }
        }

        SortNodes();

        nodes[0].Obj.GetComponentInChildren<Manager>().Ativar();

        List<string> options = new();

        foreach (var item in nodes)
            options.Add(item.Label);

        partida.AddOptions(options);
        chegada.AddOptions(options);
    }

    public void OnValueChanged(Int32 value)
    {
        GameObject item = EventSystem.current.currentSelectedGameObject;
        Transform caller = item.transform;
        for (int i = 0; i < 4; i++) caller = caller.parent;
        Dropdown dropdown = caller.GetComponent<Dropdown>();

        var material = GetMaterial(caller.name);

        for (int i = 0; i < nodes.Length; i++)
        {
            var temp = GetMaterial(nodes[i]).name.Remove(material.name.Length);

            if (temp.Equals(material.name)) SetMaterial(nodes[i], defaultMat);
        }

        if (value > 0)
        {
            var temp = GetMaterial(nodes[value - 1]).name.Remove(defaultMat.name.Length);
            var temp2 = GetMaterial(nodes[value - 1]).name.Remove(percusoMat.name.Length);

            if (temp.Equals(defaultMat.name) || temp2.Equals(percusoMat.name)) SetMaterial(nodes[value - 1], material);
            else dropdown.value = 0;
        }

        Dijkstra();
    }

    private void Dijkstra()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = GetMaterial(nodes[i]).name.Remove(percusoMat.name.Length);

            if (node.Equals(percusoMat.name)) SetMaterial(nodes[i], defaultMat);
        }

        if (partida.value > 0 && chegada.value > 0)
        {
            IShortestPathFinder pathFinder = new Dijkstra();

            Node node1 = nodes[partida.value - 1], node2 = nodes[chegada.value - 1];

            path = pathFinder.FindShortestPath(node1, node2);

            if (path != null)
            {
                var temp = "Caminho:\n";
                foreach (var node in path)
                {
                    temp += $"{node.Label} - ";

                    var temp2 = GetMaterial(node).name.Remove(defaultMat.name.Length);
                    if (temp2.Equals(defaultMat.name)) SetMaterial(node, percusoMat);
                }
                    
                pathText.text = temp[..^3];
            }
            else print("Não há caminho disponível entre os nós especificados.");
        }
    }

    private void SortNodes()
    {
        while (true)
        {
            bool stop = true;

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                if (nodes[i].Label.Length > nodes[i + 1].Label.Length)
                {
                    (nodes[i], nodes[i + 1]) = (nodes[i + 1], nodes[i]);
                    stop = false;
                }
                else if (nodes[i].Label.Length == nodes[i + 1].Label.Length)
                {
                    if (nodes[i].Label[^1] > nodes[i + 1].Label[^1])
                    {
                        (nodes[i], nodes[i + 1]) = (nodes[i + 1], nodes[i]);
                        stop = false;
                    }
                }
            }

            if (stop) break;
        }
    }

    private void SetMaterial(Node node, Material material)
    {
        MeshRenderer renderer = node.Obj.GetComponentInChildren<MeshRenderer>();
        renderer.material = material;
    }

    private Material GetMaterial(Node node)
    {
        MeshRenderer renderer = node.Obj.GetComponentInChildren<MeshRenderer>();
        return renderer.material;
    }

    private Material GetMaterial(string caller)
    {
        return caller switch
        {
            "Partida" => partidaMat,
            "Chegada" => chegadaMat,
            _ => null
        };
    }
}