using NormandErwan.MasterThesisExperiment.Variables;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.States
{
    // TODO: supports State[] in editor fields
    // TODO: supports that there is no states in editor fields (e.g. supports only states for trials)
    // TOOD: supports multiple tasks by creating a TaskManager and add in editor fields a TaskManager[]
    public class StateManager : MonoBehaviour
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

        public State CurrentState { get { return currentState; } internal set { currentState = value; CurrentStateUpdated.Invoke(currentState); } }
        public int CurrentTrial { get; internal set; }

        public int StatesTotal { get; internal set; }
        public int StatesProgress { get; internal set; }

        public int ConditionsTotal { get; internal set; }
        public int ConditionsProgress { get; internal set; }

        public int TrialsTotal { get; internal set; }
        public int TrialsProgress { get; internal set; }

        // Events

        public Action<State> RequestCurrentStateSync = delegate { };
        public Action<State> CurrentStateUpdated = delegate { };

        // Variables

        private State currentState;

        // Methods

        public void NextState()
        {
            if (CurrentState.id == experimentBeginState.id 
                || (CurrentState.id == taskEndState.id && ConditionsProgress < ConditionsTotal))
            {
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

                ConditionsProgress++;
                StatesProgress++;
                RequestCurrentStateSync.Invoke(taskBeginState);
            }
            else if (CurrentState.id == taskBeginState.id)
            {
                TrialsProgress++;
                CurrentTrial++;
                StatesProgress++;
                RequestCurrentStateSync.Invoke(taskTrialState);
            }
            else if (CurrentState.id == taskTrialState.id)
            {
                StatesProgress++;
                if (CurrentTrial < TrialsPerCondition)
                {
                    TrialsProgress++;
                    CurrentTrial++;
                }
                else
                {
                    CurrentTrial = 0;
                    RequestCurrentStateSync.Invoke(taskEndState);
                }
            }
            else if (CurrentState.id == taskEndState.id)
            {
                StatesProgress = StatesTotal;
                RequestCurrentStateSync.Invoke(experimentEndState);
            }
            else if (CurrentState.id == experimentEndState.id)
            {
                ResetProgress();
                RequestCurrentStateSync.Invoke(experimentBeginState);
            }
        }

        public override string ToString()
        {
            return "StatesManager: [CurrentState: '" + CurrentState.id 
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

            ResetProgress();
            CurrentState = experimentBeginState;
        }

        protected virtual void ResetProgress()
        {
            CurrentTrial = 0;
            StatesProgress = 0;
            ConditionsProgress = 0;
            TrialsProgress = 0;
        }
    }
}
