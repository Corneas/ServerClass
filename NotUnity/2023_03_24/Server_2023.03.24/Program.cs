﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server_2023._03._24
{
    class Program
    {

        // [][][][][] [][][][][] [][][][][] [][][][][] [][][][][]
        static void Main(string[] args)
        {

            int[,] arr = new int[10000, 10000];

            long now = DateTime.Now.Ticks;
            for (int y = 0; y < 10000; y++)
            {
                for (int x = 0; x < 10000; x++)
                {
                    arr[y, x] = 1;
                }
            }
            long end = DateTime.Now.Ticks;
            Console.WriteLine(end - now);


            now = DateTime.Now.Ticks;
            for (int y = 0; y < 10000; y++)
            {
                for (int x = 0; x < 10000; x++)
                {
                    arr[x, y] = 1;
                }
            }
            end = DateTime.Now.Ticks;
            Console.WriteLine(end - now);


        }
    }


}
