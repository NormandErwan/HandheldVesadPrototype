using System;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ILongPressable : ISelectable
  {
    // Properties

    bool IsLongPressable { get; }

    // Events

    event Action<ILongPressable> LongPressable;

    // Methods

    void SetLongPressable(bool value);
  }
}