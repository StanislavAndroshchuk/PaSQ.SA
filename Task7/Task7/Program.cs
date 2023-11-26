using System;
using System.Collections.Generic;


class Graph3
{
    private int _vertices;
    private int[,] _graph;
    private Random _random = new Random();

    public Graph3(int vertices)
    {
        _vertices = vertices;
        _graph = new int[vertices, vertices];
    }

    public int[,] getGraph()
    {
        return _graph;
    }

    public void GenerateRandomGraph(int edgeDensity)
    {
        // Ensure the graph is connected
        for (int i = 1; i < _vertices; i++)
        {
            int weight = _random.Next(1, 100); // Random weight between 1 and 10
            AddEdge(i, i - 1, weight);
        }

        // Add additional edges based on edge density
        int additionalEdges = (int)(edgeDensity * _vertices * (_vertices - 1) / 200.0);

        while (additionalEdges > 0)
        {
            int u = _random.Next(_vertices);
            int v = _random.Next(_vertices);
            if (u != v && _graph[u, v] == 0)
            {
                int weight = _random.Next(1, 100); // Random weight between 1 and 10
                AddEdge(u, v, weight);
                additionalEdges--;
            }
        }
    }
    public void PrimMSTParallel(int startVertex, int threadCount)
    {
        int[] parent = new int[_vertices];
        int[] key = new int[_vertices];
        bool[] mstSet = new bool[_vertices];
        object lockObject = new object();  // For sync resources

        for (int i = 0; i < _vertices; i++)
        {
            key[i] = int.MaxValue;
            mstSet[i] = false;
        }

        key[startVertex] = 0;
        parent[startVertex] = -1;

        for (int count = 0; count < _vertices - 1; count++)
        {
            int u = MinKey(key, mstSet);

            mstSet[u] = true;

     
            int verticesPerThread = _vertices / threadCount;
            Thread[] threads = new Thread[threadCount];

            for (int t = 0; t < threadCount; t++)
            {
                int start = t * verticesPerThread;
                int end = (t == threadCount - 1) ? _vertices : start + verticesPerThread;
                threads[t] = new Thread(() => {
                    for (int v = start; v < end; v++)
                    {
                        if (_graph[u, v] != 0 && mstSet[v] == false && _graph[u, v] < key[v])
                        {
                            lock (lockObject)  // Synchronize access to shared resources
                            {
                                parent[v] = u;
                                key[v] = _graph[u, v];
                            }
                        }
                    }
                });

                threads[t].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();  
            }
        }

        //PrintMST(parent, startVertex);
    }
    

    public void AddEdge(int u, int v, int weight)
    {
        _graph[u, v] = weight;
        _graph[v, u] = weight; // Since the graph is undirected
    }
    
    
    private int MinKey(int[] key, bool[] setMST)
    {
        int min = int.MaxValue, minIndex = -1;

        for (int v = 0; v < _vertices; v++)
            if (setMST[v] == false && key[v] < min)
            {
                min = key[v];
                minIndex = v;
            }

        return minIndex;
    }

    public void PrimMST(int startVertex)
    {
        int[] parent = new int[_vertices]; // Array to store constructed MST
        int[] key = new int[_vertices];   // Key values used to pick minimum weight edge in cut
        bool[] mstSet = new bool[_vertices];  // To represent set of vertices not yet included in MST

        for (int i = 0; i < _vertices; i++)
        {
            key[i] = int.MaxValue;
            mstSet[i] = false;
        }

        key[startVertex] = 0;     // Start from the startVertex, make key 0
        parent[startVertex] = -1; // The startVertex is the root of MST

        for (int count = 0; count < _vertices - 1; count++)
        {
            int u = MinKey(key, mstSet);

            mstSet[u] = true;

            for (int v = 0; v < _vertices; v++)
                if (_graph[u, v] != 0 && mstSet[v] == false && _graph[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = _graph[u, v];
                }
        }

        //PrintMST(parent, startVertex);
    }

    private void PrintMST(int[] parent, int startVertex)
    {
        Console.WriteLine("Edge \tWeight");
        for (int i = 0; i < _vertices; i++)
        {
            if (parent[i] != -1) // Skip the startVertex which has no parent
                Console.WriteLine(parent[i] + " - " + i + "\t" + _graph[i, parent[i]]);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        int vertices = 10000; // A larger number of vertices
        int edgeDensity = 50; // Percentage of possible edges to include in the graph
        int k = 4; // Number of threads for parallelization
        int startVertex = 0; // Starting vertex for Prim's algorithm;
        Graph3 g = new Graph3(vertices);
        g.GenerateRandomGraph(edgeDensity);
        Graph3 g2 = g;
        
        // Measure time
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();    
        g.PrimMST(startVertex);
        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        double timeOrderedMilisec = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();
        g2.PrimMSTParallel(startVertex,k);
        stopwatch.Stop();
        Console.WriteLine($"Execution Time Parallel: {stopwatch.ElapsedMilliseconds} ms");
        double timeOrderedMilisecParallel = (double)stopwatch.ElapsedMilliseconds;
        double speedUp = (double)timeOrderedMilisec / (double)timeOrderedMilisecParallel;
        double efficiency = (double)speedUp / k;
        
        Console.WriteLine($"Amount of threads: {k}");
        Console.WriteLine($"Speed up : {speedUp}");
        Console.WriteLine($"Efficiency : {efficiency}");
        Console.WriteLine($"Amount of vertices : {vertices}");
    
        Console.ReadKey();
    }
    
    //For manual input
    
    // static void Main(string[] args)
    // {
    //     int vertices = 6; // Example for 6 vertices
    //     int startVertex = 0;
    //     Graph3 g = new Graph3(vertices);
    //
    //     // Example edges
    //     g.AddEdge(0, 1, 4);
    //     g.AddEdge(0, 2, 4);
    //     g.AddEdge(1, 2, 2);
    //     g.AddEdge(1, 0, 4);
    //     g.AddEdge(2, 0, 4);
    //     g.AddEdge(2, 1, 2);
    //     g.AddEdge(2, 3, 3);
    //     g.AddEdge(2, 5, 2);
    //     g.AddEdge(2, 4, 4);
    //     g.AddEdge(3, 2, 3);
    //     g.AddEdge(3, 4, 3);
    //     g.AddEdge(4, 2, 4);
    //     g.AddEdge(4, 3, 3);
    //     g.AddEdge(5, 2, 2);
    //     g.AddEdge(5, 4, 3);
    //
    //     // Measure time
    //     var watch = System.Diagnostics.Stopwatch.StartNew();
    //     g.PrimMST(startVertex);
    //     watch.Stop();
    //     Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    //
    //     Console.ReadKey();
    // }
}