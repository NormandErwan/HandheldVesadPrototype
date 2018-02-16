using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class FingerCursorTriggerISelectable<T> : CursorTriggerIInteractable<T, FingerCursor> where T : ISelectable
  {
    // Constants

    public const float defaultMaxCursorDistance = 0.002f; // in meters

    // Variables

    protected Dictionary<T, float> selectionTimers = new Dictionary<T, float>();
    protected Dictionary<T, Vector3> cursorEnterPositions = new Dictionary<T, Vector3>();
    private SortedDictionary<int, List<T>> selected = new SortedDictionary<int, List<T>>(new DescendingComparer<int>());

    // Methods

    public override void ProcessPriorityLists()
    {
      base.ProcessPriorityLists();
      ProcessSelected();
    }

    protected override void OnTriggerEnter(T selectable, Collider other)
    {
      if (!IsValid(selectable))
      {
        ClearCursorTimer(selectable);
      }
      else if (!selectionTimers.ContainsKey(selectable))
      {
        selectionTimers.Add(selectable, Time.time);
        cursorEnterPositions.Add(selectable, Project(selectable, Cursor.transform.position));
      }
    }

    protected override void OnTriggerStay(T selectable, Collider other)
    {
      OnTriggerEnter(selectable, other);
    }

    protected override void OnTriggerExit(T selectable, Collider other)
    {
      ClearCursorTimer(selectable);
    }

    protected virtual bool IsValid(T selectable)
    {
      bool valid = selectable.IsInteractable && selectable.IsSelectable;
      if (cursorEnterPositions.ContainsKey(selectable))
      {
        var cursorDistance = (Project(selectable, Cursor.transform.position) - cursorEnterPositions[selectable]).magnitude;
        valid = valid && (cursorDistance < Cursor.MaxSelectableDistance);
      }
      return valid;
    }

    protected void ClearCursorTimer(T selectable)
    {
      if (selectionTimers.ContainsKey(selectable))
      {
        selectionTimers.Remove(selectable);
        cursorEnterPositions.Remove(selectable);
      }
    }

    protected void SetSelected(T selectable)
    {
      ClearCursorTimer(selectable);

      if (!selected.ContainsKey(selectable.Priority))
      {
        selected.Add(selectable.Priority, new List<T>());
      }
      selected[selectable.Priority].Add(selectable);
    }

    protected void ProcessSelected()
    {
      bool hasFirstSelected = false;
      T firstSelected = default(T);

      foreach (var selectedPriority in selected)
      {
        foreach (var selectable in selectedPriority.Value)
        {
          if (IsValid(selectable))
          {
            hasFirstSelected = true;
            firstSelected = selectable;
            break;
          }
        }

        if (hasFirstSelected)
        {
          break;
        }
      }

      foreach (var tappedPriority in selected)
      {
        tappedPriority.Value.Clear();
      }

      if (hasFirstSelected)
      {
        firstSelected.SetSelected(!firstSelected.IsSelected);
      }
    }
  }
}
