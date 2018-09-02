using Libs;
using Network;
using UnityEngine;

public class LocalBallEric: BallEric{

    public new Rigidbody rigidbody;

    private readonly float SPEED = 10;
    private float actualCycle;
    private float time;

    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        actualCycle = 0;
        time = 0;
    }

    private void FixedUpdate()
    {
        float verticalForce = Input.GetAxis("Vertical") * SPEED;
        float horizontalForce = Input.GetAxis("Horizontal") * SPEED;
        rigidbody.AddForce(new Vector3(horizontalForce, 0, verticalForce));

        actualCycle += Time.deltaTime;
        time += Time.deltaTime;
    }
    
    

}