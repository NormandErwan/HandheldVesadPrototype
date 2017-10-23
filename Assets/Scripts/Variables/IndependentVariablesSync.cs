using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using System.Collections.Generic;
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

        protected IndependentVariablesMessage currentMessage = new IndependentVariablesMessage();

        // Methods

        protected override void Awake()
        {
            base.Awake();
            MessageTypes.Add(currentMessage.MessageType);
        }

        protected override void Start()
        {
            base.Start();
            foreach (var independentVariable in independentVariables)
            {
                independentVariable.RequestCurrentConditionSync += IndependentVariableManager_RequestCurrentConditionSync;
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
            currentMessage = netMessage.ReadMessage<IndependentVariablesMessage>();
            currentMessage.Restore(independentVariables);
            return currentMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var stateMessage = netMessage.ReadMessage<IndependentVariablesMessage>();
            if (!isServer) // Don't update twice if the device is a host
            {
                currentMessage = stateMessage;
                currentMessage.Restore(independentVariables);
            }
            return stateMessage;
        }

        protected override void OnClientDeviceConnected(int deviceId)
        {
            if (deviceId != NetworkManager.client.connection.connectionId)
            {
                foreach (var independentVariable in independentVariables)
                {
                    currentMessage.Update(independentVariable.id, independentVariable.CurrentConditionId);
                    SendToClient(deviceId, currentMessage);
                }
            }
        }

        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }

        protected virtual void IndependentVariableManager_RequestCurrentConditionSync(string independentVariableManagerId, string currentConditionId)
        {
            currentMessage.Update(independentVariableManagerId, currentConditionId);
            SendToServer(currentMessage);
        }
    }
}
