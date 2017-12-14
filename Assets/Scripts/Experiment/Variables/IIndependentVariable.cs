using System;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  /// <summary>
  /// Serializable version of IndependentVariable, to be used in Unity inspector.
  /// </summary>
  public abstract class IIndependentVariable : Variable
  {
    // Properties

    public int ConditionsCount { get; protected set; }
    public string CurrentConditionId { get; protected set; }
    public int CurrentConditionIndex { get; protected set; }

    // Events

    public event Action<string, string> CurrentConditionSync = delegate { };
    public event Action CurrentConditionUpdated = delegate { };

    // Methods

    public abstract void NextCondition();
    internal abstract void SetCurrentCondition(string currentConditionId);

    protected virtual void OnRequestCurrentConditionSync(string independentVariableId, string currentConditionId)
    {
      CurrentConditionSync.Invoke(independentVariableId, currentConditionId);
    }

    protected virtual void OnCurrentConditionUpdated()
    {
      CurrentConditionUpdated.Invoke();
    }
  }
}
