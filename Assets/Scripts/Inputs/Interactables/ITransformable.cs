using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ITransformable : IInteractable
  {
    // Properties

    Transform Transform { get; }
    GenericVector3<bool> FreezePosition { get; }
    GenericVector3<Range<float>> PositionRange { get; }

    // Methods

    Vector3 ProjectPosition(Vector3 position);
  }
}