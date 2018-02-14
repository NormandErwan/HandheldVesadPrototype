using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public interface ICursorTriggerIInteractable
  {
    BaseCursor Cursor { get; }

    void OnTrigger(TriggerType triggerType, Collider other);
    void ProcessPriorityLists();
  }
}
