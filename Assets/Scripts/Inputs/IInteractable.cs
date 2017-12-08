using System;

namespace NormandErwan.MasterThesisExperiment.Inputs
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