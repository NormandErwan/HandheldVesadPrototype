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

      Grid.CompleteSync += Grid_CompleteSync;
      Grid.ItemSelectSync += Grid_ItemSelectSync;
      Grid.ItemMoveSync += Grid_ItemMoveSync;
    }

    public GridSyncEventsMessage()
    {
    }

    ~GridSyncEventsMessage()
    {
      Grid.CompleteSync -= Grid_CompleteSync;
      Grid.ItemSelectSync -= Grid_ItemSelectSync;
      Grid.ItemMoveSync -= Grid_ItemMoveSync;
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
      if (gridEvent == GridEvent.Completed)
      {
        grid.SetCompleted();
      }
      else
      {
        var container = grid.At(new Vector2Int((int)containerPosition.x, (int)containerPosition.y));
        if (gridEvent == GridEvent.ItemSelected)
        {
          var item = container.Elements[itemIndex];
          grid.SetItemSelected(item, container);
        }
        else if (gridEvent == GridEvent.ItemMoved)
        {
          grid.SetItemMoved(container);
        }
      }
    }

    protected virtual void Grid_CompleteSync()
    {
      gridEvent = GridEvent.Completed;
      SendToServer();
    }

    protected virtual void Grid_ItemSelectSync(Item item)
    {
      var container = Grid.GetContainer(item);
      containerPosition = Grid.GetPosition(container);
      itemIndex = container.Elements.IndexOf(item);
      gridEvent = GridEvent.ItemSelected;
      SendToServer();
    }

    protected virtual void Grid_ItemMoveSync(Container newContainer)
    {
      containerPosition = Grid.GetPosition(newContainer);
      gridEvent = GridEvent.ItemMoved;
      SendToServer();
    }
  }
}
