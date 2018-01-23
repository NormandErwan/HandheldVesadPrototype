using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface IZoomable : ITransformable
  {
    // Properties

    bool DragToZoom { get; }
    bool IsZooming { get; }
    GenericVector3<Range<float>> ScaleRange { get; }

    // Events

    event Action<IZoomable> ZoomingStarted;
    event Action<IZoomable, Vector3, Vector3> Zooming;
    event Action<IZoomable> ZoomingStopped;

    // Methods

    void SetZooming(bool value);
    void Zoom(Vector3 scaling, Vector3 translation);
  }
}