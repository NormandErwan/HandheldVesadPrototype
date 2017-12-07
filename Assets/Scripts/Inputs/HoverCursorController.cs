using Hover.Core.Cursors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Inputs
{
  [RequireComponent(typeof(HoverCursorData))]
  [RequireComponent(typeof(Collider))]
  public class HoverCursorController : MonoBehaviour, ICursor
  {
    // Constants

    public static readonly float longPressTimeout = 0.5f; // in seconds
    public static readonly float tapTimeout = 0.3f; // in seconds

    // Editor fields

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Properties

    public CursorType Type { get; protected set; }
    public HoverCursorData HoverCursorData { get; protected set; }

    public bool IsFinger { get { return Type != CursorType.Look; } }
    public bool IsIndex { get { return Type == CursorType.LeftIndex || Type == CursorType.RightIndex; } }
    public bool IsThumb { get { return Type == CursorType.LeftThumb || Type == CursorType.RightThumb; } }
    public bool IsMiddle { get { return Type == CursorType.LeftMiddle || Type == CursorType.RightMiddle; } }

    // Variables

    protected Dictionary<IDraggable, Vector3> dragLastCursorProjections = new Dictionary<IDraggable, Vector3>();
    protected Dictionary<ILongPressable, float> longPressTimers = new Dictionary<ILongPressable, float>();
    protected Dictionary<ITappable, float> tapTimers = new Dictionary<ITappable, float>();

    // Methods

    protected void OnEnable()
    {
      grid.DraggingStarted += Grid_DraggingStarted;
      grid.ZoomingStarted += Grid_ZoomingStarted;
    }

    protected void OnDisable()
    {
      grid.DraggingStarted -= Grid_DraggingStarted;
      grid.ZoomingStarted -= Grid_ZoomingStarted;
    }

    protected void Awake()
    {
      HoverCursorData = GetComponent<HoverCursorData>();
      switch (HoverCursorData.Type)
      {
        case Hover.Core.Cursors.CursorType.LeftThumb:   Type = CursorType.LeftThumb;    break;
        case Hover.Core.Cursors.CursorType.LeftIndex:   Type = CursorType.LeftIndex;    break;
        case Hover.Core.Cursors.CursorType.LeftMiddle:  Type = CursorType.LeftMiddle;   break;
        case Hover.Core.Cursors.CursorType.LeftRing:    Type = CursorType.LeftRing;     break;
        case Hover.Core.Cursors.CursorType.LeftPinky:   Type = CursorType.LeftPinky;    break;
        case Hover.Core.Cursors.CursorType.RightThumb:  Type = CursorType.RightThumb;   break;
        case Hover.Core.Cursors.CursorType.RightIndex:  Type = CursorType.RightIndex;   break;
        case Hover.Core.Cursors.CursorType.RightMiddle: Type = CursorType.RightMiddle;  break;
        case Hover.Core.Cursors.CursorType.RightRing:   Type = CursorType.RightRing;    break;
        case Hover.Core.Cursors.CursorType.RightPinky:  Type = CursorType.RightPinky;   break;
        case Hover.Core.Cursors.CursorType.Look:        Type = CursorType.Look;         break;
      }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      GetInteractable<IFocusable>(other, (focusable) =>
      {
        focusable.SetFocused(true);
      });

      if (!grid.IsDragging && !grid.IsZooming && IsFinger)
      {
        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (!dragLastCursorProjections.ContainsKey(draggable))
          {
            var projectedCursor = Vector3.ProjectOnPlane(transform.position, draggable.PlaneNormal);
            dragLastCursorProjections.Add(draggable, projectedCursor);
          }
          else
          {
            dragLastCursorProjections.Remove(draggable); // Only one finger can drag, cancel if more than one finger
          }
        });

        GetInteractable<ILongPressable>(other, (longPressable) =>
        {
          longPressTimers.Add(longPressable, 0);
        });

        GetInteractable<ITappable>(other, (tappable) =>
        {
          tapTimers.Add(tappable, 0);
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          zoomable.AddCursor(this);
          if (zoomable.InteractingCursors.Count() >= 2 && !zoomable.IsZooming)
          {
            // TODO
          }
        });
      }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      GetInteractable<IDraggable>(other, (draggable) =>
      {
        if (dragLastCursorProjections.ContainsKey(draggable))
        {
          dragLastCursorProjections.Remove(draggable);
        }
        draggable.SetDragging(false);
      });

      GetInteractable<IFocusable>(other, (focusable) =>
      {
        focusable.SetFocused(false);
      });

      GetInteractable<ILongPressable>(other, (longPressable) =>
      {
        if (longPressTimers.ContainsKey(longPressable))
        {
          longPressTimers.Remove(longPressable);
        }
      });

      GetInteractable<ITappable>(other, (tappable) =>
      {
        if (tapTimers.ContainsKey(tappable))
        {
          if (tapTimers[tappable] < tapTimeout)
          {
            tappable.SetSelected(true);
          }
          tapTimers.Remove(tappable);
        }
      });

      GetInteractable<IZoomable>(other, (interactable) =>
      {
        interactable.RemoveCursor(this);
        if (interactable.InteractingCursors.Count() < 2 && interactable.IsZooming)
        {
          interactable.SetZooming(false);
        }
      });
    }

    protected virtual void Update()
    {
      ForEach(dragLastCursorProjections, (draggable) =>
      {
        var projectedCursor = Vector3.ProjectOnPlane(transform.position, draggable.PlaneNormal);
        var movement = projectedCursor - dragLastCursorProjections[draggable];

        if (draggable.IsDragging)
        {
          draggable.Drag(movement);
          dragLastCursorProjections[draggable] = projectedCursor;
        }
        else if (movement.magnitude > draggable.DistanceToStartDragging)
        {
          draggable.SetDragging(true);
          dragLastCursorProjections[draggable] = projectedCursor;
        }
      });

      ForEach(longPressTimers, (longPressable) =>
      {
        if (longPressTimers[longPressable] < longPressTimeout)
        {
          longPressTimers[longPressable] += Time.deltaTime;
        }
        else
        {
          longPressable.SetSelected(true);
          longPressTimers.Remove(longPressable);
        }
      });

      ForEach(tapTimers, (tappable) =>
      {
        if (tapTimers[tappable] < longPressTimeout)
        {
          tapTimers[tappable] += Time.deltaTime;
        }
      });
    }

    protected virtual void Grid_DraggingStarted(IDraggable grid)
    {
      CancelTimers();
    }

    protected virtual void Grid_ZoomingStarted(IZoomable grid)
    {
      CancelTimers();
    }

    /// <summary>
    /// Cancels the long press and tap timers when <see cref="Grid.CurrentMode"/> is different of <see cref="Experiment.Task.Grid.Mode.Idle"/>.
    /// </summary>
    protected virtual void CancelTimers()
    {
      foreach (var longPressable in new List<ILongPressable>(longPressTimers.Keys))
      {
        longPressTimers.Remove(longPressable);
      }

      foreach (var tappable in new List<ITappable>(tapTimers.Keys))
      {
        tapTimers.Remove(tappable);
      }
    }

    protected virtual void GetInteractable<T>(Component component, Action<T> actionOnInteractable)
    {
      var interactable = component.GetComponent<T>();
      if (interactable != null)
      {
        actionOnInteractable(interactable);
      }
    }

    protected virtual void ForEach<T, U>(Dictionary<T, U> dictionary, Action<T> actionOnEntries)
    {
      if (dictionary.Count > 0)
      {
        foreach (var key in new List<T>(dictionary.Keys))
        {
          actionOnEntries(key);
        }
      }
    }
  }
}