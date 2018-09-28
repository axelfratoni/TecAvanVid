using Network;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private readonly Ball _ball;
    private Rigidbody _rigidBody;

    private readonly float SPEED = 10;

    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float verticalForce = Input.GetAxis("Vertical") * SPEED;
        float horizontalForce = Input.GetAxis("Horizontal") * SPEED;
        _rigidBody.AddForce(new Vector3(horizontalForce, 0, verticalForce));
    }

}