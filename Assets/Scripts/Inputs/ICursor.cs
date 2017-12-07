using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface ICursor
  {
    CursorType Type { get; }
    Transform Transform { get; }
  }
}