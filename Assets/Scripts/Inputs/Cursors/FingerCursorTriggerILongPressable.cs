using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerILongPressable : FingerCursorTriggerISelectable<ILongPressable>
  {
    // Constants

    public const float longPressMinTime = 0.5f; // in seconds

    // Methods

    protected override void OnTriggerStay(ILongPressable longPressable, Collider other)
    {
      base.OnTriggerStay(longPressable, other);

      if (selectionTimers.ContainsKey(longPressable) && Time.time - selectionTimers[longPressable] > longPressMinTime)
      {
        SetSelected(longPressable);
      }
    }

    protected override bool IsValid(ILongPressable longPressable)
    {
      return base.IsValid(longPressable) && longPressable.IsLongPressable;
    }
  }
}
