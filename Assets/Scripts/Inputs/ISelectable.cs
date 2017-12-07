using System;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface ISelectable
  {
    // Properties

    bool IsSelected { get; }

    // Events

    event Action<ISelectable> Selected;

    // Methods

    void SetSelected(bool value);
  }
}