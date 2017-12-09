using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.UI.Grid
{
  public interface IGridElement<T> where T : IGridElement<T>
  {
    // Properties

    GameObject GameObject { get; }
    Vector2 Scale { get; set; }
    Vector2 Margin { get; set; }

    // Methods

    T Instantiate();
    void Configure();
  }
}