using Events.Actions;
using Network;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Ball _ball;
    private Rigidbody _rigidBody;

    private readonly float SPEED = 10;

    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //transform.position = _ball.Position;
    }

    public void SetBall(Ball ball)
    {
        _ball = ball;
    }
    
    public void Move(double time, InputEnum input)
    {
        /*movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidBody.MovePosition(transform.position + movement);*/
    }

}