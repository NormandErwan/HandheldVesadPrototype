using DevicesSyncUnity.Messages;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSyncEventsMessage : DevicesSyncMessage
  {
    public enum GridEvent
    {
      Completed,
      SetDragging,
      SetZooming,
      ItemSelected,
      ItemMoved
    };

    // Constructors and destructor

    public TaskGridSyncEventsMessage(TaskGrid taskGrid, Action sendToServer)
    {
      TaskGrid = taskGrid;
      SendToServer = sendToServer;
      ReadyToSend = false;

      TaskGrid.CompleteSync += TaskGrid_CompleteSync;

      TaskGrid.SetDraggingSync += TaskGrid_SetDraggingSync;
      TaskGrid.SetZoomingSync += TaskGrid_SetZoomingSync;

      TaskGrid.ItemSelectSync += TaskGrid_ItemSelectSync;
      TaskGrid.ItemMoveSync += TaskGrid_ItemMoveSync;
    }

    public TaskGridSyncEventsMessage()
    {
    }

    ~TaskGridSyncEventsMessage()
    {
      TaskGrid.CompleteSync -= TaskGrid_CompleteSync;

      TaskGrid.SetDraggingSync -= TaskGrid_SetDraggingSync;
      TaskGrid.SetZoomingSync -= TaskGrid_SetZoomingSync;

      TaskGrid.ItemSelectSync -= TaskGrid_ItemSelectSync;
      TaskGrid.ItemMoveSync -= TaskGrid_ItemMoveSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridEvents; } }
    public bool ReadyToSend { get; protected set; }

    protected TaskGrid TaskGrid { get; private set; }
    protected Action SendToServer { get; private set; }

    // Variables

    public int senderConnectionId;
    public GridEvent gridEvent;

    public bool isDragging;
    public bool isZooming;
    public Vector3 translation;
    public Vector3 scaling;

    public Vector2 containerPosition;
    public int itemIndex;

    // Methods

    public void SyncGrid(TaskGrid taskGrid)
    {
      if (gridEvent == GridEvent.Completed)
      {
        taskGrid.SetCompleted();
      }
      else if (gridEvent == GridEvent.SetDragging)
      {
        taskGrid.SetDragged(isDragging);
      }
      else if (gridEvent == GridEvent.SetZooming)
      {
        taskGrid.SetZoomed(isZooming);
      }
      else
      {
        var container = taskGrid.At(new Vector2Int((int)containerPosition.x, (int)containerPosition.y));
        if (gridEvent == GridEvent.ItemSelected)
        {
          var item = container.Elements[itemIndex];
          taskGrid.SetItemSelected(item, container);
        }
        else if (gridEvent == GridEvent.ItemMoved)
        {
          taskGrid.SetItemMoved(container);
        }
      }
    }

    protected virtual void TaskGrid_CompleteSync()
    {
      gridEvent = GridEvent.Completed;
      SendToServer();

      TaskGrid.SetCompleted();
    }

    protected virtual void TaskGrid_SetDraggingSync(bool isDragging)
    {
      this.isDragging = isDragging;
      gridEvent = GridEvent.SetDragging;
      ReadyToSend = true;

      TaskGrid.SetDragged(isDragging);
    }

    protected virtual void TaskGrid_SetZoomingSync(bool isZooming)
    {
      this.isZooming = isZooming;
      gridEvent = GridEvent.SetZooming;
      ReadyToSend = true;

      TaskGrid.SetZoomed(isZooming);
    }

    protected virtual void TaskGrid_ItemSelectSync(Item item)
    {
      var container = TaskGrid.GetContainer(item);
      containerPosition = TaskGrid.GetPosition(container);
      itemIndex = container.Elements.IndexOf(item);
      gridEvent = GridEvent.ItemSelected;
      SendToServer();

      TaskGrid.SetItemSelected(item, container);
    }

    protected virtual void TaskGrid_ItemMoveSync(Container container)
    {
      containerPosition = TaskGrid.GetPosition(container);
      gridEvent = GridEvent.ItemMoved;
      SendToServer();

      TaskGrid.SetItemMoved(container);
    }
  }
}
