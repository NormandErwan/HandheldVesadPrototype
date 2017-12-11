using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public class IVClassificationDifficultyCondition : IndependentVariableCondition
  {
    // Editor Fields

    [SerializeField]
    private float minimumAverageClassificationDistance;

    [SerializeField]
    private float maximumAverageClassificationDistance;

    [SerializeField]
    private int numberOfItemsToClass;

    // Properties

    public Range<float> AverageClassificationDistanceRange { get; protected set; }

    public int NumberOfItemsToClass { get { return numberOfItemsToClass; } protected set { numberOfItemsToClass = value; } }

    // Methods

    protected virtual void Awake()
    {
      AverageClassificationDistanceRange = new Range<float>(minimumAverageClassificationDistance, maximumAverageClassificationDistance);
    }
  }
}
