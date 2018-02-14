using Leap.Unity;
using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class LookCursorInput : CursorsInput<LookCursor>
  {
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

    public bool TapWithRightHand { get; set; }

    // Variables

    protected Vector3 defaultScale;

    // Methods

    protected override void Awake()
    {
      base.Awake();

      Cursors.Add(lookCursor.Type, lookCursor);
      defaultScale = lookCursor.transform.localScale;
    }

    protected override void UpdateCursors()
    {
      lookCursor.transform.position = head.position + defaultLookCursorDistance * head.forward;
      lookCursor.transform.LookAt(head);
      lookCursor.transform.localScale = Vector3.Distance(head.position, lookCursor.transform.position) / defaultLookCursorDistance * defaultScale;

      if (lookCursor.IsActive)
      {
        lookCursor.SetVisible(true);
      }
    }
  }
}
