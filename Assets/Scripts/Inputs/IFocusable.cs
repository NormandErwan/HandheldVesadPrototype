using System;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface IFocusable : IInteractable
  {
    // Properties

    bool IsFocused { get; }

    // Events

    event Action<IFocusable> Focused;
    
    // Methods

    void SetFocused(bool value);
  }
}