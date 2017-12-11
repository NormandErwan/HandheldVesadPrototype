using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class TouchFingerCursorsInput : CursorsInput
  {
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

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      keys = new List<CursorType>(Cursors.Keys);

      foreach (var cursor in Cursors)
      {
        cursor.Value.GetComponent<MeshRenderer>().enabled = false;
      }
    }

    // CursorsInput methods

    protected override void DeactivateCursors()
    {
      foreach (var cursor in Cursors)
      {
        cursor.Value.gameObject.SetActive(true);
        cursor.Value.transform.position = new Vector3(cursor.Value.transform.position.x, cursor.Value.transform.position.y, Camera.transform.position.z);
      }
    }

    protected override void UpdateCursors()
    {
      if (Input.GetKey(KeyCode.Mouse0))
      {
        UpdateCursor(Input.mousePosition);
      }

      for (int i = 0; i < Input.touchCount; i++)
      {
        var touch = Input.GetTouch(i);
        if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
          UpdateCursor(touch.position);
        }
      }
    }

    protected virtual void UpdateCursor(Vector3 cursorScreenPosition)
    {
      var cursorPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
      cursorPosition = Vector3.ProjectOnPlane(cursorPosition, -PlaneToProjectCursors.forward);

      var cursor = Cursors[keys[0]];
      cursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, PlaneToProjectCursors.position.z);
      cursor.transform.forward = planeToProjectCursors.forward;
    }
  }
}
