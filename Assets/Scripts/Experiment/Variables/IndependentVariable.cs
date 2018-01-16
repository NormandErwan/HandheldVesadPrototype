using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public abstract class IndependentVariable<T> : IIndependentVariable where T : IndependentVariableCondition
  {
    // Editor Fields

    [SerializeField]
    private T[] conditions;

    // Properties

    public SortedList<string, T> Conditions { get; protected set; }
    public T CurrentCondition { get; protected set; }

    // Methods

    public override void NextCondition()
    {
      int requestConditionIndex = (CurrentConditionIndex + 1) % Conditions.Count;
      OnRequestCurrentConditionSync(Id, Conditions.Values[requestConditionIndex].Id);
    }

    internal override void SetCurrentCondition(string currentConditionId)
    {
      CurrentCondition = Conditions[currentConditionId];
      CurrentConditionId = CurrentCondition.Id;
      CurrentConditionIndex = Conditions.IndexOfKey(CurrentCondition.Id);
      OnCurrentConditionUpdated();
    }

    protected virtual void Awake()
    {
      Conditions = new SortedList<string, T>();
      foreach (var condition in conditions)
      {
        Conditions.Add(condition.Id, condition);
      }
      ConditionsCount = Conditions.Count;
      CurrentConditionIndex = 0;
      CurrentConditionId = Conditions.Values[CurrentConditionIndex].Id;
      SetCurrentCondition(CurrentConditionId);
    }
  }
}
