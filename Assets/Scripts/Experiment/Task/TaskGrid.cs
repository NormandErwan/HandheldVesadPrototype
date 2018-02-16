using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class TaskGrid : Grid<TaskGrid, Container>, IFocusable, IDraggable, IZoomable
  {
    // Enums

    public enum ItemMovedType
    {
      Classified,
      Error
    }

    public enum InteractionMode
    {
      Select = 1,
      Pan = 2,
      Zoom = 4
    }

    // Constants

    protected int gridGenerationMaxNumber = 10000;

    // Editor fields

    [SerializeField]
    private int interactablePriority;

    [Header("Task grid")]
    [SerializeField]
    private float scaleFactor = 0.0001f;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private StateController stateController;

    // Interfaces properties

    public int Priority { get { return interactablePriority; } }
    public IInteractable Parent { get { return null; } }

    public bool IsInteractable { get; protected set; }
    public bool IsTransformable { get; protected set; }

    public bool IsFocusable { get; protected set; }
    public bool IsFocused { get; protected set; }

    public bool IsTransforming { get { return IsDragging || IsZooming; } }
    public bool IsDragging { get; protected set; }
    public bool IsZooming { get; protected set; }

    public bool DragToZoom { get; protected set; }

    public Transform Transform { get { return transform; } }
    public GenericVector3<Range<float>> PositionRange { get; protected set; }
    public GenericVector3<Range<float>> ScaleRange { get; protected set; }

    // Properties

    public InteractionMode Mode { get; protected set; }

    public bool IsConfigured { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public int RemainingItemsToClassify { get; protected set; }

    public GridGenerator GridGenerator { get; protected set; }

    // Interfaces events

    public event Action<IInteractable> Interactable = delegate { };
    public event Action<ITransformable> Transformable = delegate { };

    public event Action<IFocusable> Focused = delegate { };

    public event Action<IDraggable> DraggingStarted = delegate { };
    public event Action<IDraggable, Vector3> Dragging = delegate { };
    public event Action<IDraggable> DraggingStopped = delegate { };

    public event Action<IZoomable> ZoomingStarted = delegate { };
    public event Action<IZoomable, Vector3, Vector3> Zooming = delegate { };
    public event Action<IZoomable> ZoomingStopped = delegate { };

    // Events

    public event Action<GridGenerator> ConfigureSync = delegate { };
    public event Action Configured = delegate { };

    public event Action CompleteSync = delegate { };
    public event Action Completed = delegate { };

    public event Action<Item> ItemSelectSync = delegate { };
    public event Action<Container, Item> ItemSelected = delegate { };

    public event Action<Container> ItemMoveSync = delegate { };
    public event Action<Container, Container, Item, ItemMovedType> ItemMoved = delegate { };

    public event Action<bool> SetDraggingSync = delegate { };
    public event Action<Vector3> DragSync = delegate { };

    public event Action<bool> SetZoomingSync = delegate { };
    public event Action<Vector3, Vector3> ZoomSync = delegate { };

    // Variables

    protected new BoxCollider collider;

    protected Container focusedContainer;
    protected Item focusedItem;

    protected Item selectedItem;
    protected bool ignoreNextItemSelected = false;

    protected IVTechnique technique;
    protected IVTextSize textSize;
    protected IVClassificationDifficulty classificationDifficulty;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      collider = GetComponent<BoxCollider>();
      PositionRange = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), new Range<float>());
      ScaleRange = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), new Range<float>());

      DragToZoom = false;
      IsConfigured = false;
      IsCompleted = false;
    }

    protected virtual void Start()
    {
      technique = stateController.GetIndependentVariable<IVTechnique>();
      textSize = stateController.GetIndependentVariable<IVTextSize>();
      classificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();

      SetInteractable(false);
      SetTransformable(false);
      SetElementsInteractables(false);
    }
    
    protected virtual void OnDestroy()
    {
      UnsubscribeFromElementEvents();
    }

    // Interfaces methods

    public override TaskGrid Instantiate()
    {
      return Instantiate(this);
    }

    public void SetInteractable(bool value)
    {
      IsInteractable = value;
      Interactable(this);
    }

    public void SetTransformable(bool value)
    {
      IsTransformable = value;
      Transformable(this);
    }

    public void SetFocused(bool value)
    {
      SetElementsInteractables(true);

      IsFocused = value;
      Focused(this);
    }

    public void SetDragging(bool value)
    {
      SetDraggingSync(value);
    }

    public void Drag(Vector3 translation)
    {
      DragSync(translation);
    }

    public void SetDragged(bool value)
    {
      if (IsDragging != value)
      {
        IsDragging = value;
        if (IsDragging)
        {
          DraggingStarted(this);
        }
        else
        {
          DraggingStopped(this);
        }
      }
      SetElementsInteractables(!IsDragging);
    }

    public void SetDragged(Vector3 translation)
    {
      transform.localPosition += translation;
      Dragging(this, translation);
    }

    public void SetZooming(bool value)
    {
      SetZoomingSync(value);
    }

    public void Zoom(Vector3 scaling, Vector3 translation)
    {
      ZoomSync(scaling, translation);
    }

    public void SetZoomed(bool value)
    {
      if (IsZooming != value)
      {
        IsZooming = value;
        if (IsZooming)
        {
          ZoomingStarted(this);
        }
        else
        {
          ZoomingStopped(this);
        }
      }
      SetElementsInteractables(!IsZooming);
    }

    public void SetZoomed(Vector3 scaling, Vector3 translation)
    {
      transform.localPosition += translation;
      transform.localScale = Vector3.Scale(scaling, transform.localScale);

      UpdateTransformRanges();

      // Fix resize bug of the colliders of the items
      foreach (var container in Elements)
      {
        foreach (var item in container.Elements)
        {
          if (transform.localScale.x < scaleFactor)
          {
            item.Collider.radius *= scaling.x;
          }
          else
          {
            item.Collider.radius = 0.5f * item.Scale.x;
          }
        }
      }

      Zooming(this, scaling, translation);
    }

    // Methods

    public override void Configure()
    {
      var classificationCondition = classificationDifficulty.CurrentCondition;

      // Pre-configure the grid
      Clean();
      base.Configure();

      // Generate a grid generator with average distance in current condition classification distance range
      int gridNumber = 0;
      GridGenerator gridGenerator;
      do
      {
        int itemsPerContainer = Elements[0].ElementsInstantiatedAtConfigure;
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, itemsPerContainer,
          classificationCondition.NumberOfItemsToClass,
          (GridGenerator.DistanceTypes)classificationDifficulty.CurrentConditionIndex);
        gridNumber++;
      }
      while (!classificationCondition.AverageClassificationDistanceRange.ContainsValue(gridGenerator.AverageDistance)
        && classificationCondition.NumberOfItemsToClass != gridGenerator.IncorrectContainersNumber
        && gridNumber < gridGenerationMaxNumber);

      if (gridNumber >= gridGenerationMaxNumber)
      {
        throw new Exception("Failed to generate a grid.");
      }

      ConfigureSync(gridGenerator);
    }

    public void Complete()
    {
      CompleteSync();
    }

    public void Show(bool value)
    {
      foreach (var container in Elements)
      {
        container.GameObject.SetActive(value);
      }
      background.SetActive(value);
    }

    public void SetMode(InteractionMode mode)
    {
      Mode = mode;
      DragToZoom = HasMode(InteractionMode.Zoom) && !HasMode(InteractionMode.Pan);
      SetTransformable(HasMode(InteractionMode.Zoom) || HasMode(InteractionMode.Pan));
      SetElementsInteractables(HasMode(InteractionMode.Select));
    }

    public Container GetContainer(Item item)
    {
      Container parentContainer = null;
      foreach (var container in Elements)
      {
        if (container.Elements.Contains(item))
        {
          parentContainer = container;
          break;
        }
      }
      return parentContainer;
    }

    internal void SetConfiguration(GridGenerator gridGenerator)
    {
      // Init the variables and properties
      GridGenerator = gridGenerator;
      GridSize = new Vector2Int(GridGenerator.ColumnsNumber, GridGenerator.RowsNumber);
      RemainingItemsToClassify = gridGenerator.IncorrectContainersNumber;

      SetInteractable(false);
      SetElementsInteractables(false);

      IsConfigured = false;
      IsCompleted = false;

      transform.localPosition = Vector3.zero;
      transform.localScale = scaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      // Configure the grid
      UnsubscribeFromElementEvents();
      Clean();
      base.Configure();
      Display();

      // Configure containers and items
      Vector2Int position = Vector2Int.zero;
      foreach (var container in Elements)
      {
        container.Configure();
        container.Display();

        container.Parent = this;
        container.ItemClass = (ItemClass)GridGenerator.Containers[position.y, position.x].GetMainItemId();
        container.ItemFontSize = textSize.CurrentCondition.fontSize;
        container.ConfigureItems(GridGenerator.Containers[position.y, position.x].items);

        container.Focused2 += Container_Focused;
        container.Selected2 += Container_Selected;
        foreach (var item in container.Elements)
        {
          item.Focused2 += Item_Focused;
          item.Selected2 += Item_Selected;
        }

        position = GetNextPosition(position);
      }

      // Finish the grid configuration
      float itemSize = Elements[0].ElementScale.x; // items have same size in x and in y
      collider.center = new Vector3(0f, 0f, itemSize);
      collider.size = new Vector3(Scale.x, Scale.y, 3f * itemSize + itemSize/2f);

      background.transform.localScale = new Vector3(Scale.x, Scale.y, 1);

      UpdateTransformRanges();

      // Update grid state
      SetInteractable(true);
      SetElementsInteractables(true);

      IsConfigured = true;
      Configured();
    }

    internal void SetCompleted()
    {
      SetInteractable(false);
      SetElementsInteractables(false);

      IsCompleted = true;
      Completed();
    }

    internal void SetItemSelected(Item item, Container container)
    {
      // Deselect the previous selected item
      var previousSelectedItem = selectedItem;
      if (selectedItem != null)
      {
        selectedItem = null;
        ignoreNextItemSelected = true;
        previousSelectedItem.SetSelected(false);
        ItemSelected(container, previousSelectedItem);
      }

      // Update selectedItem with the new item
      if (item != previousSelectedItem)
      {
        selectedItem = item;
        if (!item.IsSelected)
        {
          ignoreNextItemSelected = true;
          item.SetSelected(true);
        }
        ItemSelected(container, selectedItem);
      }
    }

    internal void SetItemMoved(Container newContainer)
    {
      // Move the selected item only if it's the container is not full
      var previousContainer = GetContainer(selectedItem);
      if (!newContainer.IsFull)
      {
        previousContainer.Remove(selectedItem);
        newContainer.Append(selectedItem);

        ItemMovedType type;
        if (previousContainer.ItemClass != selectedItem.ItemClass && newContainer.ItemClass == selectedItem.ItemClass)
        {
          type = ItemMovedType.Classified;
        }
        else
        {
          type = ItemMovedType.Error;
        }
        ItemMoved(previousContainer, newContainer, selectedItem, type);
        SetItemSelected(selectedItem, newContainer);
      }
      else
      {
        ItemMoved(previousContainer, previousContainer, selectedItem, ItemMovedType.Error);
        SetItemSelected(selectedItem, previousContainer);
      }
    }

    protected void UnsubscribeFromElementEvents()
    {
      if (IsConfigured)
      {
        foreach (var container in Elements)
        {
          container.Selected2 -= Container_Selected;
          foreach (var item in container.Elements)
          {
            item.Selected2 -= Item_Selected;
          }
        }
      }
    }

    protected void Item_Focused(Item item)
    {
      if (focusedItem != item)
      {
        if (focusedItem != null)
        {
          focusedItem.SetFocused(false);
        }
        if (focusedContainer != null)
        {
          focusedContainer.SetFocused(false);
        }

        focusedItem = item;
        ((ISelectable)item.Parent).SetSelectable(false); // Avoid the parent container of the item to be selected before the item
      }
      else if (!item.IsFocused)
      {
        focusedItem = null;
        ((ISelectable)item.Parent).SetSelectable(true);
      }
    }

    protected void Item_Selected(Item item)
    {
      if (ignoreNextItemSelected)
      {
        ignoreNextItemSelected = false;
        return;
      }
      ItemSelectSync(item);
    }

    protected void Container_Focused(Container container)
    {
      if (focusedItem != null)
      {
        if (focusedContainer == container)
        {
          focusedContainer.SetFocused(false);
          focusedContainer = null;
        }
        else if (container.IsFocused)
        {
          container.SetFocused(false);
        }
      }
      else if (focusedContainer != container)
      {
        if (focusedContainer != null)
        {
          focusedContainer.SetFocused(false);
        }
        focusedContainer = container;
      }
      else if (!container.IsFocused)
      {
        focusedContainer = null;
      }
    }

    protected void Container_Selected(Container container)
    {
      if (container.IsSelected && selectedItem != null)
      {
        Container previousContainer = GetContainer(selectedItem);
        if (previousContainer != container) // Move the item only if it's a different container
        {
          // Update RemainingItemsToClassify and classification events
          if (!container.IsFull)
          {
            if (previousContainer.ItemClass != selectedItem.ItemClass && container.ItemClass == selectedItem.ItemClass)
            {
              RemainingItemsToClassify--;
            }
            else if (previousContainer.ItemClass == selectedItem.ItemClass && container.ItemClass != selectedItem.ItemClass)
            {
              RemainingItemsToClassify++;
            }
          }

          // Sync the item move
          ItemMoveSync(container);

          // Call Complete if all items are classified
          if (RemainingItemsToClassify == 0)
          {
            Complete();
          }
        }
        else
        {
          selectedItem.SetSelected(false);
        }
      }
    }

    protected void SetElementsInteractables(bool value)
    {
      value = value && HasMode(InteractionMode.Select);

      foreach (var container in Elements)
      {
        container.SetInteractable(value);
        container.SetLongPressable(technique.CurrentCondition.useLeapInput);
        container.SetTappable(technique.CurrentCondition.useTouchInput);

        foreach (var item in container.Elements)
        {
          item.SetInteractable(value);
          item.SetLongPressable(technique.CurrentCondition.useLeapInput);
          item.SetTappable(technique.CurrentCondition.useTouchInput);
        }
      }

      if (value == false)
      {
        DefocusElements();
      }
    }

    protected void DefocusElements()
    {
      foreach (var container in Elements)
      {
        container.SetFocused(false);
        foreach (var item in container.Elements)
        {
          item.SetFocused(false);
        }
      }
    }

    protected void UpdateTransformRanges()
    {
      var itemSize = Elements[0].ElementScale.x;
      ScaleRange.X.Minimum = ScaleRange.Y.Minimum = scaleFactor * Mathf.Max(ElementScale.x / Scale.x, ElementScale.y / Scale.y);
      ScaleRange.X.Maximum = ScaleRange.Y.Maximum = scaleFactor * Mathf.Min(ElementScale.x, ElementScale.y) / itemSize;
      ScaleRange.Z.Minimum = ScaleRange.Z.Maximum = transform.localScale.z;

      // Limit the grid position to the borders of the central container
      var bla = Vector3.Scale(ElementScale, transform.lossyScale);
      var positionMax = (LossyScale - bla) / 2;
      PositionRange.X.Minimum = -positionMax.x;
      PositionRange.X.Maximum = positionMax.x;
      PositionRange.Y.Minimum = -positionMax.y;
      PositionRange.Y.Maximum = positionMax.y;
      PositionRange.Z.Minimum = PositionRange.Z.Maximum = transform.localPosition.z;
    }

    protected bool HasMode(InteractionMode mode)
    {
      return (Mode & mode) == mode;
    }
  }
}
