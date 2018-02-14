using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class CursorDistance : PositionDistance
  {
    public CursorDistance(FingerCursor cursor) : base()
    {
      Cursor = cursor;
    }

    public FingerCursor Cursor { get; protected set; }

    protected override bool UpdateThisFrame()
    {
      return Cursor.IsTracked;
    }

    protected override Vector3 GetCurrentPosition()
    {
      return Cursor.transform.position;
    }
  }
}
