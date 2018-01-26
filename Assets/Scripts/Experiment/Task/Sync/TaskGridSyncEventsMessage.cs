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
      Dragged,
      SetZooming,
      Zoomed,
      ItemSelected,
      ItemMoved
    };

    // Constructors and destructor

    public TaskGridSyncEventsMessage(TaskGrid taskGrid, Action sendToServer)
    {
      TaskGrid = taskGrid;
      SendToServer = sendToServer;

      TaskGrid.CompleteSync += TaskGrid_CompleteSync;

      TaskGrid.SetDraggingSync += TaskGrid_SetDraggingSync;
      TaskGrid.DragSync += TaskGrid_DragSync;

      TaskGrid.SetZoomingSync += TaskGrid_SetZoomingSync;
      TaskGrid.ZoomSync += TaskGrid_ZoomSync;

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
      TaskGrid.DragSync -= TaskGrid_DragSync;

      TaskGrid.SetZoomingSync -= TaskGrid_SetZoomingSync;
      TaskGrid.ZoomSync -= TaskGrid_ZoomSync;

      TaskGrid.ItemSelectSync -= TaskGrid_ItemSelectSync;
      TaskGrid.ItemMoveSync -= TaskGrid_ItemMoveSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridEvents; } }

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

    public void SyncGrid(TaskGrid grid)
    {
      if (gridEvent == GridEvent.Completed)
      {
        grid.SetCompleted();
      }
      else if (gridEvent == GridEvent.SetDragging)
      {
        grid.SetDragged(isDragging);
      }
      else if (gridEvent == GridEvent.Dragged)
      {
        grid.SetDragged(translation);
      }
      else if (gridEvent == GridEvent.SetZooming)
      {
        grid.SetZoomed(isZooming);
      }
      else if (gridEvent == GridEvent.Zoomed)
      {
        grid.SetZoomed(scaling, translation);
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

    protected virtual void TaskGrid_CompleteSync()
    {
      gridEvent = GridEvent.Completed;
      SendToServer();
    }

    protected virtual void TaskGrid_SetDraggingSync(bool isDragging)
    {
      this.isDragging = isDragging;
      gridEvent = GridEvent.SetDragging;
      SendToServer();
    }

    protected virtual void TaskGrid_DragSync(Vector3 translation)
    {
      this.translation = translation;
      gridEvent = GridEvent.Dragged;
      SendToServer();
    }

    protected virtual void TaskGrid_SetZoomingSync(bool isZooming)
    {
      this.isZooming = isZooming;
      gridEvent = GridEvent.SetZooming;
      SendToServer();
    }

    protected virtual void TaskGrid_ZoomSync(Vector3 scaling, Vector3 translation)
    {
      this.scaling = scaling;
      this.translation = translation;
      gridEvent = GridEvent.Zoomed;
      SendToServer();
    }

    protected virtual void TaskGrid_ItemSelectSync(Item item)
    {
      var container = TaskGrid.GetContainer(item);
      containerPosition = TaskGrid.GetPosition(container);
      itemIndex = container.Elements.IndexOf(item);
      gridEvent = GridEvent.ItemSelected;
      SendToServer();
    }

    protected virtual void TaskGrid_ItemMoveSync(Container newContainer)
    {
      containerPosition = TaskGrid.GetPosition(newContainer);
      gridEvent = GridEvent.ItemMoved;
      SendToServer();
    }
  }
}
