using System.Collections.Generic;
using Events.Actions;
using Network;

namespace Events
{
    public class ServerManager : BehaviourManager
    {
        private readonly EventManager _eventManager;
        private readonly WorldManager _worldManager;
        
        public ServerManager(EventManager eventManager, WorldManager worldManager)
        {
            _eventManager = eventManager;
            _worldManager = worldManager;
        }
        
        public void Update()
        {
            List<BallController> ballList = _worldManager.GetBallList();
            ballList.ForEach(ball =>
            {
                Ball ballData = ball.GetBall();
                _eventManager.BroadcastEventAction(new SnapshotAction(ballData.ObjectId, ballData.Position, 0));
            });
        }
    }
}