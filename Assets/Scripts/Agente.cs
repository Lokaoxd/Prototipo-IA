using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class Agente : MonoBehaviour
{
    Transform target;
    [SerializeField] Text pathText, agenteFeedback;
    [SerializeField] LayerMask layerMask;
    NavMeshAgent agent;

    Node[] path;
    public Node[] nodes;
    int pathPosition = 0;
    bool ativado = false;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    private void Start()
    {
        var objects = FindObjectsByType<Manager>(FindObjectsSortMode.None);
        nodes = new Node[objects.Length];

        foreach (var item in objects)
        {
            string label = item.name[^1].ToString();
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

        // Proximo passo, colocar pra selecionar o caminho pelo Canvas baseado nos nodes ordenados
        // Criar um botao Começar para ativar o agente

        IShortestPathFinder pathFinder = new Dijkstra();
        //path = pathFinder.FindShortestPath(a, p);

        if (path != null)
        {
            var temp = "Caminho: ";
            foreach (var node in path)
                temp += $"{node.Label} - ";
            pathText.text = temp[..^3];
        }
        else print("Não há caminho disponível entre os nós especificados.");
    }

    void Update()
    {
        if (!ativado) return;
        if (pathPosition < path.Length)
        {
            if (pathPosition > 0)
                agenteFeedback.text = $"O agente esta se direcionando para o ponto {path[pathPosition].Label}";

            if (Vector3.Distance(transform.position, path[pathPosition].Obj.position) > 1f)
                agent.SetDestination(path[pathPosition].Obj.position);
            else pathPosition++;
        }
        else agenteFeedback.text = "O agente chegou ao destino";
    }

    private void SortNodes()
    {
        while (true)
        {
            bool stop = true;

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                if (nodes[i].Label[0] > nodes[i + 1].Label[0])
                {
                    (nodes[i], nodes[i + 1]) = (nodes[i + 1], nodes[i]);
                    stop = false;
                }
            }

            if (stop) break;
        }
    }
}