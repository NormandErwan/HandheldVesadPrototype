using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class CursorTriggerIFocusable : CursorTriggerIInteractable<IFocusable, BaseCursor>
  {
    protected override void OnTriggerEnter(IFocusable focusable, Collider other)
    {
      if (focusable.IsInteractable && !focusable.IsFocused)
      {
        focusable.SetFocused(true);
      }
    }

    protected override void OnTriggerStay(IFocusable focusable, Collider other)
    {
      OnTriggerEnter(focusable, other);
    }

    protected override void OnTriggerExit(IFocusable focusable, Collider other)
    {
      if (focusable.IsInteractable && focusable.IsFocused)
      {
        focusable.SetFocused(false);
      }
    }
  }
}
