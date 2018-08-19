using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
 
 
	void OnMouseDown() {
		Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(mouse);
	}
 
 
	void OnMouseDrag() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		curPosition.y = 2f;
		transform.rotation = new Quaternion(curPosition.x,curPosition.y,curPosition.z,curPosition.x);
	}
	
	void Update(){
		Plane plane=new Plane(Vector3.up,new Vector3(0, 2, 0));
		var w = Input.mousePosition;
		var ww = Camera.main;
		Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		if(plane.Raycast(ray, out distance)) {
			transform.position=ray.GetPoint(distance);
		}
	}
}
