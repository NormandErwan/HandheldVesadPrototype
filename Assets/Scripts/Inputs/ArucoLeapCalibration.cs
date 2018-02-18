using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  /// <summary>
  /// Aligns the <see cref="leapHandController"/> with the aruco coordinates: point your right index <see cref="leapIndexEnd"/> to the
  /// <see cref="targetSphere"/> that is a child of an Aruco Object and press the <see cref="aligningKey"/> to update the position of the leap hand
  /// controller.
  /// </summary>
  public class ArucoLeapCalibration : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private bool activate;

    [SerializeField]
    private Transform targetSphere;

    [SerializeField]
    private KeyCode aligningKey = KeyCode.L;

    // Properties

    public bool ParticipantIsRightHanded { get; set; }
    public LeapFingerCursorsInput LeapFingerCursorsInput { get; set; }
    public Vector3 CursorsPositionOffset { get; protected set; }

    // Methods

    protected void Start()
    {
      targetSphere.gameObject.SetActive(activate);
    }

    protected void Update()
    {
      if (Input.GetKeyUp(aligningKey))
      {
        var cursorType = (ParticipantIsRightHanded) ? CursorType.RightIndex : CursorType.LeftIndex;
        var indexCursor = LeapFingerCursorsInput.Cursors[cursorType];
        
        CursorsPositionOffset += targetSphere.position - indexCursor.transform.position;
        print("CursorsPositionOffset: " + CursorsPositionOffset.ToString("F4"));
      }

      if (activate && LeapFingerCursorsInput != null && LeapFingerCursorsInput.CursorsPositionOffset != CursorsPositionOffset)
      {
        LeapFingerCursorsInput.CursorsPositionOffset = CursorsPositionOffset;
      }
    }
  }
}