using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerITappable : FingerCursorTriggerISelectable<ITappable>
  {
    // Constants

    public static readonly float tapTimeout = 0.5f; // in seconds

    // Variables

    protected Dictionary<ITappable, float> tapTimers = new Dictionary<ITappable, float>();
    protected SortedDictionary<int, List<ITappable>> tapped = new SortedDictionary<int, List<ITappable>>(new DescendingComparer<int>());

    // Methods

    public override void ProcessPriorityLists()
    {
      base.ProcessPriorityLists();
      TryTriggerTapped();
    }

    protected override void OnTriggerEnter(ITappable tappable, Collider other)
    {
      if (tappable.IsInteractable && tappable.IsSelectable && tappable.IsTappable)
      {
        tapTimers.Add(tappable, Time.time);
      }
    }

    protected override void OnTriggerStay(ITappable tappable, Collider other)
    {
      if (tapTimers.ContainsKey(tappable))
      {
        if (CancelTimer(tappable) || Time.time - tapTimers[tappable] > tapTimeout)
        {
          tapTimers.Remove(tappable);
        }
        else
        {
          tapTimers[tappable] += Time.deltaTime;
        }
      }
    }

    protected override void OnTriggerExit(ITappable tappable, Collider other)
    {
      if (tapTimers.ContainsKey(tappable))
      {
        if (!CancelTimer(tappable) && Time.time - tapTimers[tappable] < tapTimeout)
        {
          if (!tapped.ContainsKey(tappable.Priority))
          {
            tapped.Add(tappable.Priority, new List<ITappable>());
          }
          tapped[tappable.Priority].Add(tappable);
        }
        tapTimers.Remove(tappable);
      }
    }

    protected void TryTriggerTapped()
    {
      ITappable firstTappable = null;

      foreach (var tappedPriority in tapped)
      {
        foreach (var tappable in tappedPriority.Value)
        {
          if (tappable.IsInteractable && tappable.IsSelectable)
          {
            firstTappable = tappable;
            break;
          }
        }

        if (firstTappable != null)
        {
          break;
        }
      }

      foreach (var tappedPriority in tapped)
      {
        tappedPriority.Value.Clear();
      }

      if (firstTappable != null)
      {
        firstTappable.SetSelected(!firstTappable.IsSelected);
      }
    }
  }
}
