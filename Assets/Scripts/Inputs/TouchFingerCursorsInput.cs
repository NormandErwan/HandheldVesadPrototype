using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class TouchFingerCursorsInput : CursorsInput
  {
    // Constants

    protected readonly static float fingerWidth = 0.005f;

    // Editor fields

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private Transform planeToProjectCursors;

    // Properties

    public Camera Camera { get { return camera; } set { camera = value; } }
    public Transform PlaneToProjectCursors { get { return planeToProjectCursors; } set { planeToProjectCursors = value; } }

    // Variables

    protected List<CursorType> keys;
    protected Vector3 mouse1Position = Vector3.zero;
    protected bool updateCursor1;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      keys = new List<CursorType>(Cursors.Keys);
    }

    // CursorsInput methods

    protected override void DeactivateCursors()
    {
      base.DeactivateCursors();
      foreach (var cursor in Cursors)
      {
        cursor.Value.transform.position = new Vector3(cursor.Value.transform.position.x, cursor.Value.transform.position.y, Camera.transform.position.z);
      }
    }

    protected override void UpdateCursors()
    {
#if UNITY_EDITOR
      if (Input.GetKey(KeyCode.Mouse0))
      {
        UpdateCursor(0, Input.mousePosition);
      }

      if (Input.GetKeyUp(KeyCode.Mouse1))
      {
        updateCursor1 = !updateCursor1;
        mouse1Position = Input.mousePosition;
      }
      if (updateCursor1)
      {
        UpdateCursor(1, mouse1Position);
      }
#endif

      for (int i = 0; i < Input.touchCount && i < Cursors.Count; i++)
      {
        var touch = Input.GetTouch(i);
        if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
          UpdateCursor(i, touch.position);
        }
      }
    }

    protected virtual void UpdateCursor(int cursorId, Vector3 cursorScreenPosition)
    {
      var cursorPosition = Camera.ScreenToWorldPoint(cursorScreenPosition);
      cursorPosition = Vector3.ProjectOnPlane(cursorPosition, -PlaneToProjectCursors.forward);

      var cursor = Cursors[keys[cursorId]];
      cursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, PlaneToProjectCursors.position.z);
      cursor.transform.forward = planeToProjectCursors.forward;
      cursor.transform.localScale = fingerWidth * Vector3.one;

      ActivateCursor(cursor.Type);
    }
  }
}
