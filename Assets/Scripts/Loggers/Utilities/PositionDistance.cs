using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public abstract class PositionDistance : Counter<float>
  {
    protected Vector3 previousPosition;

    protected override float GetCurrent()
    {
      var position = GetCurrentPosition();
      var distance = (position - previousPosition).magnitude;
      previousPosition = position;
      return distance;
    }

    protected override void UpdateTotal()
    {
      Total += Current;
    }

    protected abstract Vector3 GetCurrentPosition();
  }
}
