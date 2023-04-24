using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        //sealed : 상속받은 다른 클래스가 OnRecv를 오버라이드 못하도록 봉인
        public static readonly int HeaderSize = 2;
        public override sealed int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            while (true)
            {
                // 최소한 헤더(사이즈)는 받을 수 있는지 확인
                if (buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                // 부분적 도착
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                processLen += dataSize;

                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;

        }
        // 상속받은 클래스들은 OnPacketRecv()로 OnRecv를 우회하여 사용
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0; // 인터락 플래그 변수

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        bool _pending = false;
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        object _lock = new object();

        public void Init(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed -= new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); // (2-2) 낚싯대 들기
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); // (2-2) 낚싯대 들기
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed -= new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            // RegisterSend();

            _recvArgs.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecv(); // (1) 낚싯대 던지기
        }
        #region 데이터 수신
        // 1. 연결대기
        void RegisterRecv()
        {
            // 커서 뒤로 이동 방지
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if(pending == false)
            {
                OnRecvCompleted(null, _recvArgs);    // (2-1) 낚싯대 들어올리기(데이터 수신 발생)
            }
        }
        // 2. 데이터 수신

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // TODO
                try
                {
                    // Write 커서 이동
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read 커서 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    RegisterRecv(); // (3) 낚싯대 다시 던지기 (이벤트 재호출)
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnReceiveComplted Failed! {e}");
                }
            }
            else
            {
                // TODO Disconnect
                Disconnect();
            }
        }
        #endregion

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        void RegisterSend()
        {
            _pending = true;

            // buff를 한 개씩 전송
            //byte[] buff = sendQueue.Dequeue();
            ////_sendArgs.SetBuffer(buff, 0, buff.Length);

            //bool pending = _socket.SendAsync(_sendArgs);
            //if(pending == false)
            //{
            //    OnSendCompleted(null, _sendArgs);
            //}

            // buff들을 리스트로 묶어 한번에 전송
            List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();

            while(sendQueue.Count  > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;
        }

        void OnSendCompleted(object send, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    // TODO
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
                        //else
                        //    _pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnReceiveCompleted Failed! {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) // 연결종료 발생시 플래그변수 1로 변경
                return;                                         // 만약 이미 1로 변경되어있다면, return 통해 아무동작하지않음
            //쫓아낸다.
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
