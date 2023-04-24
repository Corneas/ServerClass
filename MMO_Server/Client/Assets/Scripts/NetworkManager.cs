using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	ServerSession _session = new ServerSession();
	private WaitForSeconds waitSec3f = new WaitForSeconds(3f);

    void Start()
    {
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

    void Update()
    {
		//IPacket packet = PacketQueue.Instance.Pop();
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
