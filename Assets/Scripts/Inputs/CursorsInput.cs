using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public abstract class CursorsInput : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private Cursor[] cursors;

    // Properties

    public Dictionary<CursorType, Cursor> Cursors { get; protected set; }

    // Methods

    /// <summary>
    /// Initializes <see cref="Cursors"/>.
    /// </summary>
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
      foreach (var cursor in Cursors)
      {
        cursor.Value.GameObject.SetActive(false);
      }

      UpdateCursors();
    }

    protected abstract void UpdateCursors();
  }
}
