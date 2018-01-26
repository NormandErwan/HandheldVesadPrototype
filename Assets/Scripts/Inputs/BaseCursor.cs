using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public abstract class BaseCursor : MonoBehaviour, ICursor
  {
    // ICursor properties

    public virtual CursorType Type { get; set; }
    public virtual bool IsActive { get; protected set; }
    public virtual bool IsVisible { get; protected set; }

    // ICursor methods

    public virtual void SetActive(bool value)
    {
      IsActive = value;
    }

    public virtual void SetVisible(bool value)
    {
      IsVisible = value;
    }
  }
}