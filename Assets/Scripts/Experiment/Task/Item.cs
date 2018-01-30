using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(SphereCollider))]
  public class Item : MonoBehaviour, IGridElement<Item>, IFocusable, ILongPressable, ITappable
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

    public bool IsLongPressable { get; protected set; }
    public bool IsTappable { get; protected set; }

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

    public SphereCollider Collider { get; protected set; }

    // Events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IFocusable> Focused = delegate { };

    public event Action<ISelectable> Selectable = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Item> Selected2 = delegate { };

    public event Action<ILongPressable> LongPressable = delegate { };
    public event Action<ITappable> Tappable = delegate { };

    // Variables

    protected Renderer itemClassTextRenderer;

    private ItemClass itemClass;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      Collider = GetComponent<SphereCollider>();

      SetSelectable(true);
      SetLongPressable(false);
      SetTappable(false);
      UpdateBackground();
    }

    // Interfaces methods

    public virtual Item Instantiate()
    {
      return Instantiate(this);
    }

    public void SetInteractable(bool value)
    {
      if (IsInteractable != value)
      {
        IsInteractable = value;
        Interactable(this);
      }
    }

    public void SetFocused(bool value)
    {
      if (IsFocused != value)
      {
        IsFocused = value;
        Focused(this);
        UpdateBackground();
      }
    }

    public void SetSelectable(bool value)
    {
      if (IsSelectable != value)
      {
        IsSelectable = value;
        Selectable(this);
      }
    }

    public void SetSelected(bool value)
    {
      IsSelected = value;
      UpdateBackground();

      Selected(this);
      Selected2(this);
    }

    public void SetLongPressable(bool value)
    {
      if (IsLongPressable != value)
      {
        IsLongPressable = value;
        LongPressable(this);
      }
    }

    public void SetTappable(bool value)
    {
      if (IsTappable != value)
      {
        IsTappable = value;
        Tappable(this);
      }
    }

    // Methods

    public virtual void Configure()
    {
      border.transform.localScale = new Vector3(Scale.x, Scale.y, 1f);

      Collider.center = Vector3.zero;
      Collider.radius = 0.5f * Scale.x;
    }

    public void SetCorrectlyClassified(bool value)
    {
      CorrectlyClassified = value;
      UpdateBackground();
    }

    protected virtual void UpdateBackground()
    {
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
