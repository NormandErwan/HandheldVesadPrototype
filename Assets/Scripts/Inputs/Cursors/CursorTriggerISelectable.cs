using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class CursorTriggerISelectable<T> : CursorTriggerIInteractable<T> where T : ISelectable
  {
    protected bool CancelTimer(ISelectable selectable)
    {
      bool cancelTimer = false;

      IInteractable latestInteractable = selectable;
      do
      {
        var transformable = latestInteractable as ITransformable;
        if (transformable != null && transformable.IsTransforming)
        {
          cancelTimer = true;
          break;
        }
        latestInteractable = latestInteractable.Parent;
      } while (latestInteractable != null);

      return cancelTimer;
    }
  }
}
