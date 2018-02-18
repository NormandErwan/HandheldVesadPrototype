using DevicesSyncUnity.Messages;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSyncZoomMessage : DevicesSyncMessage
  {
    // Constructors and destructor

    public TaskGridSyncZoomMessage(TaskGrid taskGrid)
    {
      TaskGrid = taskGrid;
      Sent();

      TaskGrid.ZoomSync += TaskGrid_ZoomSync;
    }

    public TaskGridSyncZoomMessage()
    {
    }

    ~TaskGridSyncZoomMessage()
    {
      TaskGrid.ZoomSync -= TaskGrid_ZoomSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridZoom; } }
    public bool ReadyToSend { get; protected set; }

    protected TaskGrid TaskGrid { get; private set; }

    // Variables

    public int senderConnectionId;
    public Vector3 scaling;
    public Vector3 translation;

    // Methods

    public void SyncGrid(TaskGrid taskGrid)
    {
      taskGrid.SetZoomed(scaling, translation);
    }

    public void Sent()
    {
      scaling = Vector3.one;
      translation = Vector3.zero;
      ReadyToSend = false;
    }

    protected virtual void TaskGrid_ZoomSync(Vector3 scaling, Vector3 translation)
    {
      this.scaling = Vector3.Scale(this.scaling, scaling);
      this.translation += translation;
      ReadyToSend = true;

      TaskGrid.SetZoomed(scaling, translation);
    }
  }
}
