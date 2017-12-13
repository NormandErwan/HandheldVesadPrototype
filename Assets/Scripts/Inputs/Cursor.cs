using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  [RequireComponent(typeof(SphereCollider))]
  public class Cursor : MonoBehaviour, ICursor
  {
    // Constants

    public static readonly float longPressMinTime = 0.5f; // in seconds
    public static readonly float tapTimeout = 0.5f; // in seconds

    // Editor fields

    [SerializeField]
    private CursorType type;

    [SerializeField]
    private DeviceController deviceController;

    // ICursor properties

    public CursorType Type { get { return type; } set { type = value; } }
    public GameObject GameObject { get { return gameObject; } }
    
    // Properties

    public DeviceController DeviceController { get { return deviceController; } set { deviceController = value; } }
    public SphereCollider Collider { get; protected set; }

    public bool IsFinger { get { return Type != CursorType.Look; } }
    public bool IsIndex { get { return Type == CursorType.LeftIndex || Type == CursorType.RightIndex; } }
    public bool IsThumb { get { return Type == CursorType.LeftThumb || Type == CursorType.RightThumb; } }
    public bool IsMiddle { get { return Type == CursorType.LeftMiddle || Type == CursorType.RightMiddle; } }

    // Variables

    protected static Dictionary<ITransformable, Dictionary<Cursor, Vector3>> latestCursorPositions 
      = new Dictionary<ITransformable, Dictionary<Cursor, Vector3>>();

    protected Dictionary<ILongPressable, float> longPressTimers = new Dictionary<ILongPressable, float>();
    protected Dictionary<ITappable, float> tapTimers = new Dictionary<ITappable, float>();

    // Methods

    protected virtual void Awake()
    {
      Collider = GetComponent<SphereCollider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      GetInteractable<IInteractable>(other, (interactable) =>
      {
        if (!interactable.IsInteractable)
        {
          return;
        }
      });

      GetInteractable<IFocusable>(other, (focusable) =>
      {
        focusable.SetFocused(true);
      });

      if (IsFinger)
      {
        GetInteractable<ITransformable>(other, (transformable) =>
        {
          if (!latestCursorPositions.ContainsKey(transformable))
          {
            latestCursorPositions.Add(transformable, new Dictionary<Cursor, Vector3>());
          }
          latestCursorPositions[transformable][this] = transform.position;
        });

        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (latestCursorPositions[draggable].Count > 1 && draggable.IsDragging)
          {
            draggable.SetDragging(false); // Only one finger can drag, cancel if more than one finger
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count == 2 && !zoomable.IsZooming)
          {
            zoomable.SetZooming(true);
          }
          else if (latestCursorPositions[zoomable].Count > 2 && zoomable.IsZooming)
          {
            zoomable.SetZooming(false);
          }
        });

        GetInteractable<ILongPressable>(other, (longPressable) =>
        {
          if (longPressable.IsSelectable)
          {
            longPressTimers.Add(longPressable, 0);
          }
        });

        GetInteractable<ITappable>(other, (tappable) =>
        {
          if (tappable.IsSelectable)
          {
            tapTimers.Add(tappable, 0);
          }
        });
      }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      GetInteractable<IInteractable>(other, (interactable) =>
      {
        if (!interactable.IsInteractable)
        {
          return;
        }
      });

      if (IsFinger)
      {
        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (draggable.IsInteractable && latestCursorPositions[draggable].Count == 1)
          {
            var translation = transform.position - latestCursorPositions[draggable][this];
            if (draggable.IsDragging)
            {
              latestCursorPositions[draggable][this] = transform.position;
              draggable.Drag(translation);
            }
            else if (translation.magnitude > DeviceController.MaxSelectableDistance)
            {
              latestCursorPositions[draggable][this] = transform.position;
              draggable.SetDragging(true);
            }
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (zoomable.IsInteractable && zoomable.IsZooming)
          {
            var cursors = new List<Cursor>(latestCursorPositions[zoomable].Keys);
            if (cursors[0] == this) // Update only once per frame
            {
              var translation = cursors[1].transform.position;
              var originalTranslation = latestCursorPositions[zoomable][cursors[1]];
              var distance = cursors[0].transform.position - cursors[1].transform.position;
              var originalDistance = latestCursorPositions[zoomable][cursors[0]] - latestCursorPositions[zoomable][cursors[1]];

              zoomable.Zoom(distance, originalDistance, translation, originalTranslation);
            }
          }
        });

        GetInteractable<ILongPressable>(other, (longPressable) =>
        {
          if (longPressTimers.ContainsKey(longPressable))
          {
            if (!longPressable.IsInteractable || !longPressable.IsSelectable)
            {
              longPressTimers.Remove(longPressable);
            }
            else if (longPressTimers[longPressable] < longPressMinTime)
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
            if (!tappable.IsInteractable || !tappable.IsSelectable)
            {
              tapTimers.Remove(tappable);
            }
            else if (tapTimers[tappable] < tapTimeout)
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
        GetInteractable<ITransformable>(other, (transformable) =>
        {
          latestCursorPositions[transformable].Remove(this);
        });

        GetInteractable<IDraggable>(other, (draggable) =>
        {
          if (latestCursorPositions[draggable].Count == 0 && draggable.IsDragging)
          {
            draggable.SetDragging(false);
          }
        });

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count < 2 && zoomable.IsZooming)
          {
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
            if (tappable.IsInteractable && tappable.IsSelectable && tapTimers[tappable] < tapTimeout)
            {
              tappable.SetSelected(true);
            }
            tapTimers.Remove(tappable);
          }
        });
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