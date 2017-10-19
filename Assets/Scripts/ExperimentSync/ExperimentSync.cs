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

        // Variables

        protected StateMessage latestStateMessage = new StateMessage();
        protected StateMessage sendStateMessage = new StateMessage();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            MessageTypes.Add(latestStateMessage.MessageType);
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

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Updates <see cref="State"/> and invokes <see cref="StateUpdated"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected virtual DevicesSyncMessage ProcessMessageReceived(NetworkMessage netMessage)
        {
            latestStateMessage = netMessage.ReadMessage<StateMessage>();
            StateUpdated.Invoke(latestStateMessage);
            return latestStateMessage;
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
