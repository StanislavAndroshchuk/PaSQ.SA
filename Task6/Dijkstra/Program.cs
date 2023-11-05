using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Graph
{
    private Dictionary<string, List<Edge>> adjacencyList;

    public Graph()
    {
        adjacencyList = new Dictionary<string, List<Edge>>();
    }

    public void AddVertex(string vertex)
    {
        if (!adjacencyList.ContainsKey(vertex))
            adjacencyList[vertex] = new List<Edge>();
    }

    public void AddEdge(string vertex1, string vertex2, int weight)
    {
        adjacencyList[vertex1].Add(new Edge(vertex2, weight));
        
    }

    public List<int> ShortestPathDijkstra(string startVertex)
    {
        Dictionary<string, int> distances = new Dictionary<string, int>();
        foreach (var vertex in adjacencyList.Keys)
        {
            distances[vertex] = int.MaxValue;
        }

        distances[startVertex] = 0;

        PriorityQueue<string> queue = new PriorityQueue<string>();
        queue.Enqueue(startVertex, 0);

        while (!queue.IsEmpty)
        {
            var currentVertex = queue.Dequeue();

            foreach (var edge in adjacencyList[currentVertex])
            {
                var neighbor = edge.Target;
                var tentativeDistance = distances[currentVertex] + edge.Weight;

                if (tentativeDistance < distances[neighbor])
                {
                    distances[neighbor] = tentativeDistance;
                    queue.Enqueue(neighbor, tentativeDistance);
                }
            }
        }

        List<int> shortestDistances = distances.Values.ToList();
        return shortestDistances;
    }
}

public class Edge
{
    public string Target { get; }
    public int Weight { get; }

    public Edge(string target, int weight)
    {
        Target = target;
        Weight = weight;
    }
}

public class PriorityQueue<T>
{
    private SortedDictionary<int, Queue<T>> data = new SortedDictionary<int, Queue<T>>();

    public bool IsEmpty
    {
        get { return data.Count == 0; }
    }

    public void Enqueue(T item, int priority)
    {
        if (!data.ContainsKey(priority))
            data[priority] = new Queue<T>();

        data[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        var item = data.First();
        var priority = item.Key;
        var queue = item.Value;

        var result = queue.Dequeue();
        if (queue.Count == 0)
            data.Remove(priority);

        return result;
    }
}

// class Program2
// {
//     static void Main(string[] args)
//     {
//         Stopwatch stopwatch = new Stopwatch();
//         // Створюємо граф та додаємо вершини та ребра
//         Graph graph = new Graph();
//         graph.AddVertex("a0");
//         graph.AddVertex("a1");
//         graph.AddVertex("a2");
//         // Додайте інші вершини та ребра
//
//         graph.AddEdge("a0", "a1", 5);
//         graph.AddEdge("a1", "a2", 3);
//         // Додайте інші ребра
//
//         string startVertex = "a0"; // Початкова вершина
//
//         // Знаходимо найкоротший шлях
//         stopwatch.Start();
//         List<int> shortestDistances = graph.ShortestPathDijkstra(startVertex);
//         stopwatch.Stop();
//         long timeOrderedMilisec = stopwatch.ElapsedMilliseconds;
//         Console.WriteLine($"Час послiдовного методу : {timeOrderedMilisec} мiлiсекунди \n");
//         // Виводимо результат
//         for (int i = 0; i < shortestDistances.Count; i++)
//         {
//             Console.WriteLine($"Найкоротший шлях від {startVertex} до вершини a{i}: {shortestDistances[i]}");
//         }
//     }
// }
