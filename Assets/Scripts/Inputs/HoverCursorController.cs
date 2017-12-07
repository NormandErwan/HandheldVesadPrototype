using Hover.Core.Cursors;
using NormandErwan.MasterThesisExperiment.Experiment.Task;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  [RequireComponent(typeof(HoverCursorData))]
  [RequireComponent(typeof(Collider))]
  public class HoverCursorController : MonoBehaviour
  {
    // Constants

    public static readonly float longPressTimeout = 0.5f; // in seconds

    // Editor fields

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Properties

    public HoverCursorData HoverCursorData { get; protected set; }

    public bool IsFinger { get { return HoverCursorData.Type != CursorType.Look; } }
    public bool IsIndex { get { return HoverCursorData.Type == CursorType.LeftIndex || HoverCursorData.Type == CursorType.RightIndex; } }
    public bool IsThumb { get { return HoverCursorData.Type == CursorType.LeftThumb || HoverCursorData.Type == CursorType.RightThumb; } }
    public bool IsMiddle { get { return HoverCursorData.Type == CursorType.LeftMiddle || HoverCursorData.Type == CursorType.RightMiddle; } }

    // Variables

    protected Dictionary<Item, float> longPressTimers = new Dictionary<Item, float>();

    // Methods

    protected void Awake()
    {
      HoverCursorData = GetComponent<HoverCursorData>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      if (grid.CurrentMode == Experiment.Task.Grid.Mode.Idle)
      {
        var item = other.GetComponent<Item>();
        if (item != null)
        {
          item.SetFocused(true);

          if (IsFinger)
          {
            longPressTimers[item] = 0;
          }
        }

        var cell = other.GetComponent<Cell>();
        if (cell != null && grid.SelectedItem != null && !grid.SelectedItem.Focused)
        {
          if (cell.Contains(grid.SelectedItem))
          {
            grid.SetSelectedItem(null);
          }
          else
          {
            grid.MoveSelectedItemTo(cell);
          }
        }
      }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      var item = other.GetComponent<Item>();
      if (item != null)
      {
      }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      var item = other.GetComponent<Item>();
      if (item != null)
      {
        item.SetFocused(false);

        if (longPressTimers.ContainsKey(item))
        {
          if (item.Selected && longPressTimers[item] < longPressTimeout)
          {
            grid.SetSelectedItem(null);
          }
          longPressTimers.Remove(item);
        }
      }
    }

    protected virtual void Update()
    {
      if (grid.CurrentMode == Experiment.Task.Grid.Mode.Idle && longPressTimers.Count > 0)
      {
        foreach (var item in new List<Item>(longPressTimers.Keys))
        {
          if (longPressTimers[item] < longPressTimeout)
          {
            longPressTimers[item] += Time.deltaTime;
          }
          else 
          {
            if (!item.Selected)
            {
              grid.SetSelectedItem(item);
            }
            else
            {
              grid.SetSelectedItem(null);
            }
            longPressTimers.Remove(item);
          }
        }
      }
    }
  }
}