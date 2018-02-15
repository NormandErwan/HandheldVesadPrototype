using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerIZoomable : FingerCursorTriggerITransformable<IZoomable>
  {
    protected override void OnTriggerEnter(IZoomable zoomable, Collider other)
    {
      base.OnTriggerEnter(zoomable, other);

      if (latestCursorPositions.ContainsKey(zoomable))
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
      base.OnTriggerStay(zoomable, other);

      if (latestCursorPositions.ContainsKey(zoomable))
      {
        if (latestCursorPositions[zoomable].Count == 1)
        {
          // Zoom with one finger if DragToZoom is true
          if (zoomable.DragToZoom && !zoomable.IsZooming)
          {
            var translation = Project(zoomable, Cursor.transform.position - latestCursorPositions[zoomable][Cursor]);
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
          var cursors = new List<FingerCursor>(latestPositions.Keys);
          if (cursors[0] == Cursor) // Update only once per frame
          {
            // Set cursors list
            var projectedZoomable = Project(zoomable, zoomable.Transform.position);

            Vector3[] cursorPositions;
            if (!zoomable.DragToZoom)
            {
              cursorPositions = new Vector3[4] {
                Project(zoomable, cursors[0].transform.position), Project(zoomable, latestPositions[cursors[0]]),
                Project(zoomable, cursors[1].transform.position), Project(zoomable, latestPositions[cursors[1]])
              };
            }
            else
            {
              cursorPositions = new Vector3[4] {
                Project(zoomable, cursors[0].transform.position), Project(zoomable, latestPositions[cursors[0]]),
                projectedZoomable, projectedZoomable
              };
            }

            // Computes scaling
            var distance = (cursorPositions[0] - cursorPositions[2]).magnitude;
            var previousDistance = (cursorPositions[1] - cursorPositions[3]).magnitude;
            float scaleFactor = (previousDistance != 0) ? distance / previousDistance : 1f;
            Vector3 scaling = ClampScaling(zoomable, scaleFactor * Vector3.one);

            // Apply zoom with a translation to keep the cursor on the same relative position on the zoomable
            if (scaling != Vector3.one)
            {
              var newZoomablePosition = cursorPositions[0] - scaleFactor * (cursorPositions[1] - projectedZoomable);
              var translation = ClampTranslation(zoomable, newZoomablePosition - projectedZoomable);
              zoomable.Zoom(scaling, translation);
            }

            // Update cursors
            latestPositions[cursors[0]] = cursors[0].transform.position;
            if (!zoomable.DragToZoom)
            {
              latestPositions[cursors[1]] = cursors[1].transform.position;
            }
          }
        }
      }
    }

    protected override void OnTriggerExit(IZoomable zoomable, Collider other)
    {
      base.OnTriggerExit(zoomable, other);

      if (latestCursorPositions.ContainsKey(zoomable) && latestCursorPositions[zoomable].Count < 2 && zoomable.IsZooming)
      {
        zoomable.SetZooming(false);
      }
    }
  }
}
