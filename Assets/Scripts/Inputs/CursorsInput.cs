using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
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

    // Events

    public event Action Updated = delegate { };

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      Cursors = new Dictionary<CursorType, Cursor>();
      foreach (var cursor in cursors)
      {
        Cursors.Add(cursor.Type, cursor);
      }
    }

    /// <summary>
    /// Deactivates each cursor's gameObject in <see cref="Cursors"/>.
    /// </summary>
    protected virtual void Update()
    {
      DeactivateCursors();
      UpdateCursors();
      Updated();
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
        cursor.Value.SetVisible(false);
        cursor.Value.transform.position = new Vector3(cursor.Value.transform.position.x, cursor.Value.transform.position.y, -10); // keep the cursor far away if not visible to activate OnTriggerExit
      }
    }

    protected abstract void UpdateCursors();
  }
}
