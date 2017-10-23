using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesisExperiment.States
{
    /// <summary>
    /// Message that contains the new current state of the experiment.
    /// </summary>
    public class StateManagerMessage : DevicesSyncMessage
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

        // Methods

        public void Update(State currentState)
        {
            currentStateId = currentState.id;
        }

        public void Restore(StateManager stateManager)
        {
            stateManager.SetCurrentState(currentStateId);
        }
    }
}
