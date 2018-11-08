using Events.Actions;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class ObjectController : MonoBehaviour
    {
        public ObjectEnum ObjectType { get; protected set; }
        public int ClientId { get; protected set; }
        public int ObjectId { get; protected set; }        
        public float LastClientInputTime { get; protected set; }

    }
    
}