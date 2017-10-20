namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IndependentVariableManager<T> : IIndependentVariableManager where T : IndependantVariable
    {
        // Editor Fields

        public T[] conditions;

        // Properties

        public override int ConditionsCount { get { return conditions.Length; } }

        public T CurrentCondition { get; protected set; }

        // Variables

        protected int currentConditionId = -1;

        // Methods

        public override void NextCondition()
        {
            currentConditionId = (currentConditionId + 1) % ConditionsCount;
            CurrentCondition = conditions[(uint)currentConditionId];
        }
    }
}
