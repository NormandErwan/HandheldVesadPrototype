using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesisExperiment.States
{
    /// <summary>
    /// Message that contains the new current state of the experiment.
    /// </summary>
    public class StateManagerSyncMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return MasterThesisExperiment.MessageType.StateManager; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        public string currentStateId;

        public string currentStateTitle;

        public string currentStateInstructions;

        public int currentTrial;

        public int statesTotal;

        public int statesProgress;

        public int conditionsTotal;

        public int conditionsProgress;

        public int trialsTotal;

        public int trialsProgress;

        // Methods

        public void Update(StateManager stateManager)
        {
            currentStateId = stateManager.CurrentState.id;
            currentStateTitle = stateManager.CurrentState.title;
            currentStateInstructions = stateManager.CurrentState.instructions;
            currentTrial = stateManager.CurrentTrial;
            statesTotal = stateManager.StatesTotal;
            statesProgress = stateManager.StatesProgress;
            conditionsTotal = stateManager.ConditionsTotal;
            conditionsProgress = stateManager.ConditionsProgress;
            trialsTotal = stateManager.TrialsTotal;
            trialsProgress = stateManager.TrialsProgress;
        }

        public void Restore(StateManager stateManager)
        {
            stateManager.CurrentTrial = currentTrial;
            stateManager.StatesTotal = statesTotal;
            stateManager.StatesProgress = statesProgress;
            stateManager.ConditionsTotal = conditionsTotal;
            stateManager.ConditionsProgress = conditionsProgress;
            stateManager.TrialsTotal = trialsTotal;
            stateManager.TrialsProgress = trialsProgress;

            var currentState = stateManager.CurrentState;
            currentState.id = currentStateId;
            currentState.title = currentStateTitle;
            currentState.instructions = currentStateInstructions;
            stateManager.CurrentState = currentState;
        }
    }
}
