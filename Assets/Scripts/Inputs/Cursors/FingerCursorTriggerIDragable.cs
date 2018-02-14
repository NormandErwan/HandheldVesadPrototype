using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public class FingerCursorTriggerIDraggable : FingerCursorTriggerITransformable<IDraggable>
  {
    protected override void OnTriggerEnter(IDraggable draggable, Collider other)
    {
      base.OnTriggerEnter(draggable, other);
      if (draggable.IsTransformable && latestCursorPositions[draggable].Count > 1 && draggable.IsDragging)
      {
        draggable.SetDragging(false); // Only one finger can drag, cancel if more than one finger
      }
    }

    protected override void OnTriggerStay(IDraggable draggable, Collider other)
    {
      if (draggable.IsTransformable && latestCursorPositions.ContainsKey(draggable) && latestCursorPositions[draggable].Count == 1)
      {
        var zoomable = other.GetComponent<IZoomable>();
        if (zoomable != null && zoomable.DragToZoom)
        {
          // Switch to zooming if was dragging
          if (draggable.IsDragging)
          {
            draggable.SetDragging(false);
            zoomable.SetZooming(true);
            latestCursorPositions[draggable][Cursor] = Cursor.transform.position;
          }
        }
        // Drag if not zooming with DragToZoom to true
        else
        {
          // Computes the translation
          var translation = draggable.ProjectPosition(Cursor.transform.position - latestCursorPositions[draggable][Cursor]);
          translation = ClampTranslation(draggable, translation);
          if (translation != Vector3.zero)
          {
            // Drag
            if (draggable.IsDragging)
            {
              draggable.Drag(translation);
              latestCursorPositions[draggable][Cursor] = Cursor.transform.position;
            }
            // Start dragging if it has moved enough
            else if (translation.magnitude > Cursor.MaxSelectableDistance)
            {
              draggable.SetDragging(true);
              latestCursorPositions[draggable][Cursor] = Cursor.transform.position;
            }
          }
        }
      }
    }

    protected override void OnTriggerExit(IDraggable draggable, Collider other)
    {
      base.OnTriggerExit(draggable, other);
      if (latestCursorPositions.ContainsKey(draggable) && latestCursorPositions[draggable].Count == 0 && draggable.IsDragging)
      {
        draggable.SetDragging(false);
      }
    }
  }
}
