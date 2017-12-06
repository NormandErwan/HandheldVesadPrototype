using Hover.Core.Cursors;
using NormandErwan.MasterThesisExperiment.Experiment.Task;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Utilities
{
  [RequireComponent(typeof(HoverCursorData))]
  [RequireComponent(typeof(Collider))]
  public class HoverCursorController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Variables

    protected HoverCursorData hoverCursorData;

    // Methods

    protected void Awake()
    {
      hoverCursorData = GetComponent<HoverCursorData>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      if (IsIndex())
      {
        var item = other.GetComponent<Item>();
        if (item != null)
        {
          item.ToggleFocused();
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
            grid.MoveCurrentItemSelected(cell);
          }
        }
      }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      if (IsIndex())
      {
        var item = other.GetComponent<Item>();
        if (item != null)
        {
          item.ToggleFocused();

          if (!item.CorrectlyClassified)
          {
            grid.SetSelectedItem(item);
          }
        }
      }
    }

    protected virtual bool IsIndex()
    {
      return hoverCursorData.Type == CursorType.LeftIndex || hoverCursorData.Type == CursorType.RightIndex;
    }
  }
}