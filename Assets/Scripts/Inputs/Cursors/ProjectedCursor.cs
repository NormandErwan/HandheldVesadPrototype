using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class ProjectedCursor : BaseCursor
  {
    // Editor fields

    [SerializeField]
    private FingerCursor cursor;

    [SerializeField]
    private DeviceController deviceController;

    [Header("Projection Line")]
    [SerializeField]
    private GameObject projectionLine;

    [SerializeField]
    private Renderer projectionLineMesh;

    [SerializeField]
    private Material defaultProjectionLineMaterial;

    [SerializeField]
    private Material triggeringProjectionLineMaterial;

    [Header("Projected Cursor Meshs")]
    [SerializeField]
    private Renderer[] projectedCursorMeshs;

    [SerializeField]
    private Material defaultProjectedCursorMeshsMaterial;

    [SerializeField]
    private Material triggeringProjectedCursorMeshsMaterial;

    // ICursor properties

    public override CursorType Type { get { return cursor.Type; } }

    // Properties

    public bool IsOnGrid { get; protected set; }

    // Variables

    protected GenericVector3<Range<float>> positionRanges = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), null);
    protected TaskGrid taskGrid;

    // Methods

    protected virtual void Awake()
    {
      deviceController.FingerCursorsInput.Updated += CursorsInput_Updated;
      
      taskGrid = deviceController.TaskGrid;
      taskGrid.Configured += TaskGrid_Configured;

      SetVisible(false);
      SetActive(true);
      IsOnGrid = false;
    }

    protected virtual void OnDestroy()
    {
      deviceController.FingerCursorsInput.Updated -= CursorsInput_Updated;
      taskGrid.Configured -= TaskGrid_Configured;
    }

    protected virtual void CursorsInput_Updated()
    {
      IsOnGrid = false;
      if (cursor.gameObject.activeSelf && cursor.IsTracked)
      {
        float cursorGridDistance = Vector3.Dot(cursor.transform.position - taskGrid.transform.position, -taskGrid.transform.forward);
        var position = cursor.transform.position + cursorGridDistance * taskGrid.transform.forward;

        var positionToGrid = position - taskGrid.transform.position;
        if (positionRanges.X.ContainsValue(positionToGrid.x) && positionRanges.Y.ContainsValue(positionToGrid.y))
        {
          IsOnGrid = true;
          transform.position = position;
          transform.rotation = taskGrid.transform.rotation;
          projectionLine.transform.localScale = new Vector3(projectionLine.transform.localScale.x, cursorGridDistance, projectionLine.transform.localScale.z);
        }
      }

      IsVisible = cursor.gameObject.activeSelf && cursor.IsVisible && IsOnGrid;
      SetVisible(IsVisible);
    }

    public override void SetVisible(bool value)
    {
      base.SetVisible(value);
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(value);
      }

      projectionLineMesh.material = (taskGrid.IsFocused) ? triggeringProjectionLineMaterial : defaultProjectionLineMaterial;
      foreach (var projectedCursorMesh in projectedCursorMeshs)
      {
        projectedCursorMesh.material = (taskGrid.IsFocused) ? triggeringProjectedCursorMeshsMaterial : defaultProjectedCursorMeshsMaterial;
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