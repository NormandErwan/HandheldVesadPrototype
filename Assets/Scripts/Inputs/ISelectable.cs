using System;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public interface ISelectable : IInteractable
  {
    // Properties

    bool IsSelectable { get; }
    bool IsSelected { get; }

    // Events

    event Action<ISelectable> Selectable;
    event Action<ISelectable> Selected;

    // Methods

    void SetSelectable(bool value);
    void SetSelected(bool value);
  }
}