using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public class IVTechniqueCondition : IndependentVariableCondition
  {
    // Editor fields

    [TextArea]
    public string instructions;

    public bool useLeapInput;

    public bool useTouchInput;
  }
}
