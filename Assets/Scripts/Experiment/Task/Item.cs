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

    [Header("Background")]
    [SerializeField]
    private Renderer background;

    [SerializeField]
    private Material classifiedMaterial;

    [SerializeField]
    private Material focusedClassifiedMaterial;

    [SerializeField]
    private Material notClassifiedMaterial;

    [SerializeField]
    private Material focusedNotClassifiedMaterial;

    [SerializeField]
    private Material selectedMaterial;

    [SerializeField]
    private Material focusedSelectedClassifiedMaterial;

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

    public bool Classified { get; protected set; }

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
      border.transform.localScale = new Vector3(Scale.x, Scale.y, 0);

      Collider.center = Vector3.zero;
      Collider.radius = 0.5f * Scale.x;
    }

    public void SetClassified(bool value)
    {
      Classified = value;
      UpdateBackground();
    }

    protected virtual void UpdateBackground()
    {
      if (IsSelected)
      {
        background.material = (IsFocused) ? focusedSelectedClassifiedMaterial : selectedMaterial;
      }
      else if (Classified)
      {
        background.material = (IsFocused) ? focusedClassifiedMaterial : classifiedMaterial;
      }
      else
      {
        background.material = (IsFocused) ? focusedNotClassifiedMaterial : notClassifiedMaterial;
      }
    }
  }
}
