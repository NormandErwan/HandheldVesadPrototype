using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Container : Grid<Container, Item>, IFocusable, ITappable
  {
    // Editor fields

    [Header("Background")]
    [SerializeField]
    private Renderer background;

    [SerializeField]
    private Material backgroundMaterial;

    [SerializeField]
    private Material backgroundMaterial_Focused;

    // Interfaces properties

    public bool IsInteractable { get; protected set; }

    public bool IsFocused { get; protected set; }

    public bool IsSelectable { get; protected set; }
    public bool IsSelected { get; protected set; }

    // Properties

    public ItemClass ItemClass { get; set; }
    public int ItemFontSize { get; set; }

    // Events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IFocusable> Focused = delegate { };

    public event Action<ISelectable> Selectable = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Container> Selected2 = delegate { };

    // Variables

    protected new BoxCollider collider;
    protected int focusedItems = 0;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      ElementsInstantiatedAtConfigure = (GridSize.x * GridSize.y) - 1; // -1 because we want to let space for the user to replace each item in its good cell

      collider = GetComponent<BoxCollider>();

      SetInteractable(true);
      SetFocused(false);
      SetSelectable(true);
      SetSelected(false);
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
      IsInteractable = value;
      Interactable(this);
    }

    public void SetFocused(bool value)
    {
      IsFocused = value;
      Focused(this);

      focusedItems = 0;
      foreach (var item in Elements)
      {
        if (item.IsFocused)
        {
          focusedItems++;
        }
      }
      UpdateBackground();
    }

    public void SetSelectable(bool value)
    {
      IsSelectable = value;
      Selectable(this);
    }

    public void SetSelected(bool value)
    {
      IsSelected = value;
      Selected(this);
      Selected2(this);
      IsSelected = false;
    }

    // Methods

    public virtual void ConfigureItems(int[] itemValues)
    {
      int index = 0;
      foreach (var item in Elements)
      {
        item.ItemClass = (ItemClass)itemValues[index];
        item.FontSize = ItemFontSize;
        item.SetCorrectlyClassified(item.ItemClass == ItemClass);
        item.Configure();

        item.Focused += Item_Focused;

        index++;
      }
    }

    public override void Append(Item item)
    {
      item.SetCorrectlyClassified(item.ItemClass == ItemClass);
      item.Focused += Item_Focused;
      base.Append(item);
    }

    public override void Remove(Item item)
    {
      item.Focused -= Item_Focused;
      base.Remove(item);
    }

    protected virtual void UpdateBackground()
    {
      background.material = (IsFocused && focusedItems == 0) ? backgroundMaterial_Focused : backgroundMaterial;
    }

    protected virtual void Item_Focused(IFocusable item)
    {
      focusedItems++;
    }
  }
}
