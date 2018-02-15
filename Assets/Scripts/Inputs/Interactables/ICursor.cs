namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ICursor
  {
    // Properties

    CursorType Type { get; }
    bool IsFinger { get; }
    bool IsRightHanded { get; }
    bool IsThumb { get; }
    bool IsIndex { get; }
    bool IsMiddle { get; }
    bool IsPinky { get; }
    bool IsRing { get; }

    bool IsActive { get; }
    bool IsVisible { get; }
    bool IsTriggering { get; }

    // Methods

    void SetActive(bool value);
    void SetTriggering(bool value);
    void SetVisible(bool value);
  }
}