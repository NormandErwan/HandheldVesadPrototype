using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IndependantVariableManager<T> : MonoBehaviour where T : IndependantVariable
    {
        // Editor Fields

        public T[] conditions;

        // Properties

        public T CurrentCondition { get; protected set; }

        // Variables

        public static int currentConditionId = -1;

        // Methods

        public T NextCondition()
        {
            currentConditionId = (currentConditionId + 1) % conditions.Length;
            CurrentCondition = conditions[(uint)currentConditionId];
            return CurrentCondition;
        }
    }
}
