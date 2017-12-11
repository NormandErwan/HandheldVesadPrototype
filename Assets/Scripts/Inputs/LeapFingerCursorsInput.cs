using Leap;
using Leap.Unity;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class LeapFingerCursorsInput : MonoBehaviour
  {
    // Constants

    protected const float oneMillimeterToMeters = 0.001f;

    // Editor fields

    [SerializeField]
    private Cursor[] cursors;

    [SerializeField]
    private LeapServiceProvider leapServiceProvider;

    [SerializeField]
    private bool useStabilizedPositions;

    // Properties

    public Dictionary<CursorType, Cursor> Cursors { get; protected set; }
    public LeapServiceProvider LeapServiceProvider { get { return leapServiceProvider; } set { leapServiceProvider = value; } }
    public bool UseStabilizedPositions { get { return useStabilizedPositions; } set { useStabilizedPositions = value; } }

    // Methods

    protected virtual void Awake()
    {
      Cursors = new Dictionary<CursorType, Cursor>();
      foreach (var cursor in cursors)
      {
        Cursors.Add(cursor.Type, cursor);
      }
    }

    protected virtual void Update()
    {
      // Deactivates the cursors
      foreach (var cursor in Cursors)
      {
        cursor.Value.gameObject.SetActive(false);
      }

      // Activates the detected cursors
      foreach (var hand in LeapServiceProvider.CurrentFrame.Hands)
      {
        foreach (var finger in hand.Fingers)
        {
          var cursorType = GetCursorType(hand.IsRight, finger.Type);

          Cursor cursor;
          if (Cursors.TryGetValue(cursorType, out cursor))
          {
            cursor.gameObject.SetActive(true);
            cursor.transform.position = (UseStabilizedPositions ? finger.StabilizedTipPosition : finger.TipPosition).ToVector3();
            cursor.transform.forward = finger.Direction.ToVector3();
            cursor.transform.localScale = finger.Width * oneMillimeterToMeters * Vector3.one;
          }
        }
      }
    }

    protected virtual CursorType GetCursorType(bool rightHand, Finger.FingerType fingerType)
    {
      switch (fingerType)
      {
        case Finger.FingerType.TYPE_THUMB: return (rightHand) ? CursorType.RightThumb : CursorType.LeftThumb;
        case Finger.FingerType.TYPE_INDEX: return (rightHand) ? CursorType.RightIndex : CursorType.LeftIndex;
        case Finger.FingerType.TYPE_MIDDLE: return (rightHand) ? CursorType.RightMiddle : CursorType.LeftMiddle;
        case Finger.FingerType.TYPE_RING: return (rightHand) ? CursorType.RightRing : CursorType.LeftRing;
        case Finger.FingerType.TYPE_PINKY: return (rightHand) ? CursorType.RightPinky : CursorType.LeftPinky;
        default: return CursorType.Look;
      }
    }
  }
}
