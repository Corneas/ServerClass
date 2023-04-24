using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest2
{
    class Program
    {

        #region 1교시
        /* static void MainThread(object state)
        {
            Console.WriteLine("Start MainThread");

            while (_stop == false)
            {

            }

        }
        static void Main(string[] args)
        {
            string s;

            ThreadPool.SetMaxThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 4; ++i)
            {
                // ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });
                Task t = new Task(() => {while (true) { } }, TaskCreationOptions.LongRunning);
                t.Start();
            }

            ThreadPool.QueueUserWorkItem(MainThread);

            while (true)
            {
                s = Console.ReadLine();
                if(s == "exit" || s == "Exit")
                {
                    Console.WriteLine("Exit this loop");
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        */
        #endregion

        volatile static bool _stop = false;

        static void MainThread()
        {
            Console.WriteLine("Start MainThread");

            while(_stop == false)
            {

            }

        }

        static void Main(string[] args)
        {


            #region 2교시

            Task t = new Task(MainThread);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("stop 호출");
            Console.WriteLine("종료 대기중");
            t.Wait();
            Console.WriteLine("종료 성공");


            #endregion

        }
    }
}
