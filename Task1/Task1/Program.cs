using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Console = System.Console;

public static class Program
{
    public static int[,] FillMatrix(int n, int m, int AddElem)
    {
        int[,] matrix = new int[n,m];
        int element = 1;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrix[i,j] = element;
                if (AddElem <= 0)
                {
                    AddElem = 1;
                }
                element += AddElem;
            }
        }
    
        return matrix;
    }
    public static void PrintMatrix(int[,] matrix)
    {
        Console.Write("-----Matrix-------\n");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i,j]} \t");
            }
            Console.Write("\n");
        }
        Console.Write("---------------------\n");
    }

    public static int[,] AddMatrixFromTo(int[,] matrixA, int[,] matrixB, int[,] changedMatrix,int fromRow, int fromColumn, int toRow, int toColumn)
    {
        for (int i = fromRow; i < toRow; i++)
        {
            for (int j = fromColumn; j < toColumn; j++)
            {
                changedMatrix[i, j] = matrixA[i,j] + matrixB[i,j];
            }
        }

        return changedMatrix;
    }
    public static int[,] AddMatrix(int[,] matrixA, int[,] matrixB, int n, int m, int numberOfThreads)
    {
        int[,] resultMatrix = new int[n, m];
        Thread[] threads = new Thread[numberOfThreads];

        int rowsPerThread = n / numberOfThreads;
        int remainingRows = n % numberOfThreads; // checking is number of rows can be divided between threads 

        int currentRow = 0;

        for (int i = 0; i < numberOfThreads; i++)
        {
            int startRow = currentRow;
            int endRow = currentRow + rowsPerThread; // if amount of rows can't be divided by amount of threads,
                                                     // add one more row to current thread

            if (i < remainingRows)
            {
                endRow++;
            }

            threads[i] = new Thread(() =>
            {
                AddMatrixFromTo(matrixA, matrixB, resultMatrix, startRow, 0, endRow, m);
            });

            threads[i].Start();

            currentRow = endRow;
        }

        // Wait for all threads to finish
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }
    
    public static void Menu()
    {
        int n;
        int m;
        int k;
        Console.WriteLine("Enter value of matrix n*m:");
        Console.Write("n: ");
        n = int.Parse(Console.ReadLine()!); // rows height
        Console.Write("m: ");
        m = int.Parse(Console.ReadLine()!); // columns width
        Console.WriteLine("Enter number k - number of threads");
        Console.Write("k: ");
        k = int.Parse(Console.ReadLine()!); // threads
        Console.WriteLine($"Creating matrix {n} by {m}");
        //int [,] matrixA = FillMatrix(n, m, 2);
        //int [,] matrixB = FillMatrix(n, m, 1);
         int[,] matrixA = new int[n, m];
         int[,] matrixB = new int[n, m];
         Random random = new();
        
         for (int i = 0; i < n; i++)
         {
             for (int j = 0; j < m; j++)
             {
                 matrixA[i, j] = random.Next(0, 1000);
                 matrixB[i, j] = random.Next(0, 1000);
             }
         }
        
        // PrintMatrix(matrixA);
        // PrintMatrix(matrixB);
        // Calculation time for ordered method ( non-parallel)
        int[,] orderMatrix = new int[n, m];
        Stopwatch stopwatchFirst = new Stopwatch();
        stopwatchFirst.Start();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                orderMatrix[i, j] = matrixA[i, j] + matrixB[i, j];
            }
        }
        stopwatchFirst.Stop();
        long elapsedMillisecondsFirst = stopwatchFirst.ElapsedMilliseconds; 
        // Console.WriteLine("Result of appending of two matrix : \n");
        // PrintMatrix(orderMatrix);
        Console.WriteLine($"Time taken to append two matrix by non-parallel method: {elapsedMillisecondsFirst}... milliseconds");
        // // Calculation time for parallel method
        int[,] parellelMatrix = new int[n, m];
        if (k > 1)
        {
            Stopwatch stopwatchSecond = new Stopwatch();
            stopwatchSecond.Start();
            parellelMatrix = AddMatrix(matrixA, matrixB, n, m, k);
            stopwatchSecond.Stop();
            long elapsedMillisecondsSecond = stopwatchSecond.ElapsedMilliseconds; 
            // Console.WriteLine("Result of appending of two matrix : \n");
            // PrintMatrix(parellelMatrix);
            Console.WriteLine($"Time taken to append two matrix by parallel method: {elapsedMillisecondsSecond}... milliseconds");
            long speedUp = elapsedMillisecondsFirst / elapsedMillisecondsSecond;
            double efficiency = (double)speedUp / k;
            
            Console.WriteLine($"SpeedUp of method is : {speedUp}");
            Console.WriteLine($"Efficiency of method is : {efficiency}"); // better if = 1;
        }
       
       
    }
    public static void Main(string[] args)
    {
        Menu();
    }
}