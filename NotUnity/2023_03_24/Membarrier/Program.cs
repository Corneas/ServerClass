﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Membarrier
{
    internal class Program
    {
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread1()
        {
            y = 1; // Store y

            Thread.MemoryBarrier();

            r1 = x; // Load x
        }

        static void Thread2()
        {
            x = 1; // Store x

            Thread.MemoryBarrier();

            r2 = y; // Load y
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                x = y = r1 = r2 = 0; // 반복문의 초기부분마다 모든 변수의 값을 0으로 할당한다.
                count++;

                Task t1 = new Task(Thread1);
                Task t2 = new Task(Thread2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 & r2 == 0)
                    break;
            }
            Console.WriteLine($"{count}번 만에 빠져 나옴!");
        }
    }
}
