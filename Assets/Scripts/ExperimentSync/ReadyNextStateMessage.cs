using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesisExperiment.ExperimentSync
{
    /// <summary>
    /// Message that informs server that the user is ready for the next state.
    /// </summary>
    public class ReadyNextStateMessage : DevicesSyncMessage
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
    }
}
