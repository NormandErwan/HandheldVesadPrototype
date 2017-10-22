using NormandErwan.MasterThesisExperiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public class IVClassificationDistanceCondition : IndependentVariableCondition
    {
        // Editor Fields

        [SerializeField]
        private float minimum;

        [SerializeField]
        private float maximum;

        // Properties

        public Range<float> Range { get; protected set; }

        // Methods

        protected virtual void Awake()
        {
            Range = new Range<float>(minimum, maximum);
        }
    }
}
