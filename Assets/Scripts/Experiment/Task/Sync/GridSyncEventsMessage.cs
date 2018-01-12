using DevicesSyncUnity.Messages;
using System;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSyncEventsMessage : DevicesSyncMessage
  {
    public enum GridEvent
    {
      Completed
    };

    // Constructors and destructor

    public GridSyncEventsMessage(Grid grid, Action sendToServer)
    {
      Grid = grid;
      SendToServer = sendToServer;

      grid.CompleteSync += Grid_CompleteSync;
    }

    public GridSyncEventsMessage()
    {
    }

    ~GridSyncEventsMessage()
    {
      Grid.CompleteSync -= Grid_CompleteSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridEvents; } }

    protected Grid Grid { get; }
    protected Action SendToServer { get; }

    // Variables

    public int senderConnectionId;
    public GridEvent gridEvent;

    // Methods

    public void SyncGrid(Grid grid)
    {
      if (gridEvent == GridEvent.Completed)
      {
        grid.SetCompleted();
      }
    }

    protected void Grid_CompleteSync()
    {
      gridEvent = GridEvent.Completed;
      SendToServer();
    }
  }
}
