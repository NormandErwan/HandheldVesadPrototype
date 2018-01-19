using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursorsSyncMessage : DevicesSyncMessage
  {
    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.ProjectedCursors; } }
    public bool TransformChanged { get; protected set; }

    // Variables

    public int senderConnectionId;
    public CursorType[] cursors;
    public Vector3[] localPositions;

    // Methods

    public void Update(Dictionary<CursorType, ProjectedCursor> projectedCursors)
    {
      for (int i = 0; i < cursors.Length; i++)
      {
        var projectedCursor = projectedCursors[cursors[i]];
        TransformChanged = !VectorEquals(localPositions[i], projectedCursor.transform.localPosition);
        if (TransformChanged)
        {
          cursors[i] = projectedCursor.Cursor.Type;
          localPositions[i] = projectedCursor.transform.localPosition;
        }
      }
    }

    public void Restore(Dictionary<CursorType, ProjectedCursor> projectedCursors)
    {
      for (int i = 0; i < cursors.Length; i++)
      {
        projectedCursors[cursors[i]].transform.localPosition = localPositions[i];
      }
    }

    protected bool VectorEquals(Vector3 v1, Vector3 v2, float precision = 0.001f)
    {
      return (v1 - v2).sqrMagnitude < (precision * precision);
    }
  }
}
