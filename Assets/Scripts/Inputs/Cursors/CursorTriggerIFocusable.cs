using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class CursorTriggerIFocusable : CursorTriggerIInteractable<IFocusable>
  {
    protected override void OnTriggerEnter(IFocusable focusable, Collider other)
    {
      if (focusable.IsInteractable)
      {
        focusable.SetFocused(true);
      }
    }

    protected override void OnTriggerStay(IFocusable focusable, Collider other)
    {
    }

    protected override void OnTriggerExit(IFocusable focusable, Collider other)
    {
      if (focusable.IsInteractable)
      {
        focusable.SetFocused(false);
      }
    }
  }
}
