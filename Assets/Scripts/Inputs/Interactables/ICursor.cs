using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ICursor
  {
    CursorType Type { get; }
    Transform Transform { get; }
  }
}