namespace NormandErwan.MasterThesis.Experiment.Inputs.Interactables
{
  public interface ICursor
  {
    // Properties

    CursorType Type { get; }
    bool IsActive { get; }
    bool IsVisible { get; }
    bool IsRightHanded { get; }

    // Methods

    void SetActive(bool value);
    void SetVisible(bool value);
  }
}