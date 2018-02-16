using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Container : Grid<Container, Item>, IFocusable, ILongPressable, ITappable
  {
    // Editor fields

    [SerializeField]
    private int interactablePriority;

    [Header("Background")]
    [SerializeField]
    private Renderer background;

    [SerializeField]
    private Material backgroundMaterial;

    [SerializeField]
    private Material backgroundMaterial_Focused;

    // Interfaces properties

    public int Priority { get { return interactablePriority; } }
    public IInteractable Parent { get; set; }
    public Transform Transform { get { return transform; } }

    public bool IsInteractable { get; protected set; }

    public bool IsFocused { get; protected set; }

    public bool IsSelectable { get; protected set; }
    public bool IsSelected { get; protected set; }

    public bool IsLongPressable { get; protected set; }
    public bool IsTappable { get; protected set; }

    // Properties

    public ItemClass ItemClass { get; set; }
    public int ItemFontSize { get; set; }

    // Events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IFocusable> Focused = delegate { };
    public event Action<Container> Focused2 = delegate { };

    public event Action<ISelectable> Selectable = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Container> Selected2 = delegate { };

    public event Action<ILongPressable> LongPressable = delegate { };
    public event Action<ITappable> Tappable = delegate { };

    // Variables

    protected new BoxCollider collider;
    protected int focusedItems = 0;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      ElementsInstantiatedAtConfigure = (GridSize.x * GridSize.y) - 1; // -1 because we want to let space for the user to replace each item in its good cell

      collider = GetComponent<BoxCollider>();

      SetSelectable(true);
      SetLongPressable(false);
      SetTappable(false);
      UpdateBackground();
    }

    // GridLayoutController methods

    public override void Configure()
    {
      // Configure the grid
      ElementScale = Scale - ElementMargin;
      ElementScale = new Vector2(ElementScale.x / GridSize.x, ElementScale.y / GridSize.y) - ElementMargin;
      base.Configure();

      // Compute the item size
      float itemSize = Mathf.Min(ElementScale.x, ElementScale.y);
      ElementScale = new Vector2(itemSize, itemSize);
      foreach (var item in Elements)
      {
        item.Parent = this;
        item.Scale = ElementScale;
      }

      // Configure the collider
      collider.center = new Vector3(0f, 0f, itemSize);
      collider.size = new Vector3(Scale.x, Scale.y, 3f * itemSize);

      // Configure the background
      background.transform.localScale = new Vector3(Scale.x, Scale.y, 1);
    }

    // Interfaces methods

    public override Container Instantiate()
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
        Focused2(this);
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
      if (value == true)
      {
        IsSelected = true;
        Selected(this);
        Selected2(this);

        IsSelected = false;
        Selected(this);
        Selected2(this);
      }
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

    public virtual void ConfigureItems(int[] itemValues)
    {
      int index = 0;
      foreach (var item in Elements)
      {
        item.ItemClass = (ItemClass)itemValues[index];
        item.FontSize = ItemFontSize;
        item.SetClassified(item.ItemClass == ItemClass);
        item.Configure();

        index++;
      }
    }

    public override void Append(Item item)
    {
      item.SetClassified(item.ItemClass == ItemClass);
      base.Append(item);
    }

    protected virtual void UpdateBackground()
    {
      background.material = (IsFocused && focusedItems == 0) ? backgroundMaterial_Focused : backgroundMaterial;
    }
  }
}
