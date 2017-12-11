using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public abstract class DeviceController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    [Range(0f, 0.02f)]
    private float maxSelectableDistance = 0.001f;

    // Properties

    public float MaxSelectableDistance { get { return maxSelectableDistance; } set { maxSelectableDistance = value; } }

    // Methods
  }
}
