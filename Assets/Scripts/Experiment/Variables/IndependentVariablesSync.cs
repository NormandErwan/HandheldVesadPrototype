using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public class IndependentVariablesSync : DevicesSync
  {
    // Editor Fields

    [SerializeField]
    private IIndependentVariable[] independentVariables;

    // Properties

    public List<IIndependentVariable> IndependentVariables { get; protected set; }
    protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

    // Variables

    protected IndependentVariablesMessage independentVariablesMessage = new IndependentVariablesMessage();

    // Methods

    protected override void Awake()
    {
      base.Awake();

      MessageTypes.Add(independentVariablesMessage.MessageType);

      IndependentVariables = new List<IIndependentVariable>(independentVariables.Length);
      foreach (var independentVariable in independentVariables)
      {
        IndependentVariables.Add(independentVariable);
        independentVariable.CurrentConditionSync += IndependentVariable_CurrentConditionSync;
      }
      DeviceConnected += DevicesInfoSync_DeviceConnected;
    }

    protected virtual void OnDestroy()
    {
      foreach (var independentVariable in IndependentVariables)
      {
        independentVariable.CurrentConditionSync -= IndependentVariable_CurrentConditionSync;
      }
      DeviceConnected -= DevicesInfoSync_DeviceConnected;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      var independentVariablesMessage = netMessage.ReadMessage<IndependentVariablesMessage>();
      independentVariablesMessage.Restore(IndependentVariables);
      return independentVariablesMessage;
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      var independentVariablesMessage = netMessage.ReadMessage<IndependentVariablesMessage>();
      if (!isServer)
      {
        independentVariablesMessage.Restore(IndependentVariables);
      }
      return independentVariablesMessage;
    }

    protected virtual void DevicesInfoSync_DeviceConnected(int deviceId)
    {
      foreach (var independentVariable in IndependentVariables)
      {
        independentVariablesMessage.Update(independentVariable.Id, independentVariable.CurrentConditionId);
        SendToClient(deviceId, independentVariablesMessage);
      }
    }

    protected virtual void IndependentVariable_CurrentConditionSync(string independentVariableId, string currentConditionId)
    {
      independentVariablesMessage.Update(independentVariableId, currentConditionId);
      SendToServer(independentVariablesMessage);
    }
  }
}
