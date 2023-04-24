using System;
using System.Threading;

namespace ThreadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int num = 0;

            object obj = new object();

            Thread t1 = new Thread(() =>
            {
                for (int i = 0; i < 1000000; ++i)
                {
                    // 모니터
                    //try
                    //{
                    //    Monitor.Enter(obj);
                    //    num++;
                    //}
                    //finally
                    //{
                    //    Monitor.Exit(obj);
                    //}

                    // 간단하게 표현
                    lock (obj)
                    {
                        num++;
                    }
                }
            });
            t1.Start();

            Thread t2 = new Thread(() =>
            {
                for (int i = 0; i < 1000000; ++i)
                {
                    try
                    {
                        Monitor.Enter(obj);
                        num++;
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
            });
            t2.Start();

            t1.Join();
            t2.Join();
            Console.WriteLine(num);

        }
    }
}
