using DevicesSyncUnity.Messages;
using System.Collections.Generic;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public class IndependentVariablesMessage : DevicesSyncMessage
  {
    // Properties

    /// <summary>
    /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
    /// </summary>
    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

    /// <summary>
    /// See <see cref="DevicesSyncMessage.MessageType"/>.
    /// </summary>
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.IndependentVariables; } }

    // Variables

    /// <summary>
    /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
    /// </summary>
    public int senderConnectionId;

    public string independentVariableId;

    public string currentConditionId;

    // Methods

    public void Update(string independentVariableId, string currentConditionId)
    {
      this.independentVariableId = independentVariableId;
      this.currentConditionId = currentConditionId;
    }

    public void Restore(List<IIndependentVariable> independentVariables)
    {
      foreach (var independentVariable in independentVariables)
      {
        if (independentVariable.Id == independentVariableId)
        {
          independentVariable.SetCurrentCondition(currentConditionId);
        }
      }
    }
  }
}
