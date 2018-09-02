using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Network;
using Network.Events;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Random = System.Random;

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
	        networkManager.AddEvent(new CreationEvent(0,0));
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

	    if (networkManager.IsServer)
	    {
		    networkManager.UpdateSeq();
		    networkManager.Loop();
	    }
		else
	    {
		    networkManager.UpdateSeq();
			networkManager.AddEvent(new SnapshotEvent(new Vector3(0f,0f,0f),0,0));
			if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.A))
			{
				Random random = new Random();
				float red = (float) random.NextDouble();
				float green = (float) random.NextDouble();
				float blue = (float) random.NextDouble();
				Color color = new Color(red, green, blue, 1.0f);
				networkManager.AddEvent(new ColorEvent(red, green, blue, 1.0f, 0, 0));
			}
			networkManager.Loop();
		}
	}

    private void OnDisable()
    {
        networkManager.Disable();
    }
}
