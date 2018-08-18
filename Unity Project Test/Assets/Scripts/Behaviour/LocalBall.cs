using Libs;
using UnityEngine;

public class LocalBall : MonoBehaviour {

    public new Rigidbody rigidbody;

    private UDPClient udpClient;
    private BitBuffer bitBuffer;
    private float SPEED = 10;
    private static float CYCLE_TIME = 0.1f;
    private float ctime;
    private float time;

    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        udpClient = new UDPClient("127.0.0.1", 11000);
        bitBuffer = new BitBuffer(1024);
        ctime = Time.deltaTime;
        time = Time.deltaTime;
    }
	
	void Update () {
        ctime += Time.deltaTime;
        time += Time.deltaTime;
        if (ctime > CYCLE_TIME)
        {
            SendPosition(); 
            ctime -= CYCLE_TIME;
        }
    }

    private void SendPosition()
    {
        bitBuffer.writeFloat(transform.position.x, -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(transform.position.y, 0.0f, 3.0f, 0.1f);
        bitBuffer.writeFloat(transform.position.z, -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(time, 0.0f, 3600.0f, 0.01f);
        bitBuffer.flush();
        udpClient.SendMessage(bitBuffer.getBuffer());
    }

    private void FixedUpdate()
    {
        float verticalForce = Input.GetAxis("Vertical") * SPEED;
        float horizontalForce = Input.GetAxis("Horizontal") * SPEED;
        rigidbody.AddForce(new Vector3(horizontalForce, 0, verticalForce));
    }

    private void OnDisable()
    {
        udpClient.Disable();
    }
}
