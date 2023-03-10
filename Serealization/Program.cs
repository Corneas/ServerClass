using System;
using System.Net;

namespace Serealization
{
    class Program
    {
        static void Main(string[] args)
        {
            int num = 1234;

            // * 리틀엔디안 -> 12 34 56 78 .. 처럼 순서대로 정렬
            // * 빅엔디안 -> 78 56 34 12 .. 처럼 역순으로 정렬

            // 직렬화
            byte[] buffer = BitConverter.GetBytes(num);
            Console.WriteLine(buffer.Length);

            // 직렬화된 데이터 살펴보기
            string hex = BitConverter.ToString(buffer);
            Console.WriteLine(hex);

            // 리틀엔디안 -> 빅 엔디안 변환
            int num2 = IPAddress.HostToNetworkOrder(1234);
            byte[] buffer2 = BitConverter.GetBytes(num2);
            Console.WriteLine(BitConverter.ToString(buffer2));

            // 빅엔디안 -> 리틀엔디안 역직렬화
            int num3 = BitConverter.ToInt32(buffer2);
            int num4 = IPAddress.NetworkToHostOrder(num3);
            Console.WriteLine(num4);
        }
    }
}
