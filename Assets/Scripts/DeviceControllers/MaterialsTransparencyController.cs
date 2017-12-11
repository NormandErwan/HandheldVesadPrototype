using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class MaterialsTransparencyController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private Material[] materials;

    [SerializeField]
    [Range(0f, 1f)]
    private float transparency = 1f;

    // Methods

    protected void OnValidate()
    {
      foreach (var material in materials)
      {
        material.color = new Color(material.color.r, material.color.g, material.color.b, transparency);
      }
    }
  }
}
