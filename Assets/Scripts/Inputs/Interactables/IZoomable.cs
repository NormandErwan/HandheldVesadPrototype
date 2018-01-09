using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface IZoomable : ITransformable
  {
    // Properties

    bool IsZooming { get; }
    GenericVector3<bool> FreezeScale { get; }
    GenericVector3<Range<float>> ScaleRange { get; }

    // Events

    event Action<IZoomable> ZoomingStarted;
    event Action<IZoomable, float, Vector3, Vector3[]> Zooming;
    event Action<IZoomable> ZoomingStopped;

    // Methods

    void SetZooming(bool value);
    void Zoom(float scaleFactor, Vector3 translation, Vector3[] cursors);
  }
}