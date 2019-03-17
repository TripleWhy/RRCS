using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class ConnectionTransitionUi : ConnectionUi
    {
        public PortUi enabledPort;

        public LineRenderer lineRenderer
        {
            get { return GetComponent<LineRenderer>(); }
        }
        
        public override void UpdatePositions()
        {
            Vector2 diff = (targetPosition - startPosition) ;
            Vector2 offsetVec = (diff.normalized * 15);
            Vector2 orthogonalOffset = new Vector2(offsetVec.y, -offsetVec.x);
            Vector2 portPosition = startPosition + (diff * 0.5f) + orthogonalOffset;
            
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, portPosition);
            lineRenderer.SetPosition(2, targetPosition);

            if (enabledPort)
            {
                enabledPort.transform.position =
                    new Vector3(portPosition.x, portPosition.y, enabledPort.transform.position.z);
            }
        }
        
        public override Connection Connection
        {
            get { return connection; }
            set
            {
                if (connection != null)
                {
                    if (object.ReferenceEquals(connection, value))
                        return;
                    throw new InvalidOperationException();
                }

                if (enabledPort != null)
                {
                    enabledPort.Port = ((StateMachineTransition) value).transitionEnabledPort;
                }
                
                connection = value;
                
                UiManager.Register(this);
            }
        }

        public override void Disconnect()
        {
            if (enabledPort != null && enabledPort.Port != null)
                enabledPort.Port.DisconnectAll();
            base.Disconnect();
        }
    }
}