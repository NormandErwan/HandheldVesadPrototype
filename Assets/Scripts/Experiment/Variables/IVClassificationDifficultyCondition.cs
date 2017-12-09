using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public class IVClassificationDifficultyCondition : IndependentVariableCondition
  {
    // Editor Fields

    [SerializeField]
    private float minimum;

    [SerializeField]
    private float maximum;

    [SerializeField]
    [Range(0f, 1f)]
    private float incorrectlyClassifiedCellsFraction = 0.5f;

    // Properties

    public Range<float> Range { get; protected set; }

    public float IncorrectlyClassifiedContainersFraction { get { return incorrectlyClassifiedCellsFraction; } protected set { incorrectlyClassifiedCellsFraction = value; } }

    // Methods

    protected virtual void Awake()
    {
      Range = new Range<float>(minimum, maximum);
    }
  }
}
