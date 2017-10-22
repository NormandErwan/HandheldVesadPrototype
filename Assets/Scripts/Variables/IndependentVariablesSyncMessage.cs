using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public class IndependentVariablesSyncMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return MasterThesisExperiment.MessageType.IndependentVariableManagers; } }

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

        public void Restore(IIndependentVariable[] independentVariables)
        {
            foreach (var independentVariable in independentVariables)
            {
                if (independentVariable.id == independentVariableId)
                {
                    independentVariable.SetCondition(currentConditionId);
                }
            }
        }
    }
}
