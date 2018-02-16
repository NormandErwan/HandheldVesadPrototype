using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class CursorTriggerIInteractable<T, U> : ICursorTriggerIInteractable
    where T : IInteractable
    where U : BaseCursor
  {
    // Properties

    public U Cursor { get; set; }
    BaseCursor ICursorTriggerIInteractable.Cursor { get { return Cursor; } }

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

    protected Vector3 Project(IInteractable interactable, Vector3 position)
    {
      float distanceToGrid = Vector3.Dot(position - interactable.Transform.position, -interactable.Transform.forward);
      var projection = position + distanceToGrid * interactable.Transform.forward;
      return Quaternion.Inverse(interactable.Transform.rotation) * projection;
    }
  }
}
