// using System;
// using System.Diagnostics;
// using System.Threading;
//
// class Graph
// {
//     public int _vertices;
//     public int[,] _edges;
//
//     public Graph(int vertices)
//     {
//         _vertices = vertices;
//         _edges = new int[vertices, vertices];
//     }
//
//     public void AddEdge(int u, int v, int weight)
//     {
//         // Make sure we do not add edges out of bounds
//         if (u < 0 || u >= _vertices || v < 0 || v >= _vertices)
//         {
//             throw new ArgumentException("The edge is out of bounds of the graph");
//         }
//         _edges[u, v] = weight;
//         _edges[v, u] = weight;
//     }
//
//     public void PrimMST()
//     {
//         int[] key = new int[_vertices];
//         bool[] setMST = new bool[_vertices];
//         int[] parent = new int[_vertices];
//
//         for (int i = 0; i < _vertices; i++)
//         {
//             key[i] = int.MaxValue;
//             setMST[i] = false;
//         }
//
//         key[0] = 0;
//         parent[0] = -1;
//
//         for (int count = 0; count < _vertices - 1; count++)
//         {
//             int u = MinKey(key, setMST);
//             setMST[u] = true;
//
//             for (int v = 0; v < _vertices; v++)
//             {
//                 if (_edges[u, v] != 0 && !setMST[v] && _edges[u, v] < key[v])
//                 {
//                     parent[v] = u;
//                     key[v] = _edges[u, v];
//                 }
//             }
//         }
//
//         PrintMST(parent);
//     }
//
//     private int MinKey(int[] key, bool[] setMST)
//     {
//         int min = int.MaxValue, minIndex = -1;
//
//         for (int i = 0; i < _vertices; i++)
//         {
//             if (!setMST[i] && key[i] < min)
//             {
//                 min = key[i];
//                 minIndex = i;
//             }
//         }
//
//         return minIndex;
//     }
//
//     private void PrintMST(int[] parent)
//     {
//         Console.WriteLine("Edge \tWeight");
//         for (int i = 1; i < _vertices; i++)
//             Console.WriteLine($"{parent[i]} - {i} \t{_edges[i, parent[i]]}");
//     }
// }
//
// class Program
// {
//     static void Main(string[] args)
//     {
//         int vertices = 6;
//         Graph graph = new Graph(vertices);
//
//         graph.AddEdge(0, 1, 4);
//         graph.AddEdge(0, 2, 4);
//         graph.AddEdge(1, 2, 2);
//         graph.AddEdge(1, 3, 6);
//         graph.AddEdge(2, 3, 8);
//         // Add more edges as needed...
//
//         Stopwatch stopwatch = Stopwatch.StartNew();
//         graph.PrimMST();
//         stopwatch.Stop();
//         Console.WriteLine($"Time taken without threads: {stopwatch.ElapsedMilliseconds} ms");
//
//         int threadCount = 4;
//         Thread[] threads = new Thread[threadCount];
//         stopwatch.Restart();
//
//         for (int i = 0; i < threadCount; i++)
//         {
//             // Creating a separate graph instance for each thread to avoid any concurrency issues
//             Graph threadGraph = new Graph(vertices);
//             CopyEdges(graph, threadGraph);
//             threads[i] = new Thread(threadGraph.PrimMST);
//             threads[i].Start();
//         }
//
//         foreach (Thread thread in threads)
//         {
//             thread.Join();
//         }
//
//         stopwatch.Stop();
//         Console.WriteLine($"Time taken with {threadCount} threads: {stopwatch.ElapsedMilliseconds} ms");
//     }
//
//     static void CopyEdges(Graph original, Graph copy)
//     {
//         for (int i = 0; i < original._vertices; i++)
//         {
//             for (int j = 0; j < original._vertices; j++)
//             {
//                 if (original._edges[i, j] != 0)
//                 {
//                     copy.AddEdge(i, j, original._edges[i, j]);
//                 }
//             }
//         }
//     }
// }
