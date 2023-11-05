using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Graph2
{
    private int[,] adjacencyMatrix;
    private int numThreads = 0;

    public Graph2(int numVertices)
    {
        adjacencyMatrix = new int[numVertices, numVertices];
    }

    public void setTheads(int k)
    {
        numThreads = k;
    }
    public void AddEdge(int from, int to, int weight)
    {
        adjacencyMatrix[from, to] = weight;
    }

    public List<int> ShortestPathDijkstra(int startVertex)
    {
        int numVertices = adjacencyMatrix.GetLength(0);
        int[] distances = new int[numVertices];
        bool[] visited = new bool[numVertices];

        for (int i = 0; i < numVertices; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
        }

        distances[startVertex] = 0;

        for (int i = 0; i < numVertices - 1; i++)
        {
            int minDistance = int.MaxValue;
            int minIndex = -1;

            for (int v = 0; v < numVertices; v++)
            {
                if (!visited[v] && distances[v] < minDistance)
                {
                    minDistance = distances[v];
                    minIndex = v;
                }
            }

            visited[minIndex] = true;

            for (int v = 0; v < numVertices; v++)
            {
                int edgeWeight = adjacencyMatrix[minIndex, v];
                if (!visited[v] && edgeWeight > 0 && distances[minIndex] + edgeWeight < distances[v])
                {
                    distances[v] = distances[minIndex] + edgeWeight;
                }
            }
        }

        return distances.ToList();
    }
    public List<int> ShortestPathDijkstraParallel(int startVertex)
    {
        int numVertices = adjacencyMatrix.GetLength(0);
        int[] distances = new int[numVertices];
        bool[] visited = new bool[numVertices];
        Thread[] threads = new Thread[numVertices];

        for (int i = 0; i < numVertices; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
        }

        distances[startVertex] = 0;

        for (int i = 0; i < numVertices - 1; i++)
        {
            int minDistance = int.MaxValue;
            int minIndex = -1;

            for (int v = 0; v < numVertices; v++)
            {
                if (!visited[v] && distances[v] < minDistance)
                {
                    minDistance = distances[v];
                    minIndex = v;
                }
            }

            visited[minIndex] = true;

            for (int v = 0; v < numVertices; v++)
            {
                if (!visited[v] && adjacencyMatrix[minIndex, v] > 0 && distances[minIndex] + adjacencyMatrix[minIndex, v] < distances[v])
                {
                    int vertex = v;
                    threads[v] = new Thread(() =>
                    {
                        distances[vertex] = distances[minIndex] + adjacencyMatrix[minIndex, vertex];
                    });
                    threads[v].Start();
                }
            }

            for (int v = 0; v < numVertices; v++)
            {
                threads[v]?.Join();
            }
        }

        return distances.ToList();
    }
}
public class GraphGenerator
{
    public static Graph2 GenerateRandomGraph(int numVertices, int numEdges, int maxWeight)
    {
        Random random = new Random();
        Graph2 graph = new Graph2(numVertices);

        for (int i = 0; i < numEdges; i++)
        {
            int from = random.Next(numVertices);
            int to = random.Next(numVertices);
            int weight = random.Next(1, maxWeight);

            graph.AddEdge(from, to, weight);
        }

        return graph;
    }
}


class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        int k = 6; // Потоки
        int numVertices = 1000; // Кількість вершин
        int numEdges = 10000; // Кількість ребер
        int maxWeight = 1000; // Максимальна вага ребра
        Graph2 graph = GraphGenerator.GenerateRandomGraph(numVertices, numEdges, maxWeight);
        graph.setTheads(k);
        Graph2 graphParallel = graph;
        int startVertex = 0; // Початкова вершина
        
        
        // int numVertices = 4; // Кількість вершин
        // Graph2 graph = new Graph2(numVertices);
        //
        // graph.AddEdge(0, 1, 5);
        // graph.AddEdge(1, 2, 3);
        // graph.AddEdge(2,3,8);
        // graph.AddEdge(1,3,2);
        // int startVertex = 0; // Початкова вершина
        // Graph2 graphParallel = graph;
        
        
        stopwatch.Start();
        List<int> shortestDistances = graph.ShortestPathDijkstra(startVertex);
        stopwatch.Stop();
        long timeOrderedMilisec = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Час послiдовного методу : {timeOrderedMilisec} мiлiсекунди \n");
        stopwatch.Reset();
        
        stopwatch.Start();
        List<int> shortestDistancesParallel = graphParallel.ShortestPathDijkstraParallel(startVertex);
        stopwatch.Stop();
        long timeOrderedMilisecParallel = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Час паралельного методу : {timeOrderedMilisecParallel} мiлiсекунди \n");
        stopwatch.Reset();
        
        double speedUp = (double)timeOrderedMilisec / (double)timeOrderedMilisecParallel;
        double efficiency = (double)speedUp / k;
        
        Console.WriteLine($"Кiлькiсть потокiв: {k}");
        Console.WriteLine($"Прискорення : {speedUp}");
        Console.WriteLine($"Ефективнiсть методу : {efficiency}");
        
        // Console.WriteLine("\nШляхи для послiдовного методу:\n");
        // for (int i = 0; i < shortestDistances.Count; i++)
        // {
        //     Console.WriteLine($"Найкоротший шлях вiд {startVertex} до вершини {i}: {shortestDistances[i]}");
        // }
        // Console.WriteLine("\nШляхи для паралельного методу:\n");
        // for (int i = 0; i < shortestDistancesParallel.Count; i++)
        // {
        //     Console.WriteLine($"Найкоротший шлях вiд {startVertex} до вершини {i}: {shortestDistancesParallel[i]}");
        // }
    }
}
