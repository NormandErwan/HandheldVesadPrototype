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
    protected GameObject line;

    // Properties

    public Cursor Cursor { get { return cursor; } set { cursor = value; } }
    public GameObject ProjectionLine { get { return line; } set { line = value; } }

    // Variables

    protected bool wasActive = false;
    protected Experiment.Task.Grid grid;
    protected GenericVector3<Range<float>> positionRanges = new GenericVector3<Range<float>>();

    // Methods

    protected virtual void Awake()
    {
      grid = transform.parent.GetComponent<Experiment.Task.Grid>();
      grid.Configured += Grid_Configured;
      grid.Zooming += Grid_Zooming;

      positionRanges.X = new Range<float>();
      positionRanges.Y = new Range<float>();
      UpdatePositionRanges();

      SetActive(false);
    }

    protected virtual void OnDestroy()
    {
      grid.Configured -= Grid_Configured;
      grid.Zooming -= Grid_Zooming;
    }

    public virtual void UpdateProjection()
    {
      bool active = false;
      if (Cursor.IsActivated)
      {
        var projectedPosition = Vector3.ProjectOnPlane(Cursor.transform.position, -grid.transform.forward);
        transform.position = new Vector3(projectedPosition.x, projectedPosition.y, transform.position.z);
        if (positionRanges.X.ContainsValue(transform.localPosition.x) && positionRanges.Y.ContainsValue(transform.localPosition.y))
        {
          active = true;

          float yRotation = (transform.position.z > Cursor.transform.position.z) ? 0f : 180f;
          ProjectionLine.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

          var distance = Vector3.Distance(transform.position, Cursor.transform.position) / 2f / transform.lossyScale.z;
          ProjectionLine.transform.localScale = new Vector3(ProjectionLine.transform.localScale.x, ProjectionLine.transform.localScale.y, distance);
        }
      }

      if (active != wasActive)
      {
        SetActive(active);
        wasActive = active;
      }
    }

    public virtual void SetActive(bool value)
    {
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(value);
      }
    }

    protected virtual void Grid_Configured()
    {
      UpdatePositionRanges();
    }

    protected virtual void Grid_Zooming(Interactables.IZoomable grid, float scaleFactor, Vector3 translation, Vector3[] cursors)
    {
      transform.localScale /= scaleFactor;
      UpdatePositionRanges();
    }

    protected virtual void UpdatePositionRanges()
    {
      positionRanges.X.Minimum = -grid.Scale.x / 2f;
      positionRanges.X.Maximum = grid.Scale.x / 2f;
      positionRanges.Y.Minimum = -grid.Scale.y / 2f;
      positionRanges.Y.Maximum = grid.Scale.y / 2f;
    }
  }
}