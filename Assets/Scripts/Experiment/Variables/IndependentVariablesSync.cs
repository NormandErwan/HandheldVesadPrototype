using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
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

      foreach (var independentVariable in independentVariables)
      {
        DeviceConnected += DevicesInfoSync_DeviceConnected;
        independentVariable.RequestCurrentConditionSync += IndependentVariableManager_RequestCurrentConditionSync;
      }

      MessageTypes.Add(currentMessage.MessageType);
    }

    protected virtual void OnDestroy()
    {
      foreach (var indeVarManager in independentVariables)
      {
        DeviceConnected -= DevicesInfoSync_DeviceConnected;
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
      if (!isServer)
      {
        currentMessage = stateMessage;
        currentMessage.Restore(independentVariables);
      }
      return stateMessage;
    }

    protected virtual void DevicesInfoSync_DeviceConnected(int deviceId)
    {
      foreach (var independentVariable in independentVariables)
      {
        currentMessage.Update(independentVariable.id, independentVariable.CurrentConditionId);
        SendToClient(deviceId, currentMessage);
      }
    }

    protected virtual void IndependentVariableManager_RequestCurrentConditionSync(string independentVariableManagerId, string currentConditionId)
    {
      currentMessage.Update(independentVariableManagerId, currentConditionId);
      SendToServer(currentMessage);
    }
  }
}
