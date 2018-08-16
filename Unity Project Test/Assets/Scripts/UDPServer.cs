/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour {  	
	/*#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread udpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient; 	
	#endregion 	
		
	// Use this for initialization
	void Start () { 		
		// Start TcpServer background thread 		
		udpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		udpListenerThread.IsBackground = true; 		
		udpListenerThread.Start(); 	
	}  	
	
	// Update is called once per frame
	void Update () { 		
		if (Input.GetKeyDown(KeyCode.B)) {             
			SendMessage();         
		} 	
	}  	
	
	struct Received
	{
		public IPEndPoint Sender;
		public string Message;
	}

	abstract class UdpBase
	{
		protected UdpClient Client;

		protected UdpBase()
		{
			Client = new UdpClient();
		}

		public async Task<Received> Receive()
		{
			var result = Client.Receive();
				.ReceiveAsync();
			return new Received()
			{
				Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
				Sender = result.RemoteEndPoint
			};
		}
	}
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("192.168.0.19"), 8052); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];  			
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							
							Debug.Log("client message received as: " + clientMessage); 						
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}  	
	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	private void SendMessage() { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 
				string serverMessage = "This is a message from your server."; 			
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 
	
}*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Libs;
using UnityEngine;

public class UDPServer : MonoBehaviour {

	UdpClient listener;
	IPEndPoint anyClient = new IPEndPoint(IPAddress.Any, 0);
	private static bool created = false;
	Thread listenThread;
	Queue<Move> qqueue = new Queue<Move>();

	private class Move
	{
		private Vector3 position;
		private Quaternion rotation;

		public Move(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public Vector3 getPosition()
		{
			return position;
		}

		public Quaternion getRotation()
		{
			return rotation;
		}
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

		while (qqueue.Count > 0)
		{
			Move m = qqueue.Dequeue();
			transform.position = m.getPosition();
			transform.rotation = m.getRotation();
		}
	}

	private void Awake()
	{
		if (!created) {
			created = true;
			DontDestroyOnLoad(this.gameObject);
			listener = new UdpClient(11000);
			Debug.Log("Quaternion: ");
			ListenForConnections();
		}
	}

	private void ListenForConnections()
	{	
		listenThread = new Thread(() =>
		{
			while (true)
			{
				try
				{
					Byte[] receivedBytes = listener.Receive(ref anyClient);
					//////////////parseBytes(receivedBytes);
					/// 
					
					BitBuffer bitBuffer = new BitBuffer(receivedBytes);
					float x = bitBuffer.readFloat(-31.0f,31.0f,1f);
					float y = bitBuffer.readFloat(0.0f,3.0f,1f);
					float z = bitBuffer.readFloat(-31.0f,31.0f,1f);
					float qx = bitBuffer.readFloat(0.0f,1.0f,0.01f);
					float qy = bitBuffer.readFloat(0.0f,1.0f,0.01f);
					float qz = bitBuffer.readFloat(0.0f,1.0f,0.01f);
					float qw = bitBuffer.readFloat(0.0f,1.0f,0.01f);
					Debug.Log("Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
					qqueue.Enqueue(new Move(new Vector3(x,y,z),new Quaternion(qx,qy,qz,qw)));
					
					
					string receivedString = Encoding.ASCII.GetString(receivedBytes);
					Debug.Log(receivedString);
					Byte[] returnString = Encoding.ASCII.GetBytes("UDP Server says: " + receivedString);
					listener.Send(returnString, returnString.Length, anyClient);
				}
				catch (Exception e)
				{
					Debug.Log(e.ToString());
				}
			}
		});
		listenThread.Start();
	}

	private void parseBytes(Byte[] bytes)
	{
	}

	public void setPosition(Vector3 position)
	{
		GameObject.Find("Cubeq").transform.position = position;
	}
	public void setRotation(Quaternion rotation)
	{
		GameObject.Find("Cubeq").transform.rotation = rotation;
	}

	private void OnDestroy()
	{
		//listenThread.Abort();
	}
}