﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContextSwitch
{
    class Lock
    {
        // Initial State인 true -> 문이 열린 상태 , false -> 문이 닫힌 상태
        AutoResetEvent _available = new AutoResetEvent(true);

        public void Acquire()
        {
            _available.WaitOne(); // 문이 열리면 입장이 가능하다.
                                  // autoResetEvent는 입장이 완료되면 자동으로 문이 닫힌다.

        }

        public void Release()
        {
            _available.Set(); // 이벤트의 상태를 시그널로 바꾼다. -> initial state를 true로 바꾼다. 
        }
    }

    class Program
    {
        static int number = 0;
        static Lock _lock = new Lock();

        static void Thread_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();

                number++; // 
                //Console.WriteLine($"Plus Number {i}");

                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();

                number--;
                //Console.WriteLine($"Minus Number {i}");

                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}
