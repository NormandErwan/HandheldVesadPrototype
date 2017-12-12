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

    // Variables

    protected Vector3 positionOffset;

    // Methods

    protected virtual void Awake()
    {
      positionOffset = transform.position - markers[0].transform.position;
      print(positionOffset.ToString("F3"));
    }

    protected virtual void LateUpdate()
    {
      bool activated = markers[0].gameObject.activeSelf;
      if (activated)
      {
        Vector3 position = markers[0].transform.position;
        Quaternion rotation = markers[0].transform.rotation;

        // Compute average position for the activated markers
        /*int activeMarkers = 0;
        foreach (var marker in markers)
        {
          if (marker.gameObject.activeSelf)
          {
            if (!activated)
            {
              rotation = marker.transform.rotation.eulerAngles;
              activated = true;
            }
            position += marker.transform.position;
            activeMarkers++;
          }
        }

        if (activeMarkers > 0)
        {
          position *= 1 / activeMarkers;
        }*/

        transform.position = position + positionOffset;
        transform.rotation = rotation;
      }
      else
      {
        transform.position = camera.transform.position - camera.transform.forward;
        transform.rotation = camera.transform.rotation;
      }
    }
  }
}
