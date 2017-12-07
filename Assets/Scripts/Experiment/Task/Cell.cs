using NormandErwan.MasterThesisExperiment.Inputs;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Cell : GridLayoutController<Item>, IFocusable, ITappable
  {
    // Editor fields

    [Header("Background")]
    [SerializeField]
    private Image background;

    [SerializeField]
    private Material backgroundMaterial;

    [SerializeField]
    private Material backgroundMaterial_Focused;

    // Interfaces properties

    public bool IsFocused { get; protected set; }
    public bool IsSelected { get; protected set; }

    // Properties

    public ItemClass ItemClass { get; set; }
    public int ItemFontSize { get; set; }

    // Events

    public event Action<IFocusable> Focused = delegate { };
    public event Action<ISelectable> Selected = delegate { };
    public event Action<Cell> SelectedCell = delegate { };

    // Variables

    protected new BoxCollider collider;
    protected int focusedItems = 0;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      collider = GetComponent<BoxCollider>();
      SetFocused(false);
      SetSelected(false);
    }

    // GridLayoutController methods

    public override void ConfigureGrid()
    {
      // Compute the item size
      var rectSizeDelta = GetComponent<RectTransform>().sizeDelta;
      int cellSize = Mathf.Min((int)rectSizeDelta.x / GridSize.x, (int)rectSizeDelta.y / GridSize.y) - 2 * CellMargins;
      CellSize = new Vector2Int(cellSize, cellSize);

      // Configure the collider
      collider.center = Vector3.zero;
      collider.size =  new Vector3(rectSizeDelta.x, rectSizeDelta.y, 2f / 3f * cellSize);

      // Configure the grid
      CellsNumberInstantiatedAtConfigure = (GridSize.x * GridSize.y) - 1; // -1 because we want to let space for the user to replace each item in its good cell
      base.ConfigureGrid();
    }

    // Interfaces methods

    public void SetFocused(bool value)
    {
      IsFocused = value;
      if (IsFocused)
      {
        Focused(this);
      }

      focusedItems = 0;
      foreach (var item in GetCells())
      {
        if (item.IsFocused)
        {
          focusedItems++;
        }
      }
      UpdateBackground();
    }

    public void SetSelected(bool value)
    {
      IsSelected = value;
      if (IsSelected)
      {
        Selected(this);
        SelectedCell(this);
        IsSelected = false;
      }
    }

    // Methods

    public virtual void ConfigureItems(int[] itemValues)
    {
      int index = 0;
      foreach (var item in GetCells())
      {
        item.ItemClass = (ItemClass)itemValues[index];
        item.FontSize = ItemFontSize;
        item.Configure();
        index++;

        AddItem(item);
      }
    }

    public virtual void AddItem(Item item)
    {
      item.transform.SetParent(GridLayout.transform);
      item.SetCorrectlyClassified(item.ItemClass == ItemClass);
      item.Focused += Item_Focused;
    }

    public virtual void RemoveItem(Item item)
    {
      item.Focused -= Item_Focused;
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
