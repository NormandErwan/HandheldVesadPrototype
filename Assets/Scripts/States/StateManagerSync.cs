using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.States
{
    /// <summary>
    /// Synchronize the experiment between devices with <see cref="StateManagerSyncMessage"/>.
    /// </summary>
    public class StateManagerSync : DevicesSync
    {
        // Editor Fields

        public StateManager stateManager;

        // Properties

        protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

        // Variables

        protected StateManagerSyncMessage currentStateMessage = new StateManagerSyncMessage();

        // Methods

        protected override void Awake()
        {
            base.Awake();
            MessageTypes.Add(currentStateMessage.MessageType);
        }

        protected override void Start()
        {
            base.Start();
            stateManager.RequestCurrentStateSync += StateManager_RequestCurrentStateSync;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stateManager.RequestCurrentStateSync -= StateManager_RequestCurrentStateSync;
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            currentStateMessage = netMessage.ReadMessage<StateManagerSyncMessage>();
            currentStateMessage.Restore(stateManager);
            return currentStateMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var stateMessage = netMessage.ReadMessage<StateManagerSyncMessage>();
            if (!isServer) // Don't update twice if the device is a host
            {
                currentStateMessage = stateMessage;
                currentStateMessage.Restore(stateManager);
            }
            return stateMessage;
        }

        protected override void OnClientDeviceConnected(int deviceId)
        {
            if (deviceId != NetworkManager.client.connection.connectionId)
            {
                currentStateMessage.Update(stateManager);
                SendToClient(deviceId, currentStateMessage);
            }
        }

        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }

        protected virtual void StateManager_RequestCurrentStateSync(State currentState)
        {
            currentStateMessage.Update(stateManager);
            SendToServer(currentStateMessage);
        }

        
    }
}
