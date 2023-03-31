using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _2023_03_31
{
    internal class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            // 문지기 생성
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler -= onAcceptHandler;
            _onAcceptHandler += onAcceptHandler;

            // 문지기 교육
            _listenSocket.Bind(endPoint); // bind : ip주소, port번호 할당

            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); // 이벤트 생성
            RegisterAccept(args); // 이벤트 등록
        }
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            // 보류중인(pending) 작업이 있는지 여부를 bool 값으로 반환
            bool pending = _listenSocket.AcceptAsync(args);
            if(pending == false)
            {
                OnAcceptCompleted(null, args);  // null은 sender 불필요, ags는 소켓 객체
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // Accept()의 블로킹 버전 추가
            if(args.SocketError == SocketError.Success) // 오류가 없을 경우
            {
                // TODO
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else // 오류가 발생할 경우
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);   // 연결이 끝난 뒤, 다음 유저 접속을 위해 또다시 Accept 대기 등록
        }
    }
}
