namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IndependentVariableManager<T> : IIndependentVariableManager where T : IndependantVariable
    {
        // Editor Fields

        public T[] conditions;

        // Properties

        public override int ConditionsCount { get { return conditions.Length; } }

        public override int CurrentConditionId { get { return currentConditionId; } }

        public T CurrentCondition { get { return conditions[(uint)currentConditionId]; } }

        // Variables

        protected int currentConditionId = 0;

        // Methods

        public override void NextCondition()
        {
            currentConditionId = (currentConditionId + 1) % ConditionsCount;
        }
    }
}
