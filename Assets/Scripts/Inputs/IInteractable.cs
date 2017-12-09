using System;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public interface IInteractable
  {
    // Properties

    bool IsInteractable { get; }

    // Events

    event Action<IInteractable> Interactable;

    // Methods

    void SetInteractable(bool value);
  }
}