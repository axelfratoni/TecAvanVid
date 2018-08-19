using Libs;
using UnityEngine;

public class LocalBall : MonoBehaviour {

    public new Rigidbody rigidbody;
    public string targetIP = "127.0.0.1";
    public int targetPort = 11000;

    private UDPChannel udpSender;
    private BitBuffer bitBuffer;
    private readonly float SPEED = 10;
    private readonly static float CYCLE_TIME = 0.1f;
    private float actualCycle;
    private float time;

    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        udpSender = new UDPChannel(targetIP, targetPort);
        bitBuffer = new BitBuffer(1024);
        actualCycle = 0;
        time = 0;
    }
	
	void Update () {
        actualCycle += Time.deltaTime;
        time += Time.deltaTime;
        if (actualCycle > CYCLE_TIME)
        {
            SendPosition(); 
            actualCycle = 0;
        }
    }

    private void SendPosition()
    {
        bitBuffer.writeFloat(transform.position.x, -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(transform.position.y, 0.0f, 3.0f, 0.1f);
        bitBuffer.writeFloat(transform.position.z, -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(time, 0.0f, 3600.0f, 0.01f);
        bitBuffer.flush();
        udpSender.SendMessage(bitBuffer.getBuffer());
    }

    private void FixedUpdate()
    {
        float verticalForce = Input.GetAxis("Vertical") * SPEED;
        float horizontalForce = Input.GetAxis("Horizontal") * SPEED;
        rigidbody.AddForce(new Vector3(horizontalForce, 0, verticalForce));
    }

    private void OnDisable()
    {
        udpSender.Disable();
    }
}
