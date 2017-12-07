using System;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface IFocusable
  {
    // Properties

    bool IsFocused { get; }

    // Events

    event Action<IFocusable> Focused;
    
    // Methods

    void SetFocused(bool value);
  }
}