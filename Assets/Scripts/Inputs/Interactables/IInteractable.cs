using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface IInteractable
  {
    // Properties

    bool IsInteractable { get; }
    int Priority { get; }
    IInteractable Parent { get; }

    Transform Transform { get; }

    // Events

    event Action<IInteractable> Interactable;

    // Methods

    void SetInteractable(bool value);
  }
}