using NormandErwan.MasterThesis.Experiment.Utilities;
using System;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ITransformable : IInteractable
  {
    // Properties

    bool IsTransformable { get; }
    bool IsTransforming { get; }

    GenericVector3<Range<float>> PositionRange { get; }

    // Events

    event Action<ITransformable> Transformable;

    // Methods

    void SetTransformable(bool value);
  }
}