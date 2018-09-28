using Events.Actions;
using UnityEngine;

namespace Events
{
    public class WorldManager
    {
        public void ProcessInput(double time, InputEnum input)
        {
            Debug.Log("Received input: " + input);
        }

        public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
        {
        }

        public void ProcessColorAction(int r, int g, int b)
        {
            Debug.Log("Received color: r " + r + " g " + g + " b " + b);
        }
    }
}