using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class CursorDistance : PositionDistance
  {
    public CursorDistance(Inputs.Cursor cursor) : base()
    {
      Cursor = cursor;
    }

    public Inputs.Cursor Cursor { get; protected set; }

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
