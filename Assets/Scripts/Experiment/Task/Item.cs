using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(SphereCollider))]
  public class Item : MonoBehaviour, IGridElement<Item>, IFocusable, ILongPressable
  {
    // Editor fields

    [Header("Text")]
    [SerializeField]
    private TextMesh itemClassText;

    [Header("Border")]
    [SerializeField]
    private Renderer border;

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
    private Renderer background;

    [SerializeField]
    private Material correctlyClassifiedMaterial;

    [SerializeField]
    private Material correctlyClassifiedMaterial_Focused;

    [SerializeField]
    private Material incorrectlyClassifiedMaterial;

    [SerializeField]
    private Material incorrectlyClassifiedMaterial_Focused;

    // Interfaces properties

    public GameObject GameObject { get { return gameObject; } }
    public Vector2 Scale { get; set; }
    public Vector2 Margin { get; set; }

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

    protected new SphereCollider collider;
    private ItemClass itemClass;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      collider = GetComponent<SphereCollider>();

      SetInteractable(false);
      SetSelectable(false);
    }

    // Interfaces methods

    public virtual Item Instantiate()
    {
      return Instantiate(this);
    }

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

      if (IsSelected)
      {
        border.material = borderMaterial_Selected;
        background.transform.localScale = new Vector3(Scale.x - 2 * borderMargins_Selected, Scale.y - 2 * borderMargins_Selected, 1f);
      }
      else
      {
        border.material = borderMaterial;
        background.transform.localScale = new Vector3(Scale.x - 2 * borderMargins, Scale.y - 2 * borderMargins, 1f);
      }
    }

    // Methods

    public virtual void Configure()
    {
      // Configure the border
      border.transform.localScale = new Vector3(Scale.x, Scale.y, 1f);

      // Configure the collider
      collider.center = Vector3.zero;
      collider.radius = 0.5f * Scale.x;

      // Configure interactions
      SetInteractable(true);
      SetFocused(false);
      SetSelectable(true);
      SetSelected(false);
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
