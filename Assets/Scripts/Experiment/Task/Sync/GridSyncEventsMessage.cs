using DevicesSyncUnity.Messages;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSyncEventsMessage : DevicesSyncMessage
  {
    public enum GridEvent
    {
      Completed,
      ItemSelected,
      ItemMoved
    };

    // Constructors and destructor

    public GridSyncEventsMessage(Grid grid, Action sendToServer)
    {
      Grid = grid;
      SendToServer = sendToServer;

      grid.CompleteSync += Grid_CompleteSync;
      grid.ItemSelectedSync += Grid_ItemSelectedSync;
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
    public Vector2 containerPosition;
    public int itemIndex;

    // Methods

    public void SyncGrid(Grid grid)
    {
      switch (gridEvent)
      {
        case GridEvent.Completed:
          grid.SetCompleted();
          break;

        case GridEvent.ItemSelected:
          var container = grid.At(new Vector2Int((int)containerPosition.x, (int)containerPosition.y));
          grid.SetItemSelected(container.Elements[itemIndex], container);
          break;

        case GridEvent.ItemMoved:
          break;
      }
    }

    private void Grid_ItemSelectedSync(Item item)
    {
      var container = Grid.GetContainer(item);
      containerPosition = Grid.GetPosition(container);
      itemIndex = container.Elements.IndexOf(item);
      gridEvent = GridEvent.ItemSelected;
      SendToServer();
    }

    protected void Grid_CompleteSync()
    {
      gridEvent = GridEvent.Completed;
      SendToServer();
    }
  }
}
