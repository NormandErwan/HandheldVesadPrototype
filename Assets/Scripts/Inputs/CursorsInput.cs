using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public abstract class CursorsInput<T> : MonoBehaviour where T : BaseCursor
  {
    // Properties

    public Dictionary<CursorType, T> Cursors { get; protected set; }

    // Events

    public event Action Updated = delegate { };

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      Cursors = new Dictionary<CursorType, T>();
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

    public virtual void DeactivateCursors()
    {
      foreach (var cursorKeyValue in Cursors)
      {
        var cursor = cursorKeyValue.Value;
        if (!cursor.IsVisible)
        {
          cursor.SetVisible(false);
          cursor.transform.position = new Vector3(cursor.transform.position.x, cursor.transform.position.y, -10); // keep the cursor far away if not visible to activate OnTriggerExit
        }
      }
    }

    protected abstract void UpdateCursors();
  }
}
