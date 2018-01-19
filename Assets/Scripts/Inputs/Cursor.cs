using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections;
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

    // ICursor properties

    public CursorType Type { get { return type; } set { type = value; } }
    public GameObject GameObject { get { return gameObject; } }
    
    // Properties

    public float MaxSelectableDistance { get; set; }

    public bool IsActivated { get; protected set; }

    public bool IsFinger { get { return Type != CursorType.Look; } }
    public bool IsIndex { get { return Type == CursorType.LeftIndex || Type == CursorType.RightIndex; } }
    public bool IsThumb { get { return Type == CursorType.LeftThumb || Type == CursorType.RightThumb; } }
    public bool IsMiddle { get { return Type == CursorType.LeftMiddle || Type == CursorType.RightMiddle; } }

    // Variables

    protected static Dictionary<ITransformable, Dictionary<Cursor, Vector3>> latestCursorPositions 
      = new Dictionary<ITransformable, Dictionary<Cursor, Vector3>>();

    protected Dictionary<ILongPressable, float> longPressTimers = new Dictionary<ILongPressable, float>();
    protected Dictionary<ITappable, float> tapTimers = new Dictionary<ITappable, float>();

    protected new Renderer renderer;

    // Methods

    public void SetActivated(bool value)
    {
      IsActivated = value;
      renderer.enabled = IsActivated;
    }

    protected virtual void Awake()
    {
      renderer = GetComponent<Renderer>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      if (!IsInteractable(other))
      {
        return;
      }

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

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count == 2 && !zoomable.DragToZoom && !zoomable.IsZooming)
          {
            zoomable.SetZooming(true);
          }
          else if (latestCursorPositions[zoomable].Count > 2 && zoomable.IsZooming)
          {
            zoomable.SetZooming(false);
          }
        });

        if (IsIndex)
        {
          GetInteractable<IDraggable>(other, (draggable) =>
          {
            if (latestCursorPositions[draggable].Count > 1 && draggable.IsDragging)
            {
              draggable.SetDragging(false); // Only one finger can drag, cancel if more than one finger
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
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      if (!IsInteractable(other))
      {
        return;
      }

      if (IsFinger)
      {
        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count == 1)
          {
            if (zoomable.DragToZoom && !zoomable.IsZooming)
            {
              var translation = zoomable.ProjectPosition(transform.position - latestCursorPositions[zoomable][this]);
              if (translation.magnitude > MaxSelectableDistance)
              {
                zoomable.SetZooming(true);
                latestCursorPositions[zoomable][this] = transform.position;
              }
            }
            else if (!zoomable.DragToZoom && zoomable.IsZooming)
            {
              var draggable = other.GetComponent<IDraggable>();
              if (draggable != null)
              {
                zoomable.SetZooming(false);
                draggable.SetDragging(true);
                latestCursorPositions[zoomable][this] = transform.position;
              }
            }
          }

          if (zoomable.IsZooming)
          {
            var latestPositions = latestCursorPositions[zoomable];
            var cursors = new List<Cursor>(latestPositions.Keys);
            if (cursors[0] == this) // Update only once per frame
            {
              Vector3[] cursorPositions;
              if (!zoomable.DragToZoom)
              {
                cursorPositions = new Vector3[4] {
                  zoomable.ProjectPosition(cursors[0].transform.position), zoomable.ProjectPosition(latestPositions[cursors[0]]),
                  zoomable.ProjectPosition(cursors[1].transform.position), zoomable.ProjectPosition(latestPositions[cursors[1]])
                };
              }
              else
              {
                cursorPositions = new Vector3[4] {
                  zoomable.ProjectPosition(cursors[0].transform.position), zoomable.ProjectPosition(latestPositions[cursors[0]]),
                  zoomable.ProjectPosition(zoomable.Transform.position), zoomable.ProjectPosition(zoomable.Transform.position)
                };
              }

              var distance = (zoomable.ProjectPosition(cursorPositions[0] - cursorPositions[2])).magnitude;
              var previousDistance = (zoomable.ProjectPosition(cursorPositions[1] - cursorPositions[3])).magnitude;
              float scaleFactor = (previousDistance != 0) ? distance / previousDistance : 1f;

              bool hasClamped = Scale(zoomable, scaleFactor * zoomable.Transform.localScale);
              if (!hasClamped)
              {
                var newPosition = cursorPositions[0] - scaleFactor * (cursorPositions[1] - zoomable.Transform.position);
                var translation = newPosition - zoomable.Transform.position;
                Translate(zoomable, translation);
                zoomable.Zoom(scaleFactor, translation, cursorPositions);
              }
              else
              {
                zoomable.Zoom(scaleFactor, Vector3.zero, cursorPositions);
              }

              latestPositions[cursors[0]] = cursorPositions[0];
              if (!zoomable.DragToZoom)
              {
                latestPositions[cursors[1]] = cursorPositions[2];
              }
            }
          }
        });

        if (IsIndex)
        {
          GetInteractable<IDraggable>(other, (draggable) =>
          {
            if (latestCursorPositions[draggable].Count == 1)
            {
              var zoomable = other.GetComponent<IZoomable>();
              if (zoomable != null && zoomable.DragToZoom)
              {
                if (draggable.IsDragging)
                {
                  draggable.SetDragging(false);
                  zoomable.SetZooming(true);
                  latestCursorPositions[draggable][this] = transform.position;
                }
              }
              else
              {
                var translation = draggable.ProjectPosition(transform.position - latestCursorPositions[draggable][this]);
                if (draggable.IsDragging)
                {
                  Translate(draggable, translation);
                  draggable.Drag(translation);
                  latestCursorPositions[draggable][this] = transform.position;
                }
                else if (translation.magnitude > MaxSelectableDistance)
                {
                  draggable.SetDragging(true);
                  latestCursorPositions[draggable][this] = transform.position;
                }
              }
            }
          });

          GetInteractable<ILongPressable>(other, (longPressable) =>
          {
            if (longPressTimers.ContainsKey(longPressable))
            {
              if (longPressTimers[longPressable] < longPressMinTime)
              {
                longPressTimers[longPressable] += Time.deltaTime;
              }
              else
              {
                StartCoroutine(SetSelected(longPressable));
                longPressTimers.Remove(longPressable);
              }
            }
          });

          GetInteractable<ITappable>(other, (tappable) =>
          {
            if (tapTimers.ContainsKey(tappable) && tapTimers[tappable] < tapTimeout)
            {
              tapTimers[tappable] += Time.deltaTime;
            }
          });
        }
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

        GetInteractable<IZoomable>(other, (zoomable) =>
        {
          if (latestCursorPositions[zoomable].Count < 2 && zoomable.IsZooming)
          {
            zoomable.SetZooming(false);
          }
        });

        if (IsIndex)
        {
          GetInteractable<IDraggable>(other, (draggable) =>
          {
            if (latestCursorPositions[draggable].Count == 0 && draggable.IsDragging)
            {
              draggable.SetDragging(false);
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
                StartCoroutine(SetSelected(tappable));
              }
              tapTimers.Remove(tappable);
            }
          });
        }
      }
    }

    protected virtual bool IsInteractable(Component component)
    {
      bool value = true;
      GetInteractable<IInteractable>(component, (interactable) =>
      {
        value = interactable.IsInteractable;
      });
      return value;
    }

    protected virtual void GetInteractable<T>(Component component, Action<T> actionOnInteractable) where T : IInteractable
    {
      var interactable = component.GetComponent<T>();
      if (interactable != null)
      {
        actionOnInteractable(interactable);
      }
    }

    protected virtual void Translate(ITransformable transformable, Vector3 translation)
    {
      Vector3 position = transformable.Transform.position;
      for (int i = 0; i < 2; i++)
      {
        if (!transformable.FreezePosition[i])
        {
          position[i] = transformable.PositionRange[i].Clamp(position[i] + translation[i]);
        }
      }
      transformable.Transform.position = position;
    }

    protected virtual bool Scale(IZoomable zoomable, Vector3 newValue)
    {
      bool hasClamped = false;

      Vector3 localScale = zoomable.Transform.localScale;
      for (int i = 0; i < 2; i++)
      {
        if (!zoomable.FreezeScale[i])
        {
          hasClamped |= !zoomable.ScaleRange[i].ContainsValue(newValue[i]);
          localScale[i] = zoomable.ScaleRange[i].Clamp(newValue[i]);
        }
      }
      zoomable.Transform.localScale = localScale;

      return hasClamped;
    }

    protected virtual IEnumerator SetSelected(ISelectable selectable)
    {
      yield return null; // wait the next frame

      if (selectable.IsInteractable && selectable.IsSelectable)
      {
        selectable.SetSelected(true);
      }
    }
  }
}