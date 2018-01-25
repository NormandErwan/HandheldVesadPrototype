using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursor : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private Cursor cursor;

    [SerializeField]
    protected GameObject projectionLine;

    [SerializeField]
    private DeviceController deviceController;

    // Properties

    public Cursor Cursor { get { return cursor; } set { cursor = value; } }
    public GameObject ProjectionLine { get { return projectionLine; } set { projectionLine = value; } }
    public bool IsActive { get; protected set; }

    // Variables

    protected GenericVector3<Range<float>> positionRanges = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), null);
    protected Experiment.Task.Grid grid;

    // Methods

    protected virtual void Start()
    {
      deviceController.CursorsInput.Updated += CursorsInput_Updated;

      grid = deviceController.Grid;
      grid.Configured += Grid_Configured;

      SetActive(false);
    }

    protected virtual void OnDestroy()
    {
      deviceController.CursorsInput.Updated -= CursorsInput_Updated;
      grid.Configured -= Grid_Configured;
    }

    protected virtual void CursorsInput_Updated()
    {
      IsActive = false;
      if (Cursor.gameObject.activeSelf && Cursor.IsVisible)
      {
        float cursorGridDistance = Vector3.Dot(Cursor.transform.position - grid.transform.position, -grid.transform.forward);
        transform.position = Cursor.transform.position + cursorGridDistance * grid.transform.forward;

        var positionToGrid = Vector3.ProjectOnPlane(transform.position - grid.transform.position, -grid.transform.forward);
        positionToGrid = transform.position - grid.transform.position;
        print(positionToGrid.ToString("F3"));
        if (positionRanges.X.ContainsValue(positionToGrid.x) && positionRanges.Y.ContainsValue(positionToGrid.y))
        {
          IsActive = true;
          transform.forward = grid.transform.forward;
          ProjectionLine.transform.localScale = new Vector3(ProjectionLine.transform.localScale.x, cursorGridDistance, ProjectionLine.transform.localScale.z);
        }
      }

      SetActive(IsActive);
    }

    public virtual void SetActive(bool value)
    {
      IsActive = value;
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(value);
      }
    }

    protected virtual void Grid_Configured()
    {
      var gridScale = Vector3.Scale(grid.transform.lossyScale, grid.Scale);
      positionRanges.X.Minimum = -gridScale.x / 2f;
      positionRanges.X.Maximum = gridScale.x / 2f;
      positionRanges.Y.Minimum = -gridScale.y / 2f;
      positionRanges.Y.Maximum = gridScale.y / 2f;
    }
  }
}