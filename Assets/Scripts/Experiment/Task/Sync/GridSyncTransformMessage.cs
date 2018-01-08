using DevicesSyncUnity.Messages;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSyncTransformMessage : DevicesSyncMessage
  {
    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridTransform ; } }

    // Variables

    public int senderConnectionId;

    public bool transformChanged;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;

    public bool completed;

    // Methods

    public void Update(Grid grid, float movementThreshold)
    {
      transformChanged = !VectorEquals(localPosition, grid.transform.localPosition, movementThreshold)
                || !VectorEquals(localRotation.eulerAngles, grid.transform.localRotation.eulerAngles, movementThreshold)
                || !VectorEquals(localScale, grid.transform.localScale, movementThreshold);
      if (transformChanged)
      {
        localPosition = grid.transform.localPosition;
        localRotation = grid.transform.localRotation;
        localScale = grid.transform.localScale;
      }
    }

    public void Restore(Grid grid)
    {
      if (transformChanged)
      {
        grid.transform.localPosition = localPosition;
        grid.transform.localRotation = localRotation;
        grid.transform.localScale = localScale;
      }

      if (completed)
      {
        grid.SetCompleted();
      }
    }

    protected bool VectorEquals(Vector3 v1, Vector3 v2, float precision = 0.001f)
    {
      return (v1 - v2).sqrMagnitude < (precision * precision);
    }
  }
}
