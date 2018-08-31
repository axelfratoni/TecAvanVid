using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Network;
using UnityEngine;

public class GameManagerController : MonoBehaviour {

    private NetworkManager networkManager;
    public int localPort = 11001;

    public void InitializeServer(object[] messageContent)
    {
        int targetPort = (int)messageContent[0];
        Debug.Log("Server");
        try
        {
            networkManager = new NetworkManager(targetPort);
            GameObject.Find("Menu").SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log("Connection error");
        }
    }

    public void InitializeClient(object[] messageContent)
    {
        string targetIP = (string) messageContent[0];
        int targetPort = (int) messageContent[1];
        Debug.Log("Client");
        try
        {
            networkManager= new NetworkManager(localPort, IPAddress.Parse(targetIP), targetPort);
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
        //TODO
        //networkManager.Disable();
    }
}
