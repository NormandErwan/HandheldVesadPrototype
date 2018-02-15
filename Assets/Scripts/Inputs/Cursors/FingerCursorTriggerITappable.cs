using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerITappable : FingerCursorTriggerISelectable<ITappable>
  {
    // Constants

    public static readonly float tapTimeout = 0.5f; // in seconds

    // Methods

    protected override void OnTriggerExit(ITappable tappable, Collider other)
    {
      if (selectionTimers.ContainsKey(tappable) && Time.time - selectionTimers[tappable] < tapTimeout)
      {
        SetSelected(tappable);
      }

      base.OnTriggerExit(tappable, other);
    }

    protected override bool IsValid(ITappable tappable)
    {
      return base.IsValid(tappable) && tappable.IsTappable;
    }
  }
}
