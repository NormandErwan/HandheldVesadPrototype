using NormandErwan.MasterThesisExperiment.Inputs;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  [RequireComponent(typeof(SphereCollider))]
  public class Item : MonoBehaviour, IFocusable, ILongPressable
  {
    // Editor fields

    [Header("Text")]
    [SerializeField]
    private Text itemClassText;

    [Header("Border")]
    [SerializeField]
    private Image border;

    [SerializeField]
    private Material borderMaterial;

    [SerializeField]
    private Material borderMaterial_Selected;

    [Header("Background")]
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

    // Interfaces properties

    public bool IsFocused { get; protected set; }
    public bool IsSelected { get; protected set; }

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

    // Events

    public event Action<IFocusable> Focused = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Item> SelectedItem = delegate { };

    // Variables

    protected new SphereCollider collider;
    private ItemClass itemClass;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      collider = GetComponent<SphereCollider>();
      SetFocused(false);
      SetSelected(false);
    }

    // Interfaces methods

    public void SetFocused(bool value)
    {
      IsFocused = value;
      if (IsFocused)
      {
        Focused(this);
      }
      UpdateBackgroundMaterial();
    }

    public void SetSelected(bool value)
    {
      IsSelected = value & !IsSelected;
      if (IsSelected)
      {
        Selected(this);
        SelectedItem(this);
      }
      border.material = (IsSelected) ? borderMaterial_Selected : borderMaterial;
    }

    // Methods

    public void Configure()
    {
      var rectSizeDelta = GetComponent<RectTransform>().sizeDelta;
      collider.center = 0.5f * new Vector3(rectSizeDelta.x, -rectSizeDelta.y, 0);
      collider.radius = 0.5f * rectSizeDelta.x;
    }

    public void SetCorrectlyClassified(bool value)
    {
      CorrectlyClassified = value;
      UpdateBackgroundMaterial();
    }

    protected virtual void UpdateBackgroundMaterial()
    {
      if (CorrectlyClassified)
      {
        background.material = (IsFocused) ? correctlyClassifiedMaterial_Focused : correctlyClassifiedMaterial;
      }
      else
      {
        background.material = (IsFocused) ? incorrectlyClassifiedMaterial_Focused : incorrectlyClassifiedMaterial;
      }
    }
  }
}
