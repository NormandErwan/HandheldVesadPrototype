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
    private Material correctlyClassifiedMaterial;

    [SerializeField]
    private Material incorrectlyClassifiedMaterial;

    [SerializeField]
    private Image background;

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

    // Variables

    private ItemClass itemClass;

    // Methods

    public void SetCorrectlyClassified(bool value)
    {
      CorrectlyClassified = value;
      background.material = (CorrectlyClassified) ? correctlyClassifiedMaterial : incorrectlyClassifiedMaterial;
    }
  }
}
