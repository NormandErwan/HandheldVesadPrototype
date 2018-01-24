using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
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
    public bool ParticipantIsRightHanded { get; set; }

    // Variables

    protected Vector3 mouse1Position = Vector3.zero;
    protected bool updateCursor1;

    // CursorsInput methods

    protected override void UpdateCursors()
    {
#if UNITY_EDITOR || !UNITY_ANDROID
      if (Input.GetKey(KeyCode.Mouse0))
      {
        UpdateCursor(GetIndex(), Input.mousePosition);
      }

      if (Input.GetKeyUp(KeyCode.Mouse1))
      {
        updateCursor1 = !updateCursor1;
        mouse1Position = Input.mousePosition;
      }
      if (updateCursor1)
      {
        UpdateCursor(GetThumb(), mouse1Position);
      }
#endif

      for (int i = 0; i < Input.touchCount && i < Cursors.Count; i++)
      {
        var touch = Input.GetTouch(i);
        if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
          switch (i)
          {
            case 0: UpdateCursor(GetIndex(), touch.position); break;
            case 1: UpdateCursor(GetThumb(), touch.position); break;
            default: break;
          }
        }
      }
    }

    protected virtual CursorType GetIndex()
    {
      return (ParticipantIsRightHanded) ? CursorType.RightIndex : CursorType.LeftIndex;
    }

    protected virtual CursorType GetThumb()
    {
      return (ParticipantIsRightHanded) ? CursorType.RightThumb : CursorType.LeftThumb;
    }

    protected virtual void UpdateCursor(CursorType cursorType, Vector3 cursorScreenPosition)
    {
      var cursorPosition = Camera.ScreenToWorldPoint(cursorScreenPosition);
      cursorPosition = Vector3.ProjectOnPlane(cursorPosition, -PlaneToProjectCursors.forward);

      var cursor = Cursors[cursorType];
      cursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, PlaneToProjectCursors.position.z);
      cursor.transform.forward = planeToProjectCursors.forward;
      cursor.transform.localScale = fingerWidth * Vector3.one;
      cursor.SetVisible(true);
    }
  }
}
