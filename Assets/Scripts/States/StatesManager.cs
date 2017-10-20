using NormandErwan.MasterThesisExperiment.Variables;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.States
{
    // TODO: supports State[] in editor fields
    // TODO: supports that there is no states in editor fields (e.g. supports only states for trials)
    // TOOD: supports multiple tasks by creating a TaskManager and add in editor fields a TaskManager[]
    public class StatesManager : MonoBehaviour
    {
        // Editor fields

        [Header("Experiment States")]
        public State experimentBeginState;
        public State experimentEndState;

        [Header("Task States")]
        public State taskBeginState;
        public State taskTrialState;
        public uint TrialsPerCondition = 1;
        public State taskEndState;

        [Header("Independent Variables")]
        public IIndependentVariableManager[] independentVariableManagers;

        // Properties

        public State CurrentState { get; protected set; }
        public int StatesTotal { get; protected set; }
        public int StatesProgress { get; protected set; }

        public int ConditionsTotal { get; protected set; }
        public int ConditionsProgress { get; protected set; }

        public int CurrentTrial { get; protected set; }
        public int TrialsTotal { get; protected set; }
        public int TrialsProgress { get; protected set; }

        // Methods

        public void NextState()
        {
            if (CurrentState == experimentBeginState 
                || (CurrentState == taskEndState && ConditionsProgress < ConditionsTotal))
            {
                CurrentState = taskBeginState;
                ConditionsProgress++;
                StatesProgress++;

                if (ConditionsProgress > 1)
                {
                    int lastIndeVarId = independentVariableManagers.Length - 1;
                    for (int indeVarId = lastIndeVarId; indeVarId >= 0; indeVarId--)
                    {
                        int nextIndeVarId = indeVarId + 1;
                        if (nextIndeVarId == lastIndeVarId && independentVariableManagers[lastIndeVarId].CurrentConditionId != 0)
                        {
                            break;
                        }
                        else if (indeVarId == lastIndeVarId || independentVariableManagers[nextIndeVarId].CurrentConditionId == 0)
                        {
                            independentVariableManagers[indeVarId].NextCondition();
                        }
                    }
                }
            }
            else if (CurrentState == taskBeginState)
            {
                CurrentState = taskTrialState;
                TrialsProgress++;
                CurrentTrial++;
                StatesProgress++;
            }
            else if (CurrentState == taskTrialState)
            {
                if (CurrentTrial < TrialsPerCondition)
                {
                    TrialsProgress++;
                    CurrentTrial++;
                }
                else
                {
                    CurrentState = taskEndState;
                    CurrentTrial = 0;
                }
                StatesProgress++;
            }
            else if (CurrentState == taskEndState)
            {
                CurrentState = experimentEndState;
                StatesProgress = StatesTotal;
            }
            else if (CurrentState == experimentEndState)
            {
                ResetCurrentState();
            }
        }

        public override string ToString()
        {
            return "StatesManager: [CurrentState: '" + CurrentState.title 
                + "', ConditionsProgress: " + ConditionsProgress + "/" + ConditionsTotal
                + ", TrialsProgress: " + TrialsProgress + "/" + TrialsTotal 
                + " (current trial: " + CurrentTrial + "/" + TrialsPerCondition + ")"
                + ", Overall progress: " + (StatesProgress * 100f / StatesTotal).ToString("F1") + "%]";
        }

        protected virtual void Awake()
        {
            ConditionsTotal = 1;
            foreach (var independentVariableManager in independentVariableManagers)
            {
                ConditionsTotal *= independentVariableManager.ConditionsCount;
            }
            TrialsTotal = ConditionsTotal * (int)TrialsPerCondition;
            StatesTotal = 2 // experimentBeginState and experimentEndState
                + 2 * ConditionsTotal // taskBeginState and taskEndState
                + TrialsTotal;

            ResetCurrentState();
        }

        protected virtual void ResetCurrentState()
        {
            CurrentState = experimentBeginState;
            CurrentTrial = 0;
            StatesProgress = 0;
            ConditionsProgress = 0;
            TrialsProgress = 0;
        }
    }
}
