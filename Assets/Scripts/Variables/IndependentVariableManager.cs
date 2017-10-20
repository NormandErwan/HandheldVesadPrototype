using System;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IndependentVariableManager<T> : IIndependentVariableManager where T : IndependentVariable
    {
        // Editor Fields

        public T[] conditions;

        // Properties

        public T CurrentCondition { get; protected set; }
        public override int ConditionsCount { get { return conditions.Length; } }
        public override int CurrentConditionIndex
        {
            get { return currentConditionIndex; }
            internal set
            {
                currentConditionIndex = value;
                CurrentCondition = conditions[currentConditionIndex];
                CurrentConditionUpdated(CurrentCondition);
            }
        }

        // Events

        public Action<T> CurrentConditionUpdated = delegate { };

        // Variables

        private int currentConditionIndex;

        // Methods

        void Start()
        {
            // TODO: remove
            CurrentConditionUpdated += (currentcondtiion) =>
            {
                print(typeof(T) + " updated : " + CurrentCondition.title);
            };
        }

        public override void NextCondition()
        {
            CurrentConditionIndex = (CurrentConditionIndex + 1) % ConditionsCount;
            RequestCurrentConditionSync(id);
        }
    }
}
