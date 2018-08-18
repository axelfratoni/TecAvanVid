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
		private float time;

		public Move(Vector3 position, Quaternion rotation, float time)
		{
			this.position = position;
			this.rotation = rotation;
			this.time = time;
		}

		public Vector3 getPosition()
		{
			return position;
		}

		public Quaternion getRotation()
		{
			return rotation;
		}
		
		public float getTime()
		{
			return time;
		}
	}

	// Use this for initialization
	void Start () {
	}

	private List<Move> moveList = new List<Move>(5);
	private float lastTime = 0;
	private static float windowTime = 0.1f;

	// Update is called once per frame
	void Update () {
		
		//Clean Queue
		while (qqueue.Count > 0)
		{
			Move m = qqueue.Dequeue();
			if (moveList.Count == moveList.Capacity)
			{
				moveList.RemoveAt(0);
			}
			moveList.Add(m);
		}
		//Ignore while there is no moves
		if (moveList.Count > 3)
		{
			//Init
			if (lastTime == 0)
			{
				Move m = moveList[0];
				transform.position = m.getPosition();
				transform.rotation = m.getRotation();
				lastTime = m.getTime();
				//Debug.Log(lastTime);
			}
			else
			{
				//Debug.Log(Time.deltaTime);
				float time = lastTime + Time.deltaTime;
				Move m;
				Move lastMove;
				int i = 1;
				//Get next saved move
				do
				{
					m = moveList[i];
					lastMove = moveList[i-1];
					i++;
					//Debug.Log("get"+ m.getTime() + " time" + time + "   " + moveList.Count);
				} while (m.getTime() < time && i < moveList.Count);

				Vector3 temp = (m.getPosition() - lastMove.getPosition()) * (time - lastMove.getTime())/ (m.getTime() - lastMove.getTime()) ;
				Debug.Log("TEM" + temp +"TEM" + m + "Last" + lastMove.getPosition());
				Debug.Log("Last" + Time.deltaTime);
				transform.position = lastMove.getPosition() + temp;
				lastTime += Time.deltaTime;
			}
		}

		
	}

	private void Awake()
	{
		if (!created) {
			created = true;
			DontDestroyOnLoad(this.gameObject);
			listener = new UdpClient(11000);
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
					float x = bitBuffer.readFloat(-31.0f,31.0f,0.1f);
					float y = bitBuffer.readFloat(-1.0f,15.0f,0.1f);
					float z = bitBuffer.readFloat(-31.0f,31.0f,0.1f);
					float qx = bitBuffer.readFloat(-1.0f,1.0f,0.01f);
					float qy = bitBuffer.readFloat(-1.0f,1.0f,0.01f);
					float qz = bitBuffer.readFloat(-1.0f,1.0f,0.01f);
					float qw = bitBuffer.readFloat(-1.0f,1.0f,0.01f);
					float time = bitBuffer.readFloat(0.0f,3600.0f,0.01f);
					//Debug.Log("Ser Pos: " + x + " " + y + " " + z + " ");
					//Debug.Log("Ser Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
					qqueue.Enqueue(new Move(new Vector3(x,y,z),new Quaternion(qx,qy,qz,qw),time));

					/*long b = bitBuffer.readInt(0,255);
					long c = bitBuffer.readInt(0,255);
					long d = bitBuffer.readInt(0,255);
					long e = bitBuffer.readInt(0,255);
					long f = bitBuffer.readInt(0,255);*/
					
					
					string receivedString = Encoding.ASCII.GetString(receivedBytes);
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


	private void OnDestroy()
	{
		//listenThread.Abort();
	}

	private void OnDisable()
	{
		listenThread.Abort();
		listener.Close();
	}
}