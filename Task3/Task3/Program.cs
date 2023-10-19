﻿using System;
using System.Collections.Generic;
 using System.Diagnostics;
 using System.Threading;


class Program
{
    static void Main()
    {
        Console.Write("Input size of Linear Equations: ");
        int n = int.Parse(Console.ReadLine()!); // 10000
        double eps = 0.001;
        var generatedEquations = GenerateSystemOfLinearEquation(n);
        int[] amountOfThreads = new int[]{2,3,4,5,8,16};
        double[] XbyOrdered = new double[n];
        
        //Ordered method
        Console.WriteLine("Ordered method : \n");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        JacobiOrdered(n, generatedEquations.Matrix, generatedEquations.FreeElements, ref XbyOrdered, eps);
        stopwatch.Stop();
        long timeOrderedMilisec = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Time for Ordered method : {timeOrderedMilisec} millisecond \n");
        
        //Parallel method
        foreach (var threads in amountOfThreads)
        {
            double[] XbyParallel = new double[n];
            Console.WriteLine("Parallel method : \n");
            Console.WriteLine($"Example for {threads} threads\n");
            stopwatch.Reset();
            stopwatch.Start();
            JacobiParallel(n, generatedEquations.Matrix, generatedEquations.FreeElements, ref XbyParallel, eps, threads);
            stopwatch.Stop();
            long timeOfParallelMillisec = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Time for Parallel method by {threads} threads: {timeOfParallelMillisec} millisecond \n");
            long speedUp = timeOrderedMilisec / timeOfParallelMillisec;
            double efficiency = (double)speedUp / threads;
        
            Console.WriteLine($"SpeedUp of method is : {speedUp}");
            Console.WriteLine($"Efficiency of method is : {efficiency}"); // better if = 1;
            // bool isParallelSolutionCorrect = EqualResult(XbyOrdered, XbyParallel);
            // Console.WriteLine($"Parallel solution is correct: {isParallelSolutionCorrect}");
            // if (isParallelSolutionCorrect)
            // {
            //     PrintMatrix(generatedEquations.Matrix);
            //     Console.WriteLine('\n');
            //     Console.WriteLine("Ordered method solution:");
            //     PrintArray(XbyOrdered);
            //
            //     Console.WriteLine("Parallel method solution:");
            //     PrintArray(XbyParallel);
            // }
            // else
            // {
            //     Console.WriteLine("Parallel method solution is incorrect.");
            // }
        }
    }
    // Matrix= A, x=[], b= coefficients
    // A = Diagonal matrix (D) + other elements matrix(R)
    // Dx = b - Rx
    //Iterations x(k+1) = D^-1*(b-Rx(k))
    //or xi(k+1) = 1/a(ii) * (bi - Sum(a(ii)*xj(k))), i = 1...n;
    public static void JacobiOrdered(int size, double[,] coefficients, double[] values, ref double[] X, double eps)
    {
        double[] previousX = new double[size];
        double err;

        do
        {
            err = 0;
            double[] newValues = new double[size];

            for (int i = 0; i < size; i++)
            {
                newValues[i] = values[i];

                for (int j = 0; j < size; j++)
                {

                    if (i != j)
                    {
                        newValues[i] -= coefficients[i,j] * previousX[j];
                    }

                }
                //update x[i] and calculate error
                //The method converges when the matrix A has a dominant main diagonal:
                newValues[i] = newValues[i] / coefficients[i,i];

                if (Math.Abs(previousX[i] - newValues[i]) > err)
                {
                    err = Math.Abs(previousX[i] - newValues[i]);
                }
            }
            previousX = newValues;
        } while (err > eps); // 

        X = previousX;
    }

    public static void JacobiParallel(int size, double[,] coefficients, double[] values, ref double[] X, double eps, int threadNum)
    {
        double[] previousX = new double[size];
        double err;

        Thread[] threads = new Thread[threadNum];

        int[,] parameters = new int[threadNum, 2];


        int remainder = size % threadNum;
        int remainderPassed = 0;

        for (int q = 0; q < threadNum; q++)
        {
            int from = size / threadNum * q + remainderPassed;
            int to = size / threadNum * (q + 1) + remainderPassed;

            if (remainder > 0)
            {
                remainder--;
                remainderPassed++;
                to++;
            }
            parameters[q, 0] = from;
            parameters[q, 1] = to;
        }

        do
        {
            err = 0;
            double[] newValues = new double[size];

            for (int q = 0; q < threadNum; q++)
            {
                int toPass = q;
                threads[q] = new Thread(v =>
                {
                    for (int i = parameters[toPass, 0]; i < parameters[toPass, 1]; i++)
                    {
                        newValues[i] = values[i];

                        for (int j = 0; j < size; j++)
                        {

                            if (i != j)
                            {
                                newValues[i] -= coefficients[i,j] * previousX[j];
                            }

                        }

                        newValues[i] = newValues[i] / coefficients[i,i];

                        if (Math.Abs(previousX[i] - newValues[i]) > err)
                        {
                            err = Math.Abs(previousX[i] - newValues[i]);
                        }
                    }
                });
            }

            foreach (var item in threads)
            {
                item.Start();
            }

            foreach (var item in threads)
            {
                item.Join();
            }

            previousX = newValues;

        } while (err > eps);

        X = previousX;
    }

    public static SystemOfLinearEquation GenerateSystemOfLinearEquation(int vars)
    {
        SystemOfLinearEquation toReturn = new SystemOfLinearEquation();

        var solutions = new double[vars];
        var freeElements = new double[vars];

        double[,] matrix = new double[vars, vars];

        Random random = new Random();

        for (int i = 0; i < vars; i++)
            solutions[i] = random.Next(1, 100);

        for (int i = 0; i < vars; i++)
        {
            for (int j = 0; j < vars; j++)
            {
                if (j == i)
                    matrix[i, j] = random.Next(100 * vars, 200 * vars);
                else
                    matrix[i, j] = random.Next(1, 100);
                freeElements[i] += matrix[i, j] * solutions[j];
            }
        }

        toReturn.Matrix = matrix;
        toReturn.FreeElements = freeElements;
        toReturn.Solutions = solutions;
        return toReturn;
    }
    public class SystemOfLinearEquation
    {
        public double[,] Matrix { get; set; }
        public double[] FreeElements { get; set; }
        public double[] Solutions { get; set; }
    }
    public static bool EqualResult(double[] first, double[] second)
    {
        if (first.Length != second.Length)
            return false;
        for (int i = 0; i < first.Length; i++)
        {
            if (Math.Round(first[i], 2) != Math.Round(second[i], 2))
                return false;
        }
        return true;
    }
    public static void PrintMatrix(double[,] matrix)
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
    public static void PrintArray<T>(T[] arr)
    {
        Console.Write("[");
        Console.Write(String.Join(", ", arr));
        Console.WriteLine("]");
    }
}




