using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ITransformable : IInteractable
  {
    // Properties

    bool IsTransformable { get; }
    bool IsTransforming { get; }

    Transform Transform { get; }
    GenericVector3<Range<float>> PositionRange { get; }

    // Events

    event Action<ITransformable> Transformable;

    // Methods

    void SetTransformable(bool value);
    Vector3 ProjectPosition(Vector3 position);
  }
}