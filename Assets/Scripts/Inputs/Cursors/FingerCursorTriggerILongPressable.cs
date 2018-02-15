using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerILongPressable : FingerCursorTriggerISelectable<ILongPressable>
  {
    // Constants

    public static readonly float longPressMinTime = 0.5f; // in seconds

    // Variables

    protected Dictionary<ILongPressable, float> longPressTimers = new Dictionary<ILongPressable, float>();

    // Methods

    protected override void OnTriggerEnter(ILongPressable longPressable, Collider other)
    {
      if (longPressable.IsInteractable && longPressable.IsSelectable && longPressable.IsLongPressable)
      {
        longPressTimers.Add(longPressable, Time.time);
      }
    }

    protected override void OnTriggerStay(ILongPressable longPressable, Collider other)
    {
      bool clearTimers = false;

      if (longPressTimers.ContainsKey(longPressable))
      {
        if (CancelTimer(longPressable))
        {
          longPressTimers.Remove(longPressable);
        }
        else if (Time.time - longPressTimers[longPressable] > longPressMinTime)
        {
          clearTimers = true;
          if (longPressable.IsInteractable && longPressable.IsSelectable && longPressable.IsLongPressable)
          {
            longPressable.SetSelected(true);
          }
        }
      }

      if (clearTimers)
      {
        longPressTimers.Clear();
      }
    }

    protected override void OnTriggerExit(ILongPressable longPressable, Collider other)
    {
      if (longPressTimers.ContainsKey(longPressable))
      {
        longPressTimers.Remove(longPressable);
      }
    }
  }
}
