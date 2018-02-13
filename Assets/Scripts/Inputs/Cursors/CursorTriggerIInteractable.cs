using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class CursorTriggerIInteractable<T> : ICursorTriggerIInteractable
    where T : IInteractable
  {
    // Properties

    public Cursor Cursor { get; set; }

    // Methods

    public void OnTrigger(TriggerType triggerType, Collider other)
    {
      var interactable = other.GetComponent<T>();
      if (interactable != null)
      {
        if (triggerType == TriggerType.Enter)
        {
          OnTriggerEnter(interactable, other);
        }
        else if (triggerType == TriggerType.Stay)
        {
          OnTriggerStay(interactable, other);
        }
        else if (triggerType == TriggerType.Exit)
        {
          OnTriggerExit(interactable, other);
        }
      }
    }

    public virtual void ProcessPriorityLists()
    {
    }

    protected abstract void OnTriggerEnter(T interactable, Collider other);
    protected abstract void OnTriggerStay(T interactable, Collider other);
    protected abstract void OnTriggerExit(T interactable, Collider other);
  }
}
