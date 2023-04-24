using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpinLock
{
    class SpinLock // Lock이 풀릴 때 까지 돌아가는 존버메타
    {
        #region 안되는 것 (원자성의 문제)
        //volatile bool _locked = false;

        //public void Acquire()
        //{
        //    while (_locked)
        //    {

        //    }

        //    _locked = true;
        //}
        #endregion

        volatile int _locked = 0;

        public void Acquire()
        {
            const int expected = 0;
            const int desired = 1;
            while (true)
            {
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                Thread.Yield(); // 기아현상 발생
                // *기아현상 : 어떠한 우선 순위로 작업을 처리할 때 우선순위가 낮은 작업은 영원히 처리되지 않는 문제
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class Program
    {
        static int number = 0;
        // 스핀락 인스턴스 생성
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();

                number++;

                _lock.Release();
            }
        }

        static void Thread_2()
        {

            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();    // 진입 후 락

                number--;

                _lock.Release();    // 나간 뒤 락 해제
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
