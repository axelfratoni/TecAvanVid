using Libs;
using System;
using System.Collections;
using UnityEngine;

public class NetworkBallEric : MonoBehaviour {


    void Start () {
    }
	
    void Update () {
        UpdatePositionAxel();
        //UpdatePositionEric();
    }

    private void UpdatePositionEric()
    {
        NetworkBuffer<Vector3>.InterpolatedItem<Vector3> interpolatedItem = networkBuffer.GetNextItem();
        if (interpolatedItem != null)
        {
            transform.position = Vector3.Lerp(interpolatedItem.previous, interpolatedItem.next, interpolatedItem.interpolation);
        }
    }

    private void UpdatePositionAxel()
    {
        NetworkBuffer2<Vector3>.InterpolatedItem<Vector3> interpolatedItem = networkBuffer2.GetNextItem();
        if (interpolatedItem != null)
        {
            transform.position = Vector3.Lerp(interpolatedItem.previous, interpolatedItem.next, interpolatedItem.interpolation);
        }
    }

}
