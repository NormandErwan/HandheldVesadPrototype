using NormandErwan.MasterThesisExperiment.Variables;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.States
{
    public class StatesManager : MonoBehaviour
    {
        // Editor fields

        [Header("Experiment States")]
        public State[] experimentBeginStates;
        public State[] experimentEndStates;

        [Header("Task States")]
        public State[] taskBeginStates;
        public State[] taskTrialStates;
        public int TrialsPerTask = 1;
        public State[] taskEndStates;

        [Header("Independent Variables")]
        public IIndependentVariableManager[] independentVariableManagers;

        // Properties

        public State CurrentState { get; protected set; }
        public int TaskTrialsTotal { get; protected set; }
        public int TaskTrialsProgress { get; protected set; }

        public int CurrentTrial { get; protected set; }

        // Methods

        public State NextState()
        {
            return CurrentState;
        }

        protected virtual void Awake()
        {
            CurrentState = experimentBeginStates[0];

            TaskTrialsTotal = 1;
            foreach (var IVManager in independentVariableManagers)
            {
                TaskTrialsTotal *= IVManager.ConditionsCount;
            }
            TaskTrialsTotal *= TrialsPerTask;

            TaskTrialsProgress = 0;
            CurrentTrial = 0;
        }
    }
}
