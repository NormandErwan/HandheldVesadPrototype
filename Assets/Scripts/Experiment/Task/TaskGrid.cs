using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class TaskGrid : Grid<TaskGrid, Container>, IDraggable, IZoomable
  {
    // Enums

    public enum ItemMovedType
    {
      Classified,
      Error
    }

    // Constants

    protected int gridGenerationMaxNumber = 1000;

    // Editor fields

    [Header("Task grid")]
    [SerializeField]
    private float scaleFactor = 0.0001f;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private StateController stateController;

    // Interfaces properties

    public bool IsInteractable { get; protected set; }
    public bool IsDragging { get; protected set; }
    public bool IsZooming { get; protected set; }

    public bool DragToZoom { get; set; }
    public Transform Transform { get { return transform; } }
    public GenericVector3<Range<float>> PositionRange { get; protected set; }
    public GenericVector3<Range<float>> ScaleRange { get; protected set; }

    // Properties

    public bool IsConfigured { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public int RemainingItemsToClassify { get; protected set; }

    public Vector3 LossyScale { get { return Vector3.Scale(Scale, transform.lossyScale); } }

    public GridGenerator GridGenerator { get; protected set; }

    // Interfaces events

    public event Action<IInteractable> Interactable = delegate { };

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

    protected Item selectedItem;
    protected bool itemSelectedThisFrame = false;

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

      SetInteractable(false);

      DragToZoom = false;

      IsConfigured = false;
      IsCompleted = false;
    }

    protected virtual void Start()
    {
      technique = stateController.GetIndependentVariable<IVTechnique>();
      textSize = stateController.GetIndependentVariable<IVTextSize>();
      classificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();

      StartCoroutine(SetElementsInteractables(false));
    }

    protected virtual void LateUpdate()
    {
      itemSelectedThisFrame = false;
    }

    protected virtual void OnDestroy()
    {
      UnsubscribeFromElements();
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

    public Vector3 ProjectPosition(Vector3 position)
    {
      return Vector3.ProjectOnPlane(position, -transform.forward);
    }

    public void SetDragging(bool value)
    {
      SetDragged(value);
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
      StartCoroutine(SetElementsInteractables(!IsDragging && !IsZooming));
    }

    public void SetDragged(Vector3 translation)
    {
      transform.localPosition += translation;
      Dragging(this, translation);
    }

    public void SetZooming(bool value)
    {
      SetZoomed(value);
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
      StartCoroutine(SetElementsInteractables(!IsDragging && !IsZooming));
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

    public virtual void Complete()
    {
      CompleteSync();
    }

    public virtual void Show(bool value)
    {
      foreach (var container in Elements)
      {
        container.GameObject.SetActive(value);
      }
      background.SetActive(value);
    }

    public virtual Container GetContainer(Item item)
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

    internal virtual void SetConfiguration(GridGenerator gridGenerator)
    {
      // Init the variables and properties
      GridGenerator = gridGenerator;
      GridSize = new Vector2Int(GridGenerator.ColumnsNumber, GridGenerator.RowsNumber);
      RemainingItemsToClassify = gridGenerator.IncorrectContainersNumber;

      SetInteractable(false);
      StartCoroutine(SetElementsInteractables(false));

      IsConfigured = false;
      IsCompleted = false;

      transform.localPosition = Vector3.zero;
      transform.localScale = scaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      // Configure the grid
      UnsubscribeFromElements();
      Clean();
      base.Configure();
      Display();

      // Configure containers and items
      Vector2Int position = Vector2Int.zero;
      foreach (var container in Elements)
      {
        container.Configure();
        container.Display();

        container.ItemClass = (ItemClass)GridGenerator.Containers[position.y, position.x].GetMainItemId();
        container.ItemFontSize = textSize.CurrentCondition.fontSize;
        container.ConfigureItems(GridGenerator.Containers[position.y, position.x].items);

        container.Selected2 += Container_Selected;
        foreach (var item in container.Elements)
        {
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
      StartCoroutine(SetElementsInteractables(true));

      IsConfigured = true;
      Configured();
    }

    internal virtual void SetCompleted()
    {
      StartCoroutine(SetElementsInteractables(false));
      SetInteractable(false);

      IsCompleted = true;
      Completed();
    }

    internal virtual void SetItemSelected(Item item, Container container)
    {
      itemSelectedThisFrame = true;

      // Deselect the previous selected item
      var oldSelectedItem = selectedItem;
      if (selectedItem != null)
      {
        selectedItem = null;
        oldSelectedItem.SetSelected(false);
        ItemSelected(container, oldSelectedItem);
      }

      // Update selectedItem with the new item
      if (oldSelectedItem != item)
      {
        if (!item.IsSelected)
        {
          item.SetSelected(true);
        }
        selectedItem = item;
        ItemSelected(container, selectedItem);
      }
    }

    internal virtual void SetItemMoved(Container newContainer)
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

    protected virtual void UnsubscribeFromElements()
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

    protected virtual void Item_Selected(Item item)
    {
      if (!itemSelectedThisFrame)
      {
        itemSelectedThisFrame = true;
        ItemSelectSync(item);
      }
    }

    protected virtual void Container_Selected(Container newContainer)
    {
      if (selectedItem != null && !itemSelectedThisFrame) // NOTE: the item will be always selected before the container (see colliders)
      {
        Container previousContainer = GetContainer(selectedItem);
        if (previousContainer != newContainer) // Move the item only if it's a different container
        {
          // Update RemainingItemsToClassify and classification events
          if (!newContainer.IsFull)
          {
            if (previousContainer.ItemClass != selectedItem.ItemClass && newContainer.ItemClass == selectedItem.ItemClass)
            {
              RemainingItemsToClassify--;
            }
            else if (previousContainer.ItemClass == selectedItem.ItemClass && newContainer.ItemClass != selectedItem.ItemClass)
            {
              RemainingItemsToClassify++;
            }
          }

          // Sync the item move
          ItemMoveSync(newContainer);

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

    protected virtual IEnumerator SetElementsInteractables(bool value)
    {
      if (value == true)
      {
        yield return null; // wait a frame before reactivacting the containers and items
      }

      foreach (var container in Elements)
      {
        container.SetInteractable(value);
        container.SetLongPressable(technique.CurrentCondition.useLeapInput);
        container.SetTappable(technique.CurrentCondition.useTouchInput);
        if (value == false)
        {
          container.SetFocused(false); // Defocus
        }

        foreach (var item in container.Elements)
        {
          item.SetInteractable(value);
          item.SetLongPressable(technique.CurrentCondition.useLeapInput);
          item.SetTappable(technique.CurrentCondition.useTouchInput);
          if (value == false)
          {
            item.SetFocused(false); // Defocus
          }
        }
      }
    }

    protected virtual void UpdateTransformRanges()
    {
      var itemSize = Elements[0].ElementScale.x;
      ScaleRange.X.Minimum = ScaleRange.Y.Minimum = scaleFactor * Mathf.Max(ElementScale.x / Scale.x, ElementScale.y / Scale.y);
      ScaleRange.X.Maximum = ScaleRange.Y.Maximum = scaleFactor * Mathf.Min(ElementScale.x, ElementScale.y) / itemSize;
      ScaleRange.Z.Minimum = ScaleRange.Z.Maximum = transform.localScale.z;

      // Limit the grid position to the borders of the central container
      Vector2 positionMax = (Vector2.Scale(transform.localScale, Scale) - scaleFactor * ElementScale) / 2;
      PositionRange.X.Minimum = -positionMax.x;
      PositionRange.X.Maximum = positionMax.x;
      PositionRange.Y.Minimum = -positionMax.y;
      PositionRange.Y.Maximum = positionMax.y;
      PositionRange.Z.Minimum = PositionRange.Z.Maximum = transform.localPosition.z;
    }
  }
}
