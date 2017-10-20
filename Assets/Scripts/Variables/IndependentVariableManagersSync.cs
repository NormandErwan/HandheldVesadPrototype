using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.Variables.Sync
{
    public class IndependentVariableManagersSync : DevicesSync
    {
        // Editor Fields

        public IIndependentVariableManager[] independentVariableManagers;

        // Properties

        protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

        // Variables

        protected IndependentVariableManagersSyncMessage currentIndeVarManagersMessage = new IndependentVariableManagersSyncMessage();

        // Methods

        protected override void Awake()
        {
            base.Awake();
            MessageTypes.Add(currentIndeVarManagersMessage.MessageType);
        }

        protected override void Start()
        {
            base.Start();
            foreach (var indeVarManager in independentVariableManagers)
            {
                indeVarManager.RequestCurrentConditionSync += IndependentVariableManager_RequestCurrentConditionSync;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var indeVarManager in independentVariableManagers)
            {
                indeVarManager.RequestCurrentConditionSync -= IndependentVariableManager_RequestCurrentConditionSync;
            }
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            currentIndeVarManagersMessage = netMessage.ReadMessage<IndependentVariableManagersSyncMessage>();
            currentIndeVarManagersMessage.Restore(independentVariableManagers);
            return currentIndeVarManagersMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var stateMessage = netMessage.ReadMessage<IndependentVariableManagersSyncMessage>();
            if (!isServer) // Don't update twice if the device is a host
            {
                currentIndeVarManagersMessage = stateMessage;
                currentIndeVarManagersMessage.Restore(independentVariableManagers);
            }
            return stateMessage;
        }

        protected override void OnClientDeviceConnected(int deviceId)
        {
            if (deviceId != NetworkManager.client.connection.connectionId)
            {
                foreach (var indeVarManager in independentVariableManagers)
                {
                    currentIndeVarManagersMessage.Update(independentVariableManagers, indeVarManager.id);
                    SendToClient(deviceId, currentIndeVarManagersMessage);
                }
            }
        }

        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }

        protected virtual void IndependentVariableManager_RequestCurrentConditionSync(int independentVariableManagerId)
        {
            currentIndeVarManagersMessage.Update(independentVariableManagers, independentVariableManagerId);
            SendToServer(currentIndeVarManagersMessage);
        }
    }
}
