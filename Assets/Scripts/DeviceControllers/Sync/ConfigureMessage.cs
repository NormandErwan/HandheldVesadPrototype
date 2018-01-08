using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers.Sync
{
  public class ConfigureMessage : DevicesSyncMessage
  {
    // Properties

    /// <summary>
    /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
    /// </summary>
    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

    /// <summary>
    /// See <see cref="DevicesSyncMessage.MessageType"/>.
    /// </summary>
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.DeviceControllerConfigure; } }

    // Variables

    /// <summary>
    /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
    /// </summary>
    public int senderConnectionId;

    public int participantId;
    public int conditionsOrdering;
    public bool participantIsRightHanded;
  }
}
