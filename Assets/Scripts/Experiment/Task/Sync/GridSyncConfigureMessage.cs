using DevicesSyncUnity.Messages;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSyncConfigureMessage : DevicesSyncMessage
  {
    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridConfigure; } }

    // Variables

    public int senderConnectionId;

    // Methods

    public void Update(Grid grid)
    {
      // TODO
    }

    public void Restore(Grid grid)
    {
      // TODO
    }
  }
}
