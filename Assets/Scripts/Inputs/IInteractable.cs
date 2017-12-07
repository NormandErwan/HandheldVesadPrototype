using System;
using System.Collections.Generic;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  public interface IInteractable
  {
    // Properties

    IEnumerable<ICursor> InteractingCursors { get; }

    // Events

    event Action<IInteractable> CursorAdded;
    event Action<IInteractable> CursorRemoved;

    // Methods

    void AddCursor(ICursor cursor);
    void RemoveCursor(ICursor cursor);
  }
}