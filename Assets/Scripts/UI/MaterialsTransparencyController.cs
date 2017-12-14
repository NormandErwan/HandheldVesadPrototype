using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.UI
{
  public class MaterialsTransparencyController : MonoBehaviour
  {
    // Constants

    protected const float opaqueAlpha = 1f;

    // Editor fields

    [SerializeField]
    private Material[] materials;

    [SerializeField]
    [Range(0f, 1f)]
    private float transparency = 1f;

    // Methods

    protected virtual void OnEnable()
    {
      foreach (var material in materials)
      {
        material.color = new Color(material.color.r, material.color.g, material.color.b, transparency);
      }
    }

    protected virtual void OnDisable()
    {
      foreach (var material in materials)
      {
        material.color = new Color(material.color.r, material.color.g, material.color.b, opaqueAlpha);
      }
    }
  }
}
