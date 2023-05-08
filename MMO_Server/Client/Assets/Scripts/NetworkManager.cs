using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	// 서버 세션 생성
	ServerSession _session = new ServerSession();
	private WaitForSeconds waitSec3f = new WaitForSeconds(3f);

    void Start()
    {
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		// 커넥터를 이용하여 서버 연동, 서버 세션 생성
		Connector connector = new Connector();

		// 세션 반환, 클라의 개수는 1개
		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

    void Update()
    {
		//IPacket packet = PacketQueue.Instance.Pop();

		// 패킷 리스트에 있는 내용 가져옴
		List<IPacket> list = PacketQueue.Instance.PopAll();
		foreach(IPacket packet in list)
			PacketManager.Instance.HandlePacket(_session, packet);
			
		//if(packet != null)
		//{
		//	// packet handler 시작
		//	PacketManager.Instance.HandlePacket(_session, packet);
		//}
    }

	public void Send(ArraySegment<byte> sendBuff)
    {
		_session.Send(sendBuff);
    }


}
