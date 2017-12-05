using Leap.Unity;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Utilities
{
  /// <summary>
  /// Aligns the <see cref="leapHandController"/> with the aruco coordinates: point your right index <see cref="leapHandRight_IndexEnd"/> to the
  /// <see cref="targetSphere"/> that is a child of an Aruco Object and press the <see cref="calibrateKey"/> to update the position of the leap hand
  /// controller.
  /// </summary>
  public class ArucoLeapHandsAlignment : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private LeapHandController leapHandController;

    [SerializeField]
    private Transform leapHandRight_IndexEnd;

    [SerializeField]
    private Transform targetSphere;

    [SerializeField]
    private KeyCode calibrateKey = KeyCode.C;

    [SerializeField]
    private Vector3 defaultLeapHandControllerOffset = new Vector3(0.0014f, -0.0412f, -0.0115f);

    // Properties

    public Vector3 LeapHandArucoMarkerPositionOffset { get; protected set; }

    // Methods

    protected virtual void Awake()
    {
      LeapHandArucoMarkerPositionOffset = defaultLeapHandControllerOffset;
    }

    protected virtual void Update()
    {
      if (Input.GetKeyUp(calibrateKey))
      {
        leapHandController.transform.position -= LeapHandArucoMarkerPositionOffset;
        LeapHandArucoMarkerPositionOffset = targetSphere.position - leapHandRight_IndexEnd.position;
        leapHandController.transform.position += LeapHandArucoMarkerPositionOffset;
      }
    }
  }
}