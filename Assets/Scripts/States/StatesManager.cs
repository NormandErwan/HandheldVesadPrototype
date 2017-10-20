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

        public int TasksTotal { get; protected set; }
        public int TasksProgress { get; protected set; }

        public int TrialsTotal { get; protected set; }
        public int TrialsProgress { get; protected set; }
        public int CurrentTrial { get; protected set; }

        // Methods

        public State NextState()
        {
            return CurrentState;
        }

        protected virtual void Awake()
        {
            CurrentState = experimentBeginStates[0];

            TasksTotal = 1;
            foreach (var IVManager in independentVariableManagers)
            {
                TasksTotal *= IVManager.ConditionsCount;
            }
            TrialsTotal = TasksTotal * TrialsPerTask;

            TasksProgress = 0;
            TrialsProgress = 0;
            CurrentTrial = 0;
        }
    }
}
