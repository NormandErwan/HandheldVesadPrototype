using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesisExperiment.States;

namespace NormandErwan.MasterThesisExperiment.ExperimentSync
{
    /// <summary>
    /// Message that contains the new current state of the experiment.
    /// </summary>
    public class StateMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return MasterThesisExperiment.MessageType.State; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// The state to send.
        /// </summary>
        public State state;
    }
}
