using System;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ITappable : ISelectable
  {
    // Properties

    bool IsTappable { get; }

    // Events

    event Action<ITappable> Tappable;

    // Methods

    void SetTappable(bool value);
  }
}