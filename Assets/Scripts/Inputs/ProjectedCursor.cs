using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursor : BaseCursor
  {
    // Editor fields

    [SerializeField]
    private Cursor cursor;

    [SerializeField]
    protected GameObject projectionLine;

    [SerializeField]
    private DeviceController deviceController;

    // ICursor properties

    public override CursorType Type { get { return cursor.Type; } }

    // Properties

    public Cursor Cursor { get { return cursor; } set { cursor = value; } }
    public GameObject ProjectionLine { get { return projectionLine; } set { projectionLine = value; } }

    public bool IsOnGrid { get; protected set; }

        // Variables

        protected GenericVector3<Range<float>> positionRanges = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), null);
    protected TaskGrid taskGrid;

    // Methods

    protected virtual void Awake()
    {
      SetVisible(false);
      SetActive(true);
      IsOnGrid = false;

      deviceController.CursorsInput.Updated += CursorsInput_Updated;

      taskGrid = deviceController.TaskGrid;
      taskGrid.Configured += TaskGrid_Configured;
    }

    protected virtual void OnDestroy()
    {
      deviceController.CursorsInput.Updated -= CursorsInput_Updated;
      taskGrid.Configured -= TaskGrid_Configured;
    }

    protected virtual void CursorsInput_Updated()
    {
      IsOnGrid = false;
      if (Cursor.gameObject.activeSelf && Cursor.IsTracked)
      {
        float cursorGridDistance = Vector3.Dot(Cursor.transform.position - taskGrid.transform.position, -taskGrid.transform.forward);
        var position = Cursor.transform.position + cursorGridDistance * taskGrid.transform.forward;

        var positionToGrid = position - taskGrid.transform.position;
        if (positionRanges.X.ContainsValue(positionToGrid.x) && positionRanges.Y.ContainsValue(positionToGrid.y))
        {
          IsOnGrid = true;
          transform.position = position;
          transform.rotation = taskGrid.transform.rotation;
          ProjectionLine.transform.localScale = new Vector3(ProjectionLine.transform.localScale.x, cursorGridDistance, ProjectionLine.transform.localScale.z);
        }
      }

      IsVisible = Cursor.gameObject.activeSelf && Cursor.IsVisible && IsOnGrid;
      SetVisible(IsVisible);
    }

    public override void SetVisible(bool value)
    {
      base.SetVisible(value);
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(value);
      }
    }

    protected virtual void TaskGrid_Configured()
    {
      var gridScale = Vector3.Scale(taskGrid.transform.lossyScale, taskGrid.Scale);
      positionRanges.X.Minimum = -gridScale.x / 2f;
      positionRanges.X.Maximum = gridScale.x / 2f;
      positionRanges.Y.Minimum = -gridScale.y / 2f;
      positionRanges.Y.Maximum = gridScale.y / 2f;
    }
  }
}