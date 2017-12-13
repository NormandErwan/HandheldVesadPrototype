using ArucoUnity.Objects;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment
{
  public class MobileDeviceTracking : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private ArucoMarker[] markers;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float smoothRotationWeigth = 0.01f;

    // Variables

    protected Vector3[] positionOffsets;
    protected bool activatedPreviousFrame = false;
    protected Vector3 previousForward, previousRight, previousUp;

    // Methods

    protected virtual void Awake()
    {
      positionOffsets = new Vector3[markers.Length];
      for (int i = 0; i < markers.Length; i++)
      {
        positionOffsets[i] = transform.position - markers[i].transform.position;
      }
    }

    protected virtual void LateUpdate()
    {
      Vector3 position = Vector3.zero;
      Quaternion rotation = Quaternion.identity;

      bool activated = false;
      for (int i = 0; i < markers.Length; i++)
      {
        if (markers[i].gameObject.activeSelf)
        {
          if (!activated)
          {
            activated = true;
            position = markers[i].transform.position + positionOffsets[i];
            rotation = markers[i].transform.rotation;
          }
        }
      }

      if (activated)
      {
        transform.position = position;
        transform.rotation = rotation;

        // Smooth the rotation
        if (activatedPreviousFrame)
        {
          Vector3 newForward = (smoothRotationWeigth * transform.forward + (1 - smoothRotationWeigth) * previousForward).normalized;
          Vector3 newRight = (smoothRotationWeigth * transform.right + (1 - smoothRotationWeigth) * previousRight).normalized;
          Vector3 newUp = Vector3.Cross(newForward, newRight).normalized;
          transform.rotation = Quaternion.LookRotation(newForward, newUp);
        }
        previousForward = transform.forward;
        previousRight = transform.right;
        previousUp = transform.up;
        activatedPreviousFrame = true;
      }
      else
      {
        transform.position = camera.transform.position - camera.transform.forward;
        transform.rotation = camera.transform.rotation;
        activatedPreviousFrame = false;
      }
    }
  }
}
