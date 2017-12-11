using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface IDraggable : ITransformable
  {
    // Properties

    bool IsDragging { get; }

    // Events

    event Action<IDraggable> DraggingStarted;
    event Action<IDraggable> Dragging;
    event Action<IDraggable> DraggingStopped;

    // Methods

    void SetDragging(bool value);
    void Drag(Vector3 translation);
  }
}