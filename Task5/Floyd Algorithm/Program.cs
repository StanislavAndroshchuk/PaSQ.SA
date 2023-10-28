using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
class Floyd
{
    public static void Main()
    {
        Stopwatch stopwatch = new Stopwatch();
        int n = 600; // Кількість вершин графа //
        int startNode = 0; // Початкова вершина
        int endNode = 599;   // Кінцева вершина //
        int k =100; // кількість потоків
        
        //----------------------------------------------
        int[,] graph = GenerateRandomGraph(n);
        // int[,] graph = new int[n, n]; // Зважений граф 
        //
        //  // Ініціалізація матриці
        //  for (int i = 0; i < n; i++)
        //  {
        //      for (int j = 0; j < n; j++)
        //      {
        //          if (i == j)
        //              graph[i, j] = 0;
        //          else
        //              graph[i, j] = int.MaxValue; // Максимальна відстань для початку
        //      }
        //  }
        //
        //  // Додаємо ребра та їх ваги до графа
        //  graph[0, 1] = 2;
        //  graph[0, 2] = 4;
        //  graph[1, 2] = 1;
        //  graph[1, 3] = 7;
        //  graph[2, 4] = 3;
        //  graph[3, 4] = 2;
        //-----------------------------------------------------
        
        //-------------------------Послідовне виконання -------
        
        stopwatch.Start();
        int[,] shortestPaths = FloydNonParallel(graph, n);
        stopwatch.Stop();
        long timeOrderedMilisec = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Час послiдовного методу : {timeOrderedMilisec} мiлiсекунди \n");

        int shortestDistance = shortestPaths[startNode, endNode];
        if (shortestDistance == int.MaxValue)
        {
            Console.WriteLine("Немає шляху між вершинами.");
        }
        else
        {
            Console.WriteLine($"Найкоротший шлях мiж вершиною {startNode} i {endNode} = {shortestDistance}");
        }
        stopwatch.Reset();

        //---------------------------------------------------------
        //--------------------Паралельне виконання ----------------
        
        stopwatch.Start();
        FloydParallel(graph,n,k);
        stopwatch.Stop();
        long timeParallelMilisec = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Час паралельного методу : {timeParallelMilisec} мiлiсекунди \n");

        int shortestDistanceParallel = graph[startNode, endNode];
        if (shortestDistanceParallel == int.MaxValue)
        {
            Console.WriteLine("Немає шляху між вершинами.");
        }
        else
        {
            Console.WriteLine($"Найкоротший шлях мiж вершиною {startNode} i {endNode} = {shortestDistanceParallel}");
        }
        stopwatch.Reset();
        long speedUp = timeOrderedMilisec / timeParallelMilisec;
        double efficiency = (double)speedUp / k;
        
        Console.WriteLine($"Прискорення : {speedUp}");
        Console.WriteLine($"Ефективнiсть методу : {efficiency}");
    }

    public static int[,] FloydNonParallel(int[,] graph, int n)
    {
        
        int[,] dist = new int[n, n];

        // Ініціалізуємо матрицю найкоротших відстаней так, як початкова матриця ваг
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                dist[i, j] = graph[i, j];
            }
        }

        // Реалізуємо алгоритм Флойда
        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue &&
                        dist[i, k] + dist[k, j] < dist[i, j])
                    {
                        dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            }
        }
        // // Виводимо результат
        // for (int i = 0; i < n; i++)
        // {
        //     for (int j = 0; j < n; j++)
        //     {
        //         if (dist[i, j] == int.MaxValue)
        //             Console.Write("INF\t");
        //         else
        //             Console.Write(dist[i, j] + "\t");
        //     }
        //     Console.WriteLine();
        // }

        return dist;
    }
    


    public static void FloydParallel(int[,] graph, int n, int k)
    {
        var threads = new Thread[k];

        // Розбиваємо матрицю ваг на k частин рівного розміру
        int partitionSize = graph.GetLength(0) / k;

        // Запускаємо потоки для кожної частини
        for (int i = 0; i < k; i++)
        {
            int start = i * partitionSize;
            int end = Math.Min((i + 1) * partitionSize, graph.GetLength(0));

            threads[i] = new Thread(() =>
            {
                // Обробляємо поточну частину
                for (int j = start; j < end; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        for (int l = 0; l < n; l++)
                        {
                            if (graph[j, k] != int.MaxValue && graph[k, l] != int.MaxValue &&
                                graph[j, k] + graph[k, l] < graph[j, l])
                            {
                                graph[j, l] = graph[j, k] + graph[k, l];
                            }
                        }
                    }
                }
            });
            threads[i].Start();
        }

        // Чекаємо на завершення всіх потоків
        for (int i = 0; i < k; i++)
        {
            threads[i].Join();
        }
    }
    // public static void FloydParallel(int[,] graph, int n, int k) //-------------- GetPartitions не працює
    // {
    //    
    //     var threads = new Thread[k];
    //
    //     // Розбиваємо матрицю ваг на k частин
    //     var partitions = graph.GetPartitions(k);
    //
    //     // Запускаємо потоки для кожної частини
    //     for (int i = 0; i < k; i++)
    //     {
    //         threads[i] = new Thread(() =>
    //         {
    //             // Обробляємо поточну частину
    //             FloydParallelHelper(partitions[i], n);
    //         });
    //         threads[i].Start();
    //     }
    //
    //     // Чекаємо на завершення всіх потоків
    //     for (int i = 0; i < k; i++)
    //     {
    //         threads[i].Join();
    //     }
    // }

    private static void FloydParallelHelper(int[,] graph, int n)
    {
        // Реалізуємо алгоритм Флойда для поточної частини
        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (graph[i, k] != int.MaxValue && graph[k, j] != int.MaxValue &&
                        graph[i, k] + graph[k, j] < graph[i, j])
                    {
                        graph[i, j] = graph[i, k] + graph[k, j];
                    }
                }
            }
        }
    }





    public static int[,] GenerateRandomGraph(int n)
    {
        Random random = new Random();
        int[,] graph = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    graph[i, j] = 0;
                }
                else
                {
                    if (random.Next(2) == 1) // Встановлюємо деякі ребра як відсутніми з імовірністю 50%.
                    {
                        graph[i, j] = int.MaxValue;
                    }
                    else
                    {
                        int weight = random.Next(1, 10); // Випадкові ваги від 1 до 10
                        graph[i, j] = weight;
                    }
                }
            }
        }

        return graph;
    }


}
