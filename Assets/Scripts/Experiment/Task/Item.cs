using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class Item : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private Text itemClassText;

    [SerializeField]
    private new SphereCollider collider;

    [SerializeField]
    private Image border;

    [SerializeField]
    private Material borderMaterial;

    [SerializeField]
    private Material borderMaterial_Selected;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Material correctlyClassifiedMaterial;

    [SerializeField]
    private Material correctlyClassifiedMaterial_Focused;

    [SerializeField]
    private Material incorrectlyClassifiedMaterial;

    [SerializeField]
    private Material incorrectlyClassifiedMaterial_Focused;

    // Properties

    public ItemClass ItemClass
    {
      get
      {
        return itemClass;
      }
      set
      {
        itemClass = value;
        itemClassText.text = new string((char)((int)itemClass + 65), 1);
      }
    }

    public int FontSize
    {
      get
      {
        return itemClassText.fontSize;
      }
      set
      {
        itemClassText.fontSize = value;
      }
    }

    public bool CorrectlyClassified { get; protected set; }

    public bool Focused { get; protected set; }

    public bool Selected { get; protected set; }

    // Variables

    private ItemClass itemClass;

    // Methods

    public void Configure()
    {
      var rectSizeDelta = GetComponent<RectTransform>().sizeDelta;
      collider.center = 0.5f * new Vector3(rectSizeDelta.x, -rectSizeDelta.y, 0);
      collider.radius = 0.5f * rectSizeDelta.x;

      background.material = correctlyClassifiedMaterial;
      border.material = borderMaterial;
    }

    public void SetCorrectlyClassified(bool value)
    {
      CorrectlyClassified = value;
      background.material = (CorrectlyClassified) ? correctlyClassifiedMaterial : incorrectlyClassifiedMaterial;
    }

    public void ToggleFocused()
    {
      Focused = !Focused;
      if (CorrectlyClassified)
      {
        background.material = (Focused) ? correctlyClassifiedMaterial_Focused : correctlyClassifiedMaterial;
      }
      else
      {
        background.material = (Focused) ? incorrectlyClassifiedMaterial_Focused : incorrectlyClassifiedMaterial;
      }
    }

    public void ToggleSelected()
    {
      Selected = !Selected;
      border.material = (Selected) ? borderMaterial_Selected : borderMaterial;
    }
  }
}
