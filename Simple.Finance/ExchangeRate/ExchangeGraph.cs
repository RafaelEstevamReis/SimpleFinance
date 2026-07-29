using System;
using System.Collections.Generic;

namespace Simple.Finance.ExchangeRate;

public class ExchangeGraph
{
    private readonly Dictionary<string, List<Edge>> graphTable;

    public ExchangeGraph(IEnumerable<IExchangeRateTable> tables)
    {
        graphTable = BuildGraph(tables);
    }
    private Dictionary<string, List<Edge>> BuildGraph(IEnumerable<IExchangeRateTable> tables)
    {
        var graph = new Dictionary<string, List<Edge>>(StringComparer.OrdinalIgnoreCase);

        foreach (var table in tables)
        {
            foreach (var (baseCur, quoteCur) in table.AvailableCurrencyPairs())
            {
                // Direto: base -> quote
                AddEdge(graph, baseCur, quoteCur, new Node(baseCur, quoteCur, table, false));

                // Invertido: quote -> base
                AddEdge(graph, quoteCur, baseCur, new Node(baseCur, quoteCur, table, true));
            }
        }
        return graph;
    }

    private void AddEdge(Dictionary<string, List<Edge>> graph, string from, string to, Node node)
    {
        if (!graph.ContainsKey(from)) graph[from] = [];

        graph[from].Add(new Edge(to, node));
    }
    public Node[] GetRoute(string startCur, string targetCur)
    {
        if (string.Equals(startCur, targetCur, StringComparison.OrdinalIgnoreCase)) return [];

        var graph = graphTable;

        var queue = new Queue<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // must match 'visited'/'graphTable': edges are keyed with the casing declared by the table,
        // while startCur/targetCur come from the caller
        var parent = new Dictionary<string, (string from, Node nodeUsed)>(StringComparer.OrdinalIgnoreCase);

        queue.Enqueue(startCur);
        visited.Add(startCur);

        bool found = false;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!graph.ContainsKey(current)) continue;

            foreach (var edge in graph[current])
            {
                if (visited.Contains(edge.To)) continue;

                visited.Add(edge.To);
                parent[edge.To] = (current, edge.Node);
                queue.Enqueue(edge.To);

                if (string.Equals(edge.To, targetCur, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (found) break;
        }

        if (!found) return Array.Empty<Node>();

        return ReconstructPath(parent, startCur, targetCur);
    }

    private Node[] ReconstructPath(Dictionary<string, (string from, Node nodeUsed)> parent, string start, string target)
    {
        var path = new List<Node>();
        var current = target;

        while (!string.Equals(current, start, StringComparison.OrdinalIgnoreCase))
        {
            var (prev, node) = parent[current];
            path.Add(node);
            current = prev;
        }

        path.Reverse();
        return path.ToArray();
    }

    public class Node(string baseCur, string quoteCur, IExchangeRateTable table, bool inverted)
    {
        public string BaseCur { get; set; } = baseCur;
        public string QuoteCur { get; set; } = quoteCur;
        public IExchangeRateTable Table { get; set; } = table;
        public bool Inverted { get; set; } = inverted;
    }

    public class Edge(string to, Node node)
    {
        public string To { get; set; } = to;
        public Node Node { get; set; } = node;
    }
}