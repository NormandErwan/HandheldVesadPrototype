using DevicesSyncUnity.Messages;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSyncDragMessage : DevicesSyncMessage
  {
    // Constructors and destructor

    public TaskGridSyncDragMessage(TaskGrid taskGrid)
    {
      TaskGrid = taskGrid;
      Sent();

      TaskGrid.DragSync += TaskGrid_DragSync;
    }

    public TaskGridSyncDragMessage()
    {
    }

    ~TaskGridSyncDragMessage()
    {
      TaskGrid.DragSync -= TaskGrid_DragSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridDrag; } }
    public bool ReadyToSend { get; protected set; }

    protected TaskGrid TaskGrid { get; private set; }

    // Variables

    public int senderConnectionId;
    public Vector3 translation;

    // Methods

    public void SyncGrid(TaskGrid taskGrid)
    {
       taskGrid.SetDragged(translation);
    }

    public void Sent()
    {
      translation = Vector3.zero;
      ReadyToSend = false;
    }

    protected virtual void TaskGrid_DragSync(Vector3 translation)
    {
      this.translation += translation;
      ReadyToSend = true;

      TaskGrid.SetDragged(translation);
    }
  }
}
