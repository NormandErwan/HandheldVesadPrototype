using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class CursorTriggerIZoomable : CursorTriggerITransformable<IZoomable>
  {
    protected override void OnTriggerEnter(IZoomable zoomable, Collider other)
    {
      base.OnTriggerEnter(zoomable, other);
      if (Cursor.IsFinger && zoomable.IsTransformable)
      {
        if (latestCursorPositions[zoomable].Count == 2 && !zoomable.DragToZoom && !zoomable.IsZooming)
        {
          zoomable.SetZooming(true);
        }
        else if (latestCursorPositions[zoomable].Count > 2 && zoomable.IsZooming)
        {
          zoomable.SetZooming(false);
        }
      }
    }

    protected override void OnTriggerStay(IZoomable zoomable, Collider other)
    {
      if (zoomable.IsTransformable && latestCursorPositions.ContainsKey(zoomable))
      {
        if (latestCursorPositions[zoomable].Count == 1)
        {
          // Zoom with one finger if DragToZoom is true
          if (zoomable.DragToZoom && !zoomable.IsZooming)
          {
            var translation = zoomable.ProjectPosition(Cursor.transform.position - latestCursorPositions[zoomable][Cursor]);
            if (translation.magnitude > Cursor.MaxSelectableDistance)
            {
              zoomable.SetZooming(true);
              latestCursorPositions[zoomable][Cursor] = Cursor.transform.position;
            }
          }
          // Switch to dragging if was zooming
          else if (!zoomable.DragToZoom && zoomable.IsZooming)
          {
            var draggable = other.GetComponent<IDraggable>();
            if (draggable != null)
            {
              zoomable.SetZooming(false);
              draggable.SetDragging(true);
              latestCursorPositions[zoomable][Cursor] = Cursor.transform.position;
            }
          }
        }

        if (zoomable.IsZooming)
        {
          var latestPositions = latestCursorPositions[zoomable];
          var cursors = new List<Cursor>(latestPositions.Keys);
          if (cursors[0] == Cursor) // Update only once per frame
          {
            // Set cursors list
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
                zoomable.ProjectPosition(zoomable.DragToZoomPivot), zoomable.ProjectPosition(zoomable.DragToZoomPivot)
              };
            }

            // Computes scaling
            var distance = (zoomable.ProjectPosition(cursorPositions[0] - cursorPositions[2])).magnitude;
            var previousDistance = (zoomable.ProjectPosition(cursorPositions[1] - cursorPositions[3])).magnitude;
            float scaleFactor = (previousDistance != 0) ? distance / previousDistance : 1f;
            Vector3 scaling = ClampScaling(zoomable, scaleFactor * Vector3.one);

            if (scaling != Vector3.one)
            {
              // Translate if it has scaled
              var newPosition = cursorPositions[0] - scaleFactor * (cursorPositions[1] - zoomable.Transform.position);
              var translation = newPosition - zoomable.Transform.position;
              translation = ClampTranslation(zoomable, translation);

              zoomable.Zoom(scaling, translation);
            }

            // Update cursors
            latestPositions[cursors[0]] = cursorPositions[0];
            if (!zoomable.DragToZoom)
            {
              latestPositions[cursors[1]] = cursorPositions[2];
            }
          }
        }
      }
    }

    protected override void OnTriggerExit(IZoomable zoomable, Collider other)
    {
      base.OnTriggerExit(zoomable, other);
      if (Cursor.IsFinger && latestCursorPositions.ContainsKey(zoomable) && latestCursorPositions[zoomable].Count < 2 && zoomable.IsZooming)
      {
        zoomable.SetZooming(false);
      }
    }
  }
}
