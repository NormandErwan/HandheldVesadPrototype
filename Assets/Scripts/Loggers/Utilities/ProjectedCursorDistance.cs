using NormandErwan.MasterThesis.Experiment.Inputs;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class ProjectedCursorDistance : PositionDistance
  {
    public ProjectedCursorDistance(ProjectedCursor projectedCursor) : base()
    {
      ProjectedCursor = projectedCursor;
    }

    public ProjectedCursor ProjectedCursor { get; protected set; }

    protected override bool UpdateThisFrame()
    {
      return ProjectedCursor.IsOnGrid;
    }

    protected override Vector3 GetCurrentPosition()
    {
      return ProjectedCursor.transform.position;
    }
  }
}
