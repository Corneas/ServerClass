using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            // 문지기 생성
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory -= sessionFactory;
            _sessionFactory += sessionFactory;    // 2번

            // 문지기 교육
            _listenSocket.Bind(endPoint); // bind : ip주소, port번호 할당

            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); // 이벤트 생성, 3번
            RegisterAccept(args); // 이벤트 등록, 4번 
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            // 보류중인(pending) 작업이 있는지 여부를 bool 값으로 반환
            bool pending = _listenSocket.AcceptAsync(args); // 5번
            if(pending == false)
            {
                OnAcceptCompleted(null, args);  // null은 sender 불필요, args는 소켓 객체
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // Accept()의 블로킹 버전 추가
            if(args.SocketError == SocketError.Success) // 오류가 없을 경우, 6번
            {
                // TODO
                Session session = _sessionFactory.Invoke();
                session.Init(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else // 오류가 발생할 경우
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);   // 연결이 끝난 뒤, 다음 유저 접속을 위해 또다시 Accept 대기 등록
        }
    }
}
