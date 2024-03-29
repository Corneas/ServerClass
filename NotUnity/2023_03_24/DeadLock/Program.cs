﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeadLock
{
    class SessionManager
    {
        static object _lock_session = new object();

        public static void TestSession()
        {
            lock (_lock_session)
            {

            }
        }

        public static void Test()
        {
            lock (_lock_session)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock_user = new object();

        public static void TestUser()
        {
            lock (_lock_user)
            {

            }
        }

        public static void Test()
        {
            lock (_lock_user)
            {
                SessionManager.TestSession();
            }
        }
    }

    class Program
    {


        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 100; i++)
            {
                SessionManager.Test();

            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100; i++)
            {
                UserManager.Test();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();

            Thread.Sleep(100);

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine("데드락에 걸리지 않았다!");
        }
    }
}
