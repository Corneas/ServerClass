using System;
using System.Threading;
using System.Threading.Tasks;

namespace TLS
{
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
