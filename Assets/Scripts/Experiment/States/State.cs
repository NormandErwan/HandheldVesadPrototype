using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.States
{
  public class State : Variable
  {
    // Editor fields

    [SerializeField]
    [TextArea]
    private string instructions;

    [SerializeField]
    private bool activateTask;

    // Properties

    public string Instructions { get { return instructions; } set { instructions = value; } }
    public bool ActivateTask { get { return activateTask; } set { activateTask = value; } }
  }
}
