using System;

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
    void Zoom();
  }
}