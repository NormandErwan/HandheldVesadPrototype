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
    private int borderMargins;

    [SerializeField]
    private Material borderMaterial_Selected;

    [SerializeField]
    private int borderMargins_Selected;

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

    public bool IsInteractable { get; protected set; }

    public bool IsFocused { get; protected set; }

    public bool IsSelectable { get; protected set; }
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

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IFocusable> Focused = delegate { };

    public event Action<ISelectable> Selectable = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Item> SelectedItem = delegate { };

    // Variables

    protected RectTransform rectTransform;
    protected RectTransform backgroundRectTransform;
    protected new SphereCollider collider;
    private ItemClass itemClass;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      rectTransform = GetComponent<RectTransform>();
      backgroundRectTransform = background.GetComponent<RectTransform>();
      collider = GetComponent<SphereCollider>();

      SetInteractable(true);
      SetFocused(false);
      SetSelectable(true);
      SetSelected(false);
    }

    // Interfaces methods

    public void SetInteractable(bool value)
    {
      IsInteractable = value;
      if (IsInteractable)
      {
        Interactable(this);
      }
    }

    public void SetFocused(bool value)
    {
      IsFocused = value;
      if (IsFocused)
      {
        Focused(this);
      }
      UpdateBackgroundMaterial();
    }

    public void SetSelectable(bool value)
    {
      IsSelectable = value;
      if (IsSelectable)
      {
        Selectable(this);
      }
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
      backgroundRectTransform.offsetMin = ((IsSelected) ? borderMargins_Selected : borderMargins) * Vector2.one;
      backgroundRectTransform.offsetMax = ((IsSelected) ? -borderMargins_Selected : -borderMargins) * Vector2.one;
    }

    // Methods

    public void Configure()
    {
      collider.center = 0.5f * new Vector3(rectTransform.sizeDelta.x, -rectTransform.sizeDelta.y, 0);
      collider.radius = 0.5f * rectTransform.sizeDelta.x;
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
