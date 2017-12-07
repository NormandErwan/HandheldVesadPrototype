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

    // ICursor properties

    public CursorType Type { get; protected set; }
    public Transform Transform { get { return transform; } }
    
    // Properties

    public HoverCursorData HoverCursorData { get; protected set; }

    public bool IsFinger { get { return Type != CursorType.Look; } }
    public bool IsIndex { get { return Type == CursorType.LeftIndex || Type == CursorType.RightIndex; } }
    public bool IsThumb { get { return Type == CursorType.LeftThumb || Type == CursorType.RightThumb; } }
    public bool IsMiddle { get { return Type == CursorType.LeftMiddle || Type == CursorType.RightMiddle; } }

    // Variables

    protected static Dictionary<IInteractable, Dictionary<ICursor, Vector3>> latestCursorPositions = new Dictionary<IInteractable, Dictionary<ICursor, Vector3>>();

    protected Dictionary<ILongPressable, float> longPressTimers = new Dictionary<ILongPressable, float>();
    protected Dictionary<ITappable, float> tapTimers = new Dictionary<ITappable, float>();

    protected bool selectablesActivated = ;

    // Methods

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

      if (IsFinger)
      {
        GetInteractable<IInteractable>(other, (interactable) =>
        {
          if (!latestCursorPositions.ContainsKey(interactable))
          {
            latestCursorPositions.Add(interactable, new Dictionary<ICursor, Vector3>());
          }
          latestCursorPositions[interactable].Add(this, transform.position);
        });

        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (latestCursorPositions[draggable].Count > 1 && draggable.IsDragging)
          {
            ActivateSelectables(true);
            draggable.SetDragging(false); // Only one finger can drag, cancel if more than one finger
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count == 2 && !zoomable.IsZooming)
          {
            ActivateSelectables(false);
            zoomable.SetZooming(true);
          }
          else if (latestCursorPositions[zoomable].Count > 2 && zoomable.IsZooming)
          {
            ActivateSelectables(true);
            zoomable.SetZooming(false);
          }
        });

        if (selectablesActivated)
        {
          GetInteractable<ILongPressable>(other, (longPressable) =>
          {
            longPressTimers.Add(longPressable, 0);
          });

          GetInteractable<ITappable>(other, (tappable) =>
          {
            tapTimers.Add(tappable, 0);
          });
        }
      }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      if (IsFinger)
      {
        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (latestCursorPositions[draggable].Count == 1)
          {
            Vector3 movement = transform.position - latestCursorPositions[draggable][this];
            if (draggable.IsDragging)
            {
              latestCursorPositions[draggable][this] = transform.position;
              draggable.Drag(movement);
            }
            else if (movement.magnitude > draggable.DistanceToStartDragging)
            {
              latestCursorPositions[draggable][this] = transform.position;
              ActivateSelectables(false);
              draggable.SetDragging(true);
            }
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (zoomable.IsZooming)
          {
            Vector3 distance = transform.position, previousDistance = latestCursorPositions[zoomable][this];
            Vector3 movement = distance - previousDistance;

            foreach (var cursorProjection in latestCursorPositions[zoomable])
            {
              if (cursorProjection.Key != (ICursor)this)
              {
                distance -= cursorProjection.Value;
                previousDistance -= cursorProjection.Value;
              }
            }
            latestCursorPositions[zoomable][this] = transform.position;

            zoomable.Zoom(distance, previousDistance, movement);
          }
        });

        GetInteractable<ILongPressable>(other, (longPressable) =>
        {
          if (longPressTimers.ContainsKey(longPressable))
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
          }
        });

        GetInteractable<ITappable>(other, (tappable) =>
        {
          if (tapTimers.ContainsKey(tappable))
          {
            if (tapTimers[tappable] < tapTimeout)
            {
              tapTimers[tappable] += Time.deltaTime;
            }
          }
        });
      }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      GetInteractable<IFocusable>(other, (focusable) =>
      {
        focusable.SetFocused(false);
      });

      if (IsFinger)
      {
        GetInteractable<IInteractable>(other, (interactable) =>
        {
          latestCursorPositions[interactable].Remove(this);
          if (latestCursorPositions[interactable].Count == 0)
          {
            latestCursorPositions.Remove(interactable);
          }
        });

        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (!latestCursorPositions.ContainsKey(draggable) && draggable.IsDragging)
          {
            ActivateSelectables(true);
            draggable.SetDragging(false);
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (!latestCursorPositions.ContainsKey(zoomable) || (latestCursorPositions[zoomable].Count != 2 && zoomable.IsZooming))
          {
            ActivateSelectables(true);
            zoomable.SetZooming(false);
          }
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
      }
    }

    /// <summary>
    /// Cancels the long press and tap timers when dragging or zooming anything.
    /// </summary>
    protected virtual void ActivateSelectables(bool value)
    {
      selectablesActivated = value;
      if (!selectablesActivated)
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
    }

    protected virtual void GetInteractable<T>(Component component, Action<T> actionOnInteractable)
    {
      var interactable = component.GetComponent<T>();
      if (interactable != null)
      {
        actionOnInteractable(interactable);
      }
    }
  }
}