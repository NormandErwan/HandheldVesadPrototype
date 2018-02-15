using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class BaseCursor : MonoBehaviour, ICursor
  {
    // ICursor properties

    public virtual CursorType Type { get; set; }
    public bool IsFinger { get { return Type != CursorType.Look; } }
    public bool IsRightHanded
    {
      get
      {
        return Type == CursorType.RightThumb
          || Type == CursorType.RightIndex
          || Type == CursorType.RightMiddle
          || Type == CursorType.RightPinky
          || Type == CursorType.RightRing;
      }
    }
    public bool IsThumb { get { return Type == CursorType.LeftThumb || Type == CursorType.RightThumb; } }
    public bool IsIndex { get { return Type == CursorType.LeftIndex || Type == CursorType.RightIndex; } }
    public bool IsMiddle { get { return Type == CursorType.LeftMiddle || Type == CursorType.RightMiddle; } }
    public bool IsPinky { get { return Type == CursorType.LeftPinky || Type == CursorType.RightPinky; } }
    public bool IsRing { get { return Type == CursorType.LeftRing || Type == CursorType.RightRing; } }

    public virtual bool IsActive { get; protected set; }
    public virtual bool IsVisible { get; protected set; }
    public virtual bool IsTriggering { get; protected set; }

    // ICursor methods

    public virtual void SetActive(bool value)
    {
      IsActive = value;
    }

    public virtual void SetVisible(bool value)
    {
      IsVisible = value;
    }

    public virtual void SetTriggering(bool value)
    {
      IsTriggering = value;
    }
  }
}