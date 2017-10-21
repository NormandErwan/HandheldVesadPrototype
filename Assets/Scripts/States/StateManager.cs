using NormandErwan.MasterThesisExperiment.Variables;
using System;
using System.Collections.Generic;
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

        public Dictionary<string, State> States { get; protected set; }
        public State CurrentState { get; protected set; }
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

        // Methods

        public void NextState()
        {
            if (CurrentState.id == experimentBeginState.id)
            {
                RequestCurrentStateSync.Invoke(taskBeginState);
            }
            else if (CurrentState.id == taskBeginState.id)
            {
                if (ConditionsProgress > 1)
                {
                    int lastIndeVarId = independentVariableManagers.Length - 1;
                    for (int indeVarId = lastIndeVarId; indeVarId >= 0; indeVarId--)
                    {
                        int nextIndeVarId = indeVarId + 1;
                        if (nextIndeVarId == lastIndeVarId && independentVariableManagers[lastIndeVarId].CurrentConditionIndex != 0)
                        {
                            break;
                        }
                        else if (indeVarId == lastIndeVarId || independentVariableManagers[nextIndeVarId].CurrentConditionIndex == 0)
                        {
                            independentVariableManagers[indeVarId].NextCondition();
                        }
                    }
                }
                RequestCurrentStateSync.Invoke(taskTrialState);
            }
            else if (CurrentState.id == taskTrialState.id)
            {
                RequestCurrentStateSync.Invoke((CurrentTrial < TrialsPerCondition) ? taskTrialState : taskEndState);
            }
            else if (CurrentState.id == taskEndState.id)
            {
                RequestCurrentStateSync.Invoke((ConditionsProgress < ConditionsTotal) ? taskBeginState : experimentEndState);
            }
            else if (CurrentState.id == experimentEndState.id)
            {
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

        internal virtual void SetCurrentState(string currentStateId)
        {
            CurrentState = States[currentStateId];
            if (CurrentState.id == experimentBeginState.id)
            {
                ResetProgress();
            }
            else if (CurrentState.id == taskBeginState.id)
            {
                ConditionsProgress++;
                CurrentTrial = 0;
                StatesProgress++;
            }
            else if (CurrentState.id == taskTrialState.id)
            {
                TrialsProgress++;
                CurrentTrial++;
                StatesProgress++;
            }
            else if (CurrentState.id == taskEndState.id)
            {
                StatesProgress++;
            }
            else if (CurrentState.id == experimentEndState.id)
            {
                StatesProgress = StatesTotal;
                RequestCurrentStateSync.Invoke(experimentEndState);
            }
            CurrentStateUpdated.Invoke(CurrentState);
        }

        // TODO: remove
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextState();
            }
        }

        protected virtual void Awake()
        {
            // TODO: remove
            CurrentStateUpdated += (state) =>
            {
                print("StateManager " + ToString());
            };

            ConditionsTotal = 1;
            foreach (var independentVariableManager in independentVariableManagers)
            {
                ConditionsTotal *= independentVariableManager.ConditionsCount;
            }
            TrialsTotal = ConditionsTotal * (int)TrialsPerCondition;
            StatesTotal = 2 // experimentBeginState and experimentEndState
                + 2 * ConditionsTotal // taskBeginState and taskEndState
                + TrialsTotal;

            States = new Dictionary<string, State>()
            {
                { experimentBeginState.id, experimentBeginState },
                { taskBeginState.id, taskBeginState },
                { taskTrialState.id, taskTrialState },
                { taskEndState.id, taskEndState },
                { experimentEndState.id, experimentEndState }
            };

            SetCurrentState(experimentBeginState.id);
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
