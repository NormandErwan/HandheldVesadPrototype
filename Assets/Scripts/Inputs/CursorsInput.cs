using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public abstract class CursorsInput : MonoBehaviour
  {
    // Constants

    protected static readonly float cursorColliderRadiusFactor = 0.65f;

    // Editor fields

    [SerializeField]
    private Cursor[] cursors;

    // Properties

    public Dictionary<CursorType, Cursor> Cursors { get; protected set; }

    // Variables

    protected Dictionary<CursorType, MeshRenderer> cursorRenderers = new Dictionary<CursorType, MeshRenderer>();
    protected Dictionary<CursorType, Cursor> fakeCursors = new Dictionary<CursorType, Cursor>();

    // MonoBehaviour methods

    /// <summary>
    /// Initializes <see cref="Cursors"/>.
    /// </summary>
    protected virtual void Awake()
    {
      Cursors = new Dictionary<CursorType, Cursor>();
      foreach (var cursor in cursors)
      {
        Cursors.Add(cursor.Type, cursor);
        cursorRenderers.Add(cursor.Type, cursor.GetComponent<MeshRenderer>());
      }
    }

    protected virtual void Start()
    {
      DeactivateCursors();
    }

    /// <summary>
    /// Deactivates each cursor's gameObject in <see cref="Cursors"/>.
    /// </summary>
    protected virtual void Update()
    {
      DeactivateCursors();
      UpdateCursors();
    }

    // Methods

    public virtual void Configure(float maxSelectableDistance)
    {
      foreach (var cursor in Cursors)
      {
        cursor.Value.MaxSelectableDistance = maxSelectableDistance;
      }
    }

    protected virtual void DeactivateCursors()
    {
      foreach (var cursor in Cursors)
      {
        cursorRenderers[cursor.Key].enabled = false;
      }
    }

    protected virtual void ActivateCursor(CursorType cursorType)
    {
      cursorRenderers[cursorType].enabled = true;
    }

    protected abstract void UpdateCursors();
  }
}
