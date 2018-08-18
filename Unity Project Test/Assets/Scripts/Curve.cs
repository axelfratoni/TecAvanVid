using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
	private float time;
	private static Vector3 g = new Vector3(0,-9.81f,0); 
	private Vector3 speed; 
	// Use this for initialization
	void Start () {
		transform.position = Vector3.zero;
		time = 0;
		speed = new Vector3(1,10,-1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (time == 0)
			time = Time.deltaTime;
		else
		{
			float dtime = Time.deltaTime;
			transform.position += speed * dtime;
			if (transform.position.y <=0)
			{
				//transform.position = new Vector3(transform.position.x,0,transform.position.z);
				speed *= -1;
			}
			else
			{
				speed += g * dtime;
			}
		}
	}
}
