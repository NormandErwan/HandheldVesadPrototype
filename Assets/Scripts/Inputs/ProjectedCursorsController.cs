using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursorsController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private CursorsInput cursorsInput;

    [SerializeField]
    private ProjectedCursor projectedCursorsPrefab;

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Properties

    public Dictionary<CursorType, ProjectedCursor> ProjectedCursors { get; protected set; }

    // Methods

    protected virtual void Start()
    {
      ProjectedCursors = new Dictionary<CursorType, ProjectedCursor>(cursorsInput.Cursors.Count);
      foreach (var cursor in cursorsInput.Cursors)
      {
        var projectedCursor = Instantiate(projectedCursorsPrefab, grid.transform);
        projectedCursor.transform.localPosition = Vector3.zero;
        projectedCursor.transform.localRotation = Quaternion.identity;

        projectedCursor.name = "Projected cursor (" + cursor.Value.name + ")";
        projectedCursor.Cursor = cursor.Value;

        ProjectedCursors.Add(cursor.Key, projectedCursor);
      }

      cursorsInput.Updated += CursorsInput_Updated;
    }

    protected virtual void OnDestroy()
    {
      cursorsInput.Updated -= CursorsInput_Updated;
    }

    private void CursorsInput_Updated()
    {
      foreach (var projectedCursor in ProjectedCursors)
      {
        projectedCursor.Value.UpdateProjection();
      }
    }
  }
}