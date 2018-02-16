using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerITappable : FingerCursorTriggerISelectable<ITappable>
  {
    // Constants

    public const float tapMinTime = 0.08f; // in seconds
    public const float tapTimeout = 0.5f; // in seconds
    public const float tappedDelay = 0.15f; // in seconds

    // Variables

    protected Dictionary<ITappable, float> futureSelected = new Dictionary<ITappable, float>();
    protected List<ITappable> futureSelectedRemove = new List<ITappable>();

    // Methods

    public override void ProcessPriorityLists()
    {
      base.ProcessPriorityLists();

      foreach (var tappable in futureSelected)
      {
        if (!IsValid(tappable.Key))
        {
          futureSelectedRemove.Add(tappable.Key);
        }
        else if (Time.time - tappable.Value > tappedDelay)
        {
          futureSelectedRemove.Add(tappable.Key);
          SetSelected(tappable.Key);
        }
      }

      foreach (var tappable in futureSelectedRemove)
      {
        futureSelected.Remove(tappable);
      }
      futureSelectedRemove.Clear();
    }

    protected override void OnTriggerExit(ITappable tappable, Collider other)
    {
      if (selectionTimers.ContainsKey(tappable) 
        && Time.time - selectionTimers[tappable] > tapMinTime
        && Time.time - selectionTimers[tappable] < tapTimeout)
      {
        futureSelected[tappable] = Time.time;
      }

      base.OnTriggerExit(tappable, other);
    }

    protected override bool IsValid(ITappable tappable)
    {
      return tappable.IsTappable && base.IsValid(tappable);
    }
  }
}
