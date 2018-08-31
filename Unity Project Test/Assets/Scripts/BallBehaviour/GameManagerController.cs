using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerController : MonoBehaviour {

    private UDPChannel udpChannel;
    public int localPort = 11001;

	public void Initialize(object[] messageContent)
    {
        string targetIP = (string) messageContent[0];
        int targetPort = (int) messageContent[1];
        try
        {
            udpChannel = new UDPChannel(targetIP, targetPort, localPort, ReceiveAction);
            GameObject.Find("Menu").SetActive(false);
        }
        catch(Exception e)
        {
            Debug.Log("Connection error");
        }
    }

    private void ReceiveAction(UDPChannel udpChannel, Byte[] receivedBytes)
    {
        Debug.Log("Received.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDisable()
    {
        udpChannel.Disable();
    }
}
