using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesisExperiment.States;
using System;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.ExperimentSync
{
    /// <summary>
    /// Synchronize the experiment between devices with <see cref="StateMessage"/>.
    /// </summary>
    public class ExperimentSync : DevicesSync
    {
        // Properties

        /// <summary>
        /// Gets the current synchronized state.
        /// </summary>
        public State State { get { return latestStateMessage.state; } set { SendNewState(value); } }

        // Events

        /// <summary>
        /// Called on server and on device client when <see cref="State"/> has been updated by a device.
        /// </summary>
        public event Action<StateMessage> StateUpdated = delegate { };

        /// <summary>
        /// Called on server when a <see cref="ReadyNextStateMessage"/> has been received.
        /// </summary>
        public event Action<ReadyNextStateMessage> ReadyNextStateReceived = delegate { };

        // Variables

        protected StateMessage latestStateMessage = new StateMessage();
        protected StateMessage sendStateMessage = new StateMessage();
        protected ReadyNextStateMessage readyNextStateMessage = new ReadyNextStateMessage();

        // Methods

        public virtual void ReadyForNextState()
        {
            SendToServer(readyNextStateMessage);
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            MessageTypes.Add(latestStateMessage.MessageType);
            MessageTypes.Add(readyNextStateMessage.MessageType);
        }

        /// <summary>
        /// Send to other devices the new state.
        /// </summary>
        /// <param name="newState">The new state to synchronise.</param>
        protected virtual void SendNewState(State newState)
        {
            if (newState != latestStateMessage.state)
            {
                sendStateMessage.state = newState;
                SendToServer(sendStateMessage);
            }
        }


        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == latestStateMessage.MessageType)
            {
                latestStateMessage = netMessage.ReadMessage<StateMessage>();
                StateUpdated.Invoke(latestStateMessage);
                return latestStateMessage;
            }
            else if (netMessage.msgType == readyNextStateMessage.MessageType)
            {
                // TODO : send the next state
            }
            return null;
        }
        
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == latestStateMessage.MessageType)
            {
                latestStateMessage = netMessage.ReadMessage<StateMessage>();
                StateUpdated.Invoke(latestStateMessage);
                return latestStateMessage;
            }
            return null;
        }

        /// <summary>
        /// See <see cref="DevicesSync.OnClientDeviceConnected(int)"/>.
        /// </summary>
        /// <param name="deviceId"></param>
        protected override void OnClientDeviceConnected(int deviceId)
        {
            SendToClient(deviceId, latestStateMessage);
        }

        /// <summary>
        /// See <see cref="DevicesSync.OnClientDeviceDisconnected(int)"/>.
        /// </summary>=
        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }
    }
}
