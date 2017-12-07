using System;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface IDraggable : IInteractable
  {
    // Properties

    bool IsDragging { get; }
    float DistanceToStartDragging { get; }

    // Events

    event Action<IDraggable> DraggingStarted;
    event Action<IDraggable> Dragging;
    event Action<IDraggable> DraggingStopped;

    // Methods

    void SetDragging(bool value);
    void Drag(Vector3 movement);
  }
}