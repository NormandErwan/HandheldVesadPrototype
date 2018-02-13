using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public interface ICursorTriggerIInteractable
  {
    Cursor Cursor { get; set; }

    void OnTrigger(TriggerType triggerType, Collider other);
    void ProcessPriorityLists();
  }
}
