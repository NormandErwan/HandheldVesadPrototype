using DevicesSyncUnity.Messages;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSyncTransformMessage : DevicesSyncMessage
  {
    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridTransform ; } }

    public bool HasChanged { get; protected set; }

    // Variables

    public int senderConnectionId;

    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
    // TODO: events

    // Methods

    public void Update(Grid grid, float movementThreshold)
    {
      HasChanged = !VectorEquals(localPosition, grid.transform.localPosition, movementThreshold)
                || !VectorEquals(localRotation.eulerAngles, grid.transform.localRotation.eulerAngles, movementThreshold)
                || !VectorEquals(localScale, grid.transform.localScale, movementThreshold);
      localPosition = grid.transform.localPosition;
      localRotation = grid.transform.localRotation;
      localScale = grid.transform.localScale;
    }

    public void Restore(Grid grid)
    {
      grid.transform.localPosition = localPosition;
      grid.transform.localRotation = localRotation;
      grid.transform.localScale = localScale;
    }

    protected bool VectorEquals(Vector3 v1, Vector3 v2, float precision = 0.001f)
    {
      return (v1 - v2).sqrMagnitude < (precision * precision);
    }
  }
}
