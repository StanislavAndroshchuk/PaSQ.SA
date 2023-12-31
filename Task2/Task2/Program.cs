﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Console = System.Console;

public static class Program
{
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

    // public static int[,] AddMatrixFromTo(int[,] matrixA, int[,] matrixB, int[,] changedMatrix,int fromRow, int fromColumn, int toRow, int toColumn)
    // {
    //     for (int i = fromRow; i < toRow; i++)
    //     {
    //         for (int j = fromColumn; j < toColumn; j++)
    //         {
    //             changedMatrix[i, j] = matrixA[i,j] + matrixB[i,j];
    //         }
    //     }
    //
    //     return changedMatrix;
    // }
    // public static int[,] AddMatrix(int[,] matrixA, int[,] matrixB, int n, int m, int numberOfThreads)
    // {
    //     int[,] resultMatrix = new int[n, m];
    //     Thread[] threads = new Thread[numberOfThreads];
    //
    //     int rowsPerThread = n / numberOfThreads;
    //     int remainingRows = n % numberOfThreads; // checking is number of rows can be divided between threads 
    //
    //     int currentRow = 0;
    //
    //     for (int i = 0; i < numberOfThreads; i++)
    //     {
    //         int startRow = currentRow;
    //         int endRow = currentRow + rowsPerThread; // if amount of rows can't be divided by amount of threads,
    //                                                  // add one more row to current thread
    //
    //         if (i < remainingRows)
    //         {
    //             endRow++;
    //         }
    //
    //         threads[i] = new Thread(() =>
    //         {
    //             AddMatrixFromTo(matrixA, matrixB, resultMatrix, startRow, 0, endRow, m);
    //         });
    //
    //         threads[i].Start();
    //
    //         currentRow = endRow;
    //     }
    //
    //     // Wait for all threads to finish
    //     foreach (Thread thread in threads)
    //     {
    //         thread.Join();
    //     }
    //
    //     return resultMatrix;
    // }

    public static int[,] MultiplyMatrix(int[,] matrixA, int[,] matrixB, int n, int a, int b)
    {
        int[,] resultMatrix = new int[n, b];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < b; j++)
            {
                int sum = 0;
                for (int k = 0; k < a; k++)
                {
                    sum += matrixA[i, k] * matrixB[k, j];
                }

                resultMatrix[i, j] = sum;
            }
        }

        return resultMatrix;
    }
    public static int[,] MultiplyMatrixParallel(int[,] matrixA, int[,] matrixB, int n, int a, int b, int numberOfThreads)
    {
        int[,] resultMatrix = new int[n, b];
        Thread[] threads = new Thread[numberOfThreads];

        int rowsPerThread = n / numberOfThreads;
        int remainingRows = n % numberOfThreads;

        int currentRow = 0;

        for (int i = 0; i < numberOfThreads; i++)
        {
            int startRow = currentRow;
            int endRow = currentRow + rowsPerThread;

            if (i < remainingRows)
            {
                endRow++;
            }

            threads[i] = new Thread(() =>
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int col = 0; col < b; col++)
                    {
                        int sum = 0;
                        for (int k = 0; k < a; k++)
                        {
                            sum += matrixA[row, k] * matrixB[k, col];
                        }
                        resultMatrix[row, col] = sum;
                    }
                }
            });

            threads[i].Start();

            currentRow = endRow;
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    public static void Menu()
    {
        int n, m, a, b;  
        int k;
        Console.WriteLine("Enter value of matrixA n*m:");
        Console.Write("n: ");
        n = int.Parse(Console.ReadLine()!); // rows height
        Console.Write("m: ");
        m = int.Parse(Console.ReadLine()!); // columns width
        
        
        
        Console.WriteLine("Enter value of matrixA a*b:");
        Console.Write("a: ");
        a = int.Parse(Console.ReadLine()!); // rows height
        Console.Write("b: ");
        b = int.Parse(Console.ReadLine()!); // columns width
        
        if (m != a)
        {
            Console.WriteLine("Impossible to multiply matrices, because the number of columns of matrix A is not equal " +
                              "to the number of rows of matrix B.");
            return;
        }
        
        
        
        Console.WriteLine("Enter number k - number of threads");
        Console.Write("k: ");
        k = int.Parse(Console.ReadLine()!); // threads
        
        
        
        Console.WriteLine($"Creating matrix 1 and 2:  {n} by {m}, {a} by {b}");
        int[,] matrixA = new int[n, m];
        int[,] matrixB = new int[a, b];
        Random random = new Random();
        
        Console.WriteLine("Matrix A:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrixA[i, j] = random.Next(0, 10);
            }
        }
        Console.WriteLine("Matrix B:");
        for (int i = 0; i < a; i++)
        {
            for (int j = 0; j < b; j++)
            {
                matrixB[i, j] = random.Next(0, 10);
            }
        }
        
        //PrintMatrix(matrixA);
        //PrintMatrix(matrixB);
        
        Stopwatch stopwatchFirst = new Stopwatch();
        stopwatchFirst.Start();
        int[,] orderMatrix = MultiplyMatrix(matrixA, matrixB, n, a, b);
        stopwatchFirst.Stop();
        Console.WriteLine("Result of multiplying of two matrix:\n");
        //PrintMatrix(orderMatrix);
        long elapsedMillisecondsFirst = stopwatchFirst.ElapsedMilliseconds; 
        Console.WriteLine($"Time taken to multiply two matrix by non-parallel method: {elapsedMillisecondsFirst}... milliseconds");
        
        
        
        
        
        Stopwatch stopwatchSecond = new Stopwatch();
        stopwatchSecond.Start();
        int[,] parallelMatrix = MultiplyMatrixParallel(matrixA, matrixB, n, a, b, k);
        stopwatchSecond.Stop();
        Console.WriteLine("Result of multiplying of two matrix:\n");
        //PrintMatrix(parallelMatrix);
        double elapsedMillisecondsSecond = stopwatchSecond.ElapsedMilliseconds; 
        Console.WriteLine($"Time taken to multiply two matrix by parallel method: {elapsedMillisecondsSecond}... milliseconds");

        double speedUp = elapsedMillisecondsFirst / elapsedMillisecondsSecond;
        double efficiency = (double)speedUp / k;
        
        Console.WriteLine($"SpeedUp of method is : {speedUp}");
        Console.WriteLine($"Efficiency of method is : {efficiency}"); // better if = 1;
        
       
       
    }
    public static void Main(string[] args)
    {
        Menu();
    }
}