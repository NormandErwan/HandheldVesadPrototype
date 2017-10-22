using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public class IndependentVariablesSync : DevicesSync
    {
        // Editor Fields

        public IIndependentVariable[] independentVariables;

        // Properties

        protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

        // Variables

        protected IndependentVariablesSyncMessage currentIVMessage = new IndependentVariablesSyncMessage();

        // Methods

        protected override void Awake()
        {
            base.Awake();
            MessageTypes.Add(currentIVMessage.MessageType);
        }

        protected override void Start()
        {
            base.Start();
            foreach (var indeVarManager in independentVariables)
            {
                indeVarManager.RequestCurrentConditionSync += IndependentVariableManager_RequestCurrentConditionSync;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var indeVarManager in independentVariables)
            {
                indeVarManager.RequestCurrentConditionSync -= IndependentVariableManager_RequestCurrentConditionSync;
            }
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            currentIVMessage = netMessage.ReadMessage<IndependentVariablesSyncMessage>();
            currentIVMessage.Restore(independentVariables);
            return currentIVMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var stateMessage = netMessage.ReadMessage<IndependentVariablesSyncMessage>();
            if (!isServer) // Don't update twice if the device is a host
            {
                currentIVMessage = stateMessage;
                currentIVMessage.Restore(independentVariables);
            }
            return stateMessage;
        }

        protected override void OnClientDeviceConnected(int deviceId)
        {
            if (deviceId != NetworkManager.client.connection.connectionId)
            {
                foreach (var independentVariable in independentVariables)
                {
                    currentIVMessage.Update(independentVariable.id, independentVariable.CurrentConditionId);
                    SendToClient(deviceId, currentIVMessage);
                }
            }
        }

        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }

        protected virtual void IndependentVariableManager_RequestCurrentConditionSync(string independentVariableManagerId, string currentConditionId)
        {
            currentIVMessage.Update(independentVariableManagerId, currentConditionId);
            SendToServer(currentIVMessage);
        }
    }
}
