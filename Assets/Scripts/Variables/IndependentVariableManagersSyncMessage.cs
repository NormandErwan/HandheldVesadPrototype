using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesisExperiment.Variables.Sync
{
    public class IndependentVariableManagersSyncMessage : DevicesSyncMessage
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

        public int independentVariableManagerId;

        public int currentConditionIndex;

        // Methods

        public void Update(IIndependentVariableManager[] independentVariableManagers, int independentVariableManagerId)
        {
            foreach (var indeVarManager in independentVariableManagers)
            {
                if (indeVarManager.id == independentVariableManagerId)
                {
                    independentVariableManagerId = indeVarManager.id;
                    currentConditionIndex = indeVarManager.CurrentConditionIndex;
                }
            }
        }

        public void Restore(IIndependentVariableManager[] independentVariableManagers)
        {
            foreach (var indeVarManager in independentVariableManagers)
            {
                if (indeVarManager.id == independentVariableManagerId)
                {
                    indeVarManager.CurrentConditionIndex = currentConditionIndex;
                }
            }
        }
    }
}
