using System;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface ISelectable : IInteractable
  {
    // Properties

    bool IsSelected { get; }

    // Events

    event Action<ISelectable> Selected;

    // Methods

    void SetSelected(bool value);
  }
}