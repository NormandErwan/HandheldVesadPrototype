using System;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IIndependentVariable : Variable
    {
        // Properties

        public int ConditionsCount { get; protected set; }
        public int CurrentConditionIndex { get; protected set; }
        public string CurrentConditionId { get; protected set; }

        // Events

        public Action<string, string> RequestCurrentConditionSync = delegate { };

        // Methods

        public abstract void NextCondition();
        internal abstract void SetCondition(string currentConditionId);
    }
}
