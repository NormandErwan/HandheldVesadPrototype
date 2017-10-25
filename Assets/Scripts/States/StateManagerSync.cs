using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.States
{
    /// <summary>
    /// Synchronize the experiment between devices with <see cref="StateManagerMessage"/>.
    /// </summary>
    public class StateManagerSync : DevicesSync
    {
        // Editor Fields

        public StateManager stateManager;

        // Properties

        protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

        // Variables

        protected StateManagerMessage currentStateMessage = new StateManagerMessage();

        // Methods

        protected override void Awake()
        {
            base.Awake();

            DeviceDisconnected += DevicesInfoSync_DeviceDisconnected;
            stateManager.RequestCurrentStateSync += StateManager_RequestCurrentStateSync;

            MessageTypes.Add(currentStateMessage.MessageType);
        }

        protected virtual void OnDestroy()
        {
            DeviceDisconnected -= DevicesInfoSync_DeviceDisconnected;
            stateManager.RequestCurrentStateSync -= StateManager_RequestCurrentStateSync;
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            currentStateMessage = netMessage.ReadMessage<StateManagerMessage>();
            currentStateMessage.Restore(stateManager);
            return currentStateMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var stateMessage = netMessage.ReadMessage<StateManagerMessage>();
            if (!isServer)
            {
                currentStateMessage = stateMessage;
                currentStateMessage.Restore(stateManager);
            }
            return stateMessage;
        }

        protected virtual void DevicesInfoSync_DeviceDisconnected(int deviceId)
        {
            if (deviceId != NetworkManager.client.connection.connectionId)
            {
                currentStateMessage.Update(stateManager.CurrentState);
                SendToClient(deviceId, currentStateMessage);
            }
        }

        protected virtual void StateManager_RequestCurrentStateSync(State currentState)
        {
            currentStateMessage.Update(currentState);
            SendToServer(currentStateMessage);
        }
    }
}
