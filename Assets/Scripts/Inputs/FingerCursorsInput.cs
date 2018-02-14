using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public abstract class FingerCursorsInput : CursorsInput<FingerCursor>
  {
    // Constants

    protected static readonly float cursorColliderRadiusFactor = 0.65f;

    // Editor fields

    [SerializeField]
    private FingerCursor[] cursors;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      foreach (var cursor in cursors)
      {
        Cursors.Add(cursor.Type, cursor);
      }
    }

    // Methods

    public virtual void Configure(float maxSelectableDistance)
    {
      foreach (var cursor in Cursors)
      {
        cursor.Value.MaxSelectableDistance = maxSelectableDistance;
      }
    }

    public override void DeactivateCursors()
    {
      base.DeactivateCursors();
      foreach (var cursor in Cursors)
      {
        cursor.Value.IsTracked = false;
      }
    }
  }
}
