using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  public abstract class CursorTriggerITransformable<T> : CursorTriggerIInteractable<T> where T : ITransformable
  {
    // Variables

    protected static Dictionary<ITransformable, Dictionary<Cursor, Vector3>> latestCursorPositions
      = new Dictionary<ITransformable, Dictionary<Cursor, Vector3>>();

    // Methods

    protected override void OnTriggerEnter(T transformable, Collider other)
    {
      if (Cursor.IsFinger && transformable.IsTransformable)
      {
        if (!latestCursorPositions.ContainsKey(transformable))
        {
          latestCursorPositions.Add(transformable, new Dictionary<Cursor, Vector3>());
        }
        latestCursorPositions[transformable][Cursor] = Cursor.transform.position;
      }
    }

    protected override void OnTriggerExit(T transformable, Collider other)
    {
      if (Cursor.IsFinger && latestCursorPositions.ContainsKey(transformable))
      {
        latestCursorPositions[transformable].Remove(Cursor);
      }
    }

    protected Vector3 ClampTranslation(ITransformable transformable, Vector3 translation)
    {
      Vector3 clampedTranslation = Vector3.zero;
      for (int i = 0; i < 2; i++)
      {
        clampedTranslation[i] = transformable.PositionRange[i].Clamp(transformable.Transform.localPosition[i] + translation[i])
          - transformable.Transform.localPosition[i];
      }
      return clampedTranslation;
    }

    protected Vector3 ClampScaling(IZoomable zoomable, Vector3 scaling)
    {
      Vector3 clampedScaling = Vector3.one;
      for (int i = 0; i < 2; i++)
      {
        clampedScaling[i] = zoomable.ScaleRange[i].Clamp(zoomable.Transform.localScale[i] * scaling[i])
          / zoomable.Transform.localScale[i];
      }
      return clampedScaling;
    }
  }
}
