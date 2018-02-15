using Leap.Unity;
using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class LookCursorInput : CursorsInput<LookCursor>
  {
    // Constants

    public const float tapTimeout = 0.5f; // In seconds
    public const float minDistanceToPinch = 20f;

    // Editor fields

    [SerializeField]
    private LookCursor lookCursor;

    [SerializeField]
    private Transform head;

    [SerializeField]
    private LeapServiceProvider leapServiceProvider;

    [SerializeField]
    private float defaultLookCursorDistance = 1f;

    [SerializeField]
    private float maxLookDistance = 10f;

    // Propreties

    public bool ParticipantIsRightHanded { get; set; }

    // Variables

    protected Vector3 defaulLookCursorScale = Vector3.zero;
    protected float lastReadyTime = 0;
    protected float lastPinchTime = 0;

    // Methods

    protected override void Awake()
    {
      base.Awake();

      Cursors.Add(lookCursor.Type, lookCursor);
      defaulLookCursorScale = lookCursor.transform.localScale;
    }

    protected override void UpdateCursors()
    {
      foreach (var hand in leapServiceProvider.CurrentFrame.Hands)
      {
        if (hand.IsRight == ParticipantIsRightHanded)
        {
          if (hand.GetIndex().IsExtended && hand.GetThumb().IsExtended)
          {
            if (!lookCursor.IsReady)
            {
              if (lastPinchTime - lastReadyTime < tapTimeout)
              {
                lookCursor.SetTapped();
              }

              lookCursor.SetReady(true);
            }
            lastReadyTime = Time.time;
          }
          else
          {
            if (lookCursor.IsReady)
            {
              lookCursor.SetReady(false);
            }

            if (hand.PinchDistance < minDistanceToPinch)
            {
              lastPinchTime = Time.time;
            }
          }
        }
      }

      lookCursor.transform.position = head.position + defaultLookCursorDistance * head.forward;
      lookCursor.transform.LookAt(head);
      lookCursor.transform.localScale = Vector3.Distance(head.position, lookCursor.transform.position) / defaultLookCursorDistance * defaulLookCursorScale;

      if (lookCursor.IsActive)
      {
        lookCursor.SetVisible(true);
      }
    }
  }
}
