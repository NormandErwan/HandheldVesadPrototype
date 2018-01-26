using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ICursor
  {
    // Properties

    CursorType Type { get; }
    bool IsActive { get; }
    bool IsVisible { get; }

    // Methods

    void SetActive(bool value);
    void SetVisible(bool value);
  }
}