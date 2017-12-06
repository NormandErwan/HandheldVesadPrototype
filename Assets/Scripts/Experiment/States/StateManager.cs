using NormandErwan.MasterThesisExperiment.Experiment.Variables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.States
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

    public event Action<State> RequestCurrentStateSync = delegate { };
    public event Action<State> CurrentStateUpdated = delegate { };

    // Methods

    public void NextState()
    {
      if (CurrentState.id == experimentBeginState.id)
      {
        RequestCurrentStateSync.Invoke(taskBeginState);
      }
      else if (CurrentState.id == taskBeginState.id)
      {
        RequestCurrentStateSync.Invoke(taskTrialState);
      }
      else if (CurrentState.id == taskTrialState.id)
      {
        RequestCurrentStateSync.Invoke((CurrentTrial < TrialsPerCondition) ? taskTrialState : taskEndState);
      }
      else if (CurrentState.id == taskEndState.id)
      {
        if (ConditionsProgress < ConditionsTotal)
        {
          UpdateIndependentVariablesCurrentCondition();
          RequestCurrentStateSync.Invoke(taskBeginState);
        }
        else
        {
          RequestCurrentStateSync.Invoke(experimentEndState);
        }
      }
      else if (CurrentState.id == experimentEndState.id)
      {
        UpdateIndependentVariablesCurrentCondition();
        RequestCurrentStateSync.Invoke(experimentBeginState);
      }
    }

    public T GetIndependentVariable<T>() where T : IIndependentVariable
    {
      foreach (var independentVariable in independentVariables)
      {
        var ivT = independentVariable as T;
        if (ivT != null)
        {
          return ivT;
        }
      }
      return null;
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
        StatesProgress++;
        ConditionsProgress++;
        CurrentTrial = 0;
      }
      else if (CurrentState.id == taskTrialState.id)
      {
        StatesProgress++;
        TrialsProgress++;
        CurrentTrial++;
      }
      else if (CurrentState.id == taskEndState.id)
      {
        StatesProgress++;
      }
      else if (CurrentState.id == experimentEndState.id)
      {
        StatesProgress = StatesTotal;
        ConditionsProgress = ConditionsTotal;
        TrialsProgress = TrialsTotal;
      }
      CurrentStateUpdated.Invoke(CurrentState);
    }

    protected virtual void Awake()
    {
      States = new Dictionary<string, State>()
      {
        { experimentBeginState.id, experimentBeginState },
        { taskBeginState.id, taskBeginState },
        { taskTrialState.id, taskTrialState },
        { taskEndState.id, taskEndState },
        { experimentEndState.id, experimentEndState }
      };
    }

    protected virtual void Start()
    {
      ConditionsTotal = 1;
      foreach (var independentVariable in independentVariables)
      {
        ConditionsTotal *= independentVariable.ConditionsCount;
      }
      TrialsTotal = ConditionsTotal * (int)TrialsPerCondition;
      StatesTotal = 2 // experimentBeginState and experimentEndState
        + 2 * ConditionsTotal // taskBeginState and taskEndState
        + TrialsTotal;
      SetCurrentState(experimentBeginState.id);
    }

    /// <summary>
    /// Update the last IVManager in list each time a task begins, and other IVManagers only if the next in list is on first condition.
    /// </summary>
    protected virtual void UpdateIndependentVariablesCurrentCondition()
    {
      int lastIVIndex = independentVariables.Length - 1;
      bool nextIVRequestedFirstCondition = false;
      for (int index = lastIVIndex; index >= 0; index--)
      {
        if (index == lastIVIndex || nextIVRequestedFirstCondition)
        {
          nextIVRequestedFirstCondition = (independentVariables[index].CurrentConditionIndex == independentVariables[index].ConditionsCount - 1);
          independentVariables[index].NextCondition();
        }
        else if (index != lastIVIndex && !nextIVRequestedFirstCondition)
        {
          break;
        }
      }
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
