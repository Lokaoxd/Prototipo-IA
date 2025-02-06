using System.Linq;
using System.Collections.Generic;

interface IShortestPathFinder
{
    Node[] FindShortestPath(Node from, Node to);
}

public class Dijkstra : IShortestPathFinder
{
    private class Weight
    {
        public Node From { get; }
        public float Value { get; }

        public Weight(Node @from, float value)
        {
            From = @from;
            Value = value;
        }
    }

    class VisitingData
    {
        readonly List<Node> _visiteds = new();

        readonly Dictionary<Node, Weight> _weights = new();

        readonly List<Node> _scheduled = new();

        public void RegisterVisitTo(Node node)
        {
            if (!_visiteds.Contains(node))
                _visiteds.Add((node));
        }

        public bool WasVisited(Node node)
        {
            return _visiteds.Contains(node);
        }

        public void UpdateWeight(Node node, Weight newWeight)
        {
            if (!_weights.ContainsKey(node))
            {
                _weights.Add(node, newWeight);
            }
            else
            {
                _weights[node] = newWeight;
            }
        }

        public Weight QueryWeight(Node node)
        {
            Weight result;
            if (!_weights.ContainsKey(node))
            {
                result = new Weight(null, int.MaxValue);
                _weights.Add(node, result);
            }
            else
            {
                result = _weights[node];
            }
            return result;
        }

        public void ScheduleVisitTo(Node node)
        {
            _scheduled.Add(node);
        }

        public bool HasScheduledVisits => _scheduled.Count > 0;

        public Node GetNodeToVisit()
        {
            var ordered = from n in _scheduled
                          orderby QueryWeight(n).Value
                          select n;

            var result = ordered.First();
            _scheduled.Remove(result);
            return result;
        }

        public bool HasComputedPathToOrigin(Node node)
        {
            return QueryWeight(node).From != null;
        }

        public IEnumerable<Node> ComputedPathToOrigin(Node node)
        {
            var n = node;
            while (n != null)
            {
                yield return n;
                n = QueryWeight(n).From;
            }
        }
    }

    public Node[] FindShortestPath(Node @from, Node to)
    {
        var control = new VisitingData();

        control.UpdateWeight(@from, new Weight(null, 0));
        control.ScheduleVisitTo(@from);

        while (control.HasScheduledVisits)
        {
            var visitingNode = control.GetNodeToVisit();
            var visitingNodeWeight = control.QueryWeight(visitingNode);
            control.RegisterVisitTo(visitingNode);

            foreach (var neighborhoodInfo in visitingNode.Neighbors)
            {
                if (!control.WasVisited(neighborhoodInfo.Node))
                {
                    control.ScheduleVisitTo(neighborhoodInfo.Node);
                }

                var neighborWeight = control.QueryWeight(neighborhoodInfo.Node);

                var probableWeight = (visitingNodeWeight.Value + neighborhoodInfo.WeightToNode);
                if (neighborWeight.Value > probableWeight)
                {
                    control.UpdateWeight(neighborhoodInfo.Node, new Weight(visitingNode, probableWeight));
                }
            }
        }

        return control.HasComputedPathToOrigin(to)
            ? control.ComputedPathToOrigin(to).Reverse().ToArray()
            : null;
    }
}