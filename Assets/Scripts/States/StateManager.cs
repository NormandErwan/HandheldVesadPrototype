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
        public IIndependentVariable[] independentVariables;

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
                if (ConditionsProgress > 1) // Don't update the IV for the first serie of trials
                {
                    // Update last IVManager in list each task begin, and other managers only if the next in list is on first condition
                    int lastIndex = independentVariables.Length - 1;
                    for (int index = lastIndex; index >= 0; index--)
                    {
                        int nextIndex = index + 1;
                        if (nextIndex == lastIndex && independentVariables[lastIndex].CurrentConditionIndex != 0)
                        {
                            break;
                        }
                        else if (index == lastIndex || independentVariables[nextIndex].CurrentConditionIndex == 0)
                        {
                            independentVariables[index].NextCondition();
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
            foreach (var independentVariable in independentVariables)
            {
                ConditionsTotal *= independentVariable.ConditionsCount;
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
