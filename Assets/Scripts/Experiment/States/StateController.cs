using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.States
{
  // TODO: supports State[] in editor fields
  // TODO: supports that there is no states in editor fields (e.g. supports only states for trials)
  // TOOD: supports multiple tasks by creating a TaskManager and add in editor fields a TaskManager[]
  public class StateController : MonoBehaviour
  {
    // Editor fields

    [Header("Experiment States")]
    public State experimentBeginState;
    public State restState;
    public State experimentEndState;

    [Header("Task States")]
    public State taskTrainingState;
    public State taskTrialState;
    public uint TrialsPerCondition = 1;

    [Header("Independent Variables")]
    public IIndependentVariable[] independentVariables;

    // Properties

    public Dictionary<string, State> States { get; protected set; }
    public State CurrentState { get; protected set; }
    public State PreviousState { get; protected set; }
    public int CurrentTrial { get; internal set; }

    public int StatesTotal { get; internal set; }
    public int StatesProgress { get; internal set; }

    public int ConditionsTotal { get; internal set; }
    public int ConditionsProgress { get; internal set; }

    public int TrialsTotal { get; internal set; }
    public int TrialsProgress { get; internal set; }

    // Events

    public event Action<State> CurrentStateSync = delegate { };
    public event Action<State> CurrentStateUpdated = delegate { };

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      States = new Dictionary<string, State>()
      {
        { experimentBeginState.id, experimentBeginState },
        { taskTrainingState.id, taskTrainingState },
        { taskTrialState.id, taskTrialState },
        { restState.id, restState },
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
        + independentVariables[0].ConditionsCount // trainings
        + TrialsTotal; // trials
    }

    // Methods

    public void BeginExperiment()
    {
      CurrentStateSync(experimentBeginState);
    }

    public void NextState()
    {
      if (CurrentState.id == experimentBeginState.id)
      {
        CurrentStateSync(taskTrainingState);
      }
      else if (CurrentState.id == taskTrainingState.id)
      {
        CurrentStateSync(taskTrialState);
      }
      else if (CurrentState.id == taskTrialState.id)
      {
        var nextState = taskTrialState;
        if (CurrentTrial == TrialsPerCondition)
        {
          if (ConditionsProgress == ConditionsTotal)
          {
            nextState = experimentEndState;
          }
          else
          {
            var previousTechniqueConditionIndex = independentVariables[0].CurrentConditionIndex;
            NextCondition();

            // Rest+Training only when there is a new technique
            if (independentVariables[0].CurrentConditionIndex != previousTechniqueConditionIndex)
            {
              nextState = restState;
            }
          }
        }
        CurrentStateSync(nextState);
      }
      else if (CurrentState.id == restState.id)
      {
        CurrentStateSync(taskTrainingState);
      }
      else if (CurrentState.id == experimentEndState.id)
      {
        NextCondition();
        CurrentStateSync(experimentBeginState);
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
      return name + ": CurrentState: '" + CurrentState.id
        + "', ConditionsProgress: " + ConditionsProgress + "/" + ConditionsTotal
        + ", TrialsProgress: " + TrialsProgress + "/" + TrialsTotal
        + " (current trial: " + CurrentTrial + "/" + TrialsPerCondition + ")"
        + ", Overall progress: " + (StatesProgress * 100f / StatesTotal).ToString("F1") + "%";
    }

    internal virtual void SetCurrentState(string currentStateId)
    {
      PreviousState = CurrentState;
      CurrentState = States[currentStateId];
      if (CurrentState.id == experimentBeginState.id)
      {
        StatesProgress = 0;
        ConditionsProgress = 1;
        TrialsProgress = 1;
        CurrentTrial = 1;
      }
      else if (CurrentState.id == taskTrainingState.id)
      {
        StatesProgress++;

        if (PreviousState.id == restState.id)
        {
          ConditionsProgress++;
          TrialsProgress++;
          CurrentTrial = 1;
        }
      }
      else if (CurrentState.id == taskTrialState.id)
      {
        StatesProgress++;

        if (CurrentTrial == TrialsPerCondition)
        {
          ConditionsProgress++;
          TrialsProgress++;
          CurrentTrial = 1;
        }
        else if (PreviousState.id != taskTrainingState.id)
        {
          TrialsProgress++;
          CurrentTrial++;
        }
      }
      else if (CurrentState.id == restState.id)
      {
        StatesProgress++;
      }
      else if (CurrentState.id == experimentEndState.id)
      {
        StatesProgress++;
      }
      CurrentStateUpdated(CurrentState);
    }

    /// <summary>
    /// Update the last IVManager in list each time a task begins, and other IVManagers only if the next in list is on first condition.
    /// </summary>
    protected virtual void NextCondition()
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
  }
}
