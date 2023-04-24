using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;

    WaitForSeconds waitSec = new WaitForSeconds(0.25f);

    private void Start()
    {
        StartCoroutine(IESendPacket());
        _network = FindObjectOfType<NetworkManager>();
    }

    private IEnumerator IESendPacket()
    {
        while (true)
        {
            yield return waitSec;

            C_Move movePacket = new C_Move();
            movePacket.posX = Random.Range(-50, 50);
            movePacket.posY = 0;
            movePacket.posZ = Random.Range(-50, 50);

            _network.Send(movePacket.Write());
        }
    }
}
