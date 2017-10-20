using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IIndependentVariableManager : MonoBehaviour
    {
        // Properties

        public abstract int ConditionsCount { get; }

        // Methods

        public abstract void NextCondition();
    }
}
