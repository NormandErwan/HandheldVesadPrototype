using System;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IIndependentVariableManager : MonoBehaviour
    {
        // Editor Fields

        public int id;

        // Properties

        public abstract int ConditionsCount { get; }
        public abstract int CurrentConditionIndex { get; internal set; }

        // Events

        public Action<int> RequestCurrentConditionSync = delegate { };

        // Methods

        public abstract void NextCondition();
    }
}
