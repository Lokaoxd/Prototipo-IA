using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    Node node, nodeAnterior;
    LayerMask mask;
    bool ativado;
    float scaleCube;

    readonly List<Node> nodesConnected = new();

    private void Awake()
        => scaleCube = transform.GetChild(0).localScale.x/2;

    public Node CriarNode(string label, LayerMask _mask)
    {
        node = new(label, transform);
        mask = _mask;
        return node;
    }
    public void Ativar(Node node = null)
    {
        ativado = true;
        nodeAnterior = node;
    }
    public void AddNode(Node node) => nodesConnected.Add(node);
    private bool Contains(Node node)
    {
        foreach (var item in nodesConnected)
            if (item.Label.Equals(node.Label))
                return true;
        return false;
    }

    private void FixedUpdate()
    {
        if (!ativado) return;

        // FRENTE
        if (Physics.Raycast(transform.position + (Vector3.forward * scaleCube), transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, mask))
        {
            hit.collider.transform.parent.TryGetComponent(out Manager manager);

            if (manager != null)
            {
                if (manager.node != nodeAnterior)
                {
                    if (!Contains(manager.node))
                    {
                        manager.AddNode(node);
                        AddNode(manager.node);
                        node.ConnectTo(manager.node);
                        manager.Ativar(node);
                    }
                    else Debug.DrawRay(transform.position + (Vector3.forward * scaleCube), transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                }
            }
            else Debug.DrawRay(transform.position + (Vector3.forward * scaleCube), transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
        }

        // DIREITA
        if (Physics.Raycast(transform.position + (Vector3.right * scaleCube), transform.TransformDirection(Vector3.right), out RaycastHit hit1, Mathf.Infinity, mask))
        {
            hit1.collider.transform.parent.TryGetComponent(out Manager manager);

            if (manager != null)
            {
                if (manager.node != nodeAnterior)
                {
                    if (!Contains(manager.node))
                    {
                        manager.AddNode(node);
                        AddNode(manager.node);
                        node.ConnectTo(manager.node);
                        manager.Ativar(node);
                    }
                    Debug.DrawRay(transform.position + (Vector3.right * scaleCube), transform.TransformDirection(Vector3.right) * hit1.distance, Color.green);
                }
            }
            else Debug.DrawRay(transform.position + (Vector3.right * scaleCube), transform.TransformDirection(Vector3.right) * hit1.distance, Color.red);
        }

        // ESQUERDA
        if (Physics.Raycast(transform.position + (Vector3.left * scaleCube), transform.TransformDirection(Vector3.left), out RaycastHit hit2, Mathf.Infinity, mask))
        {
            hit2.collider.transform.parent.TryGetComponent(out Manager manager);

            if (manager != null)
            {
                if (manager.node != nodeAnterior)
                {
                    if (!Contains(manager.node))
                    {
                        manager.AddNode(node);
                        AddNode(manager.node);
                        node.ConnectTo(manager.node);
                        manager.Ativar(node);
                    }
                    Debug.DrawRay(transform.position + (Vector3.left * scaleCube), transform.TransformDirection(Vector3.left) * hit2.distance, Color.green);
                }
            }
            else Debug.DrawRay(transform.position + (Vector3.left * scaleCube), transform.TransformDirection(Vector3.left) * hit2.distance, Color.red);
        }

        // ATRAS
        if (Physics.Raycast(transform.position + (Vector3.back * scaleCube), transform.TransformDirection(Vector3.back), out RaycastHit hit3, Mathf.Infinity, mask))
        {
            hit3.collider.transform.parent.TryGetComponent(out Manager manager);

            if (manager != null)
            {
                if (manager.node != nodeAnterior)
                {
                    if (!Contains(manager.node))
                    {
                        manager.AddNode(node);
                        AddNode(manager.node);
                        node.ConnectTo(manager.node);
                        manager.Ativar(node);
                    }
                    Debug.DrawRay(transform.position + (Vector3.back * scaleCube), transform.TransformDirection(Vector3.back) * hit3.distance, Color.green);
                }
            }
            else Debug.DrawRay(transform.position + (Vector3.back * scaleCube), transform.TransformDirection(Vector3.back) * hit3.distance, Color.red);
        }
    }
}