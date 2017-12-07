using System;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface IZoomable : IInteractable
  {
    // Properties

    bool IsZooming { get; }

    // Events

    event Action<IZoomable> ZoomingStarted;
    event Action<IZoomable> Zooming;
    event Action<IZoomable> ZoomingStopped;

    // Methods

    void SetZooming(bool value);
    void Zoom(Vector3 distance, Vector3 previousDistance, Vector3 movement);
  }
}