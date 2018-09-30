using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Ball _ball;
    private Rigidbody _rigidBody;

    private readonly float SPEED = (float) 0.05;

    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_ball.Position);
    }

    
    public void ApplyInput(double time, List<InputEnum> inputList) 
    {
        float v = 0; 
        float h = 0; 
        inputList.ForEach(input =>
        {
             v += input.Equals(InputEnum.W) ? 1 : input.Equals(InputEnum.S) ? -1 : 0;
             h += input.Equals(InputEnum.D) ? 1 : input.Equals(InputEnum.A) ? -1 : 0;
        });
        
        Vector3 movement = new Vector3(h, 0, v);
        movement = movement.normalized * SPEED;
        _ball.MovePosition(movement);
    }

    public void ApplySnapshot(double time, Vector3 position)
    {
        _ball.SetPosition(position);
    }

    public Ball GetBall()
    {
        return _ball;
    }
    
    public void SetBall(Ball ball)
    {
        _ball = ball;
    }
}