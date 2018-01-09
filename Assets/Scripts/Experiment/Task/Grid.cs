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
  public class Grid : Grid<Grid, Container>, IDraggable, IZoomable
  {
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

    public Transform Transform { get { return transform; } }

    public GenericVector3<bool> FreezePosition { get; protected set; }
    public GenericVector3<Range<float>> PositionRange { get; protected set; }

    public GenericVector3<bool> FreezeScale { get; protected set; }
    public GenericVector3<Range<float>> ScaleRange { get; protected set; }

    // Properties

    public bool IsConfigured { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public int RemainingItemsToClassify { get; protected set; }

    public Vector3 LossyScale { get{ return Vector3.Scale(Scale, transform.lossyScale); } }

    // Interfaces events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IDraggable> DraggingStarted = delegate { };
    public event Action<IDraggable> Dragging = delegate { };
    public event Action<IDraggable> DraggingStopped = delegate { };

    public event Action<IZoomable> ZoomingStarted = delegate { };
    public event Action<IZoomable> Zooming = delegate { };
    public event Action<IZoomable> ZoomingStopped = delegate { };

    // Events

    public event Action<GridGenerator> ConfigureSync = delegate { };
    public event Action Configured = delegate { };

    public event Action CompleteSync = delegate { };
    public event Action Completed = delegate { };

    public event Action<Container, Item> ItemSelected = delegate { };
    public event Action<Container, Item> ItemDeselected = delegate { };
    public event Action<Container, Container, Item> ClassificationSuccess = delegate { };
    public event Action<Container, Container, Item> ClassificationError = delegate { };

    // Variables

    protected new BoxCollider collider;

    protected Item selectedItem;

    protected IVTextSize ivTextSize;
    protected IVClassificationDifficulty iVClassificationDifficulty;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      collider = GetComponent<BoxCollider>();

      FreezePosition = FreezeScale = new GenericVector3<bool>(false, false, true);
      PositionRange = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), null);
      ScaleRange = new GenericVector3<Range<float>>(new Range<float>(), new Range<float>(), null);

      SetInteractable(false);
      StartCoroutine(SetContainersItemsInteractable(false));

      IsConfigured = false;
      IsCompleted = false;
    }

    protected virtual void OnDestroy()
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

    // Interfaces methods

    public override Grid Instantiate()
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
      IsDragging = value;
      if (IsDragging)
      {
        DraggingStarted(this);
      }
      else
      {
        DraggingStopped(this);
      }

      StartCoroutine(SetContainersItemsInteractable(!IsDragging && !IsZooming));
    }

    public void Drag(Vector3 translation)
    {
      Dragging(this);
    }

    public void SetZooming(bool value)
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

      StartCoroutine(SetContainersItemsInteractable(!IsDragging && !IsZooming));
    }

    public void Zoom(float scaleFactor, Vector3 translation)
    {
      UpdateTransformRanges();

      // Fix resize bug of the colliders of the items
      if (translation.sqrMagnitude > 0)
      {
        foreach (var container in Elements)
        {
          foreach (var item in container.Elements)
          {
            item.Collider.radius *= scaleFactor;
          }
        }
      }

      Zooming(this);
    }

    // Methods

    public override void Configure()
    {
      if (iVClassificationDifficulty == null)
      {
        iVClassificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();
      }
      var classificationCondition = iVClassificationDifficulty.CurrentCondition;

      // Pre-configure the grid
      CleanGrid();
      base.Configure();

      // Generate a grid generator with average distance in current condition classification distance range
      int gridNumber = 0;
      GridGenerator gridGenerator;
      do
      {
        int itemsPerContainer = Elements[0].ElementsInstantiatedAtConfigure;
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, itemsPerContainer,
          classificationCondition.NumberOfItemsToClass,
          (GridGenerator.DistanceTypes)iVClassificationDifficulty.CurrentConditionIndex);
        gridNumber++;
      }
      while (!classificationCondition.AverageClassificationDistanceRange.ContainsValue(gridGenerator.AverageDistance)
        && classificationCondition.NumberOfItemsToClass != gridGenerator.IncorrectContainersNumber
        && gridNumber < gridGenerationMaxNumber);

      if (gridNumber >= gridGenerationMaxNumber)
      {
        throw new Exception("Failed to generate a grid.");
      }
      RemainingItemsToClassify = gridGenerator.IncorrectContainersNumber;

      ConfigureSync(gridGenerator);
    }

    public virtual void Show(bool value)
    {
      foreach (var container in Elements)
      {
        container.GameObject.SetActive(value);
      }
      background.SetActive(value);
    }

    internal virtual void SetConfiguration(GridGenerator gridGenerator)
    {
      // Init the variables and properties
      if (ivTextSize == null)
      {
        ivTextSize = stateController.GetIndependentVariable<IVTextSize>();
      }

      SetInteractable(false);
      StartCoroutine(SetContainersItemsInteractable(false));

      IsConfigured = false;
      IsCompleted = false;

      transform.localPosition = Vector3.zero;
      transform.localScale = scaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      // Configure the grid
      CleanGrid();
      base.Configure();
      BuildGrid();

      // Configure containers and items
      int containerRow = 0, containerColumn = 0;
      foreach (var container in Elements)
      {
        container.Configure();
        container.BuildGrid();

        container.ItemClass = (ItemClass)gridGenerator.Containers[containerRow, containerColumn].GetMainItemId();
        container.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
        container.ConfigureItems(gridGenerator.Containers[containerRow, containerColumn].items);

        container.Selected2 += Container_Selected;
        foreach (var item in container.Elements)
        {
          item.Selected2 += Item_Selected;
        }

        containerColumn = (containerColumn + 1) % GridSize.x;
        if (containerColumn == 0)
        {
          containerRow = (containerRow + 1) % GridSize.y;
        }
      }

      // Finish the grid configuration
      float itemSize = Elements[0].ElementScale.x; // items have same size in x and in y
      collider.center = new Vector3(0f, 0f, itemSize);
      collider.size = new Vector3(Scale.x, Scale.y, 3f * itemSize + itemSize/2f);

      background.transform.localScale = new Vector3(Scale.x, Scale.y, 1);

      UpdateTransformRanges();

      // Update grid state
      SetInteractable(true);
      StartCoroutine(SetContainersItemsInteractable(true));

      IsConfigured = true;
      Configured();
    }

    internal virtual void SetCompleted()
    {
      StartCoroutine(SetContainersItemsInteractable(false));
      SetInteractable(false);

      IsCompleted = true;
      Completed();
    }

    protected virtual void Container_Selected(Container container)
    {
      if (selectedItem != null)
      {
        Container previousContainer = GetContainer(selectedItem);
        if (previousContainer == container)
        {
          // Deselect the item if it's the same container
          ItemDeselected(previousContainer, selectedItem);
        }
        else
        {
          // Update RemainingItemsToClassify and classification events
          if (container.IsFull || container.ItemClass != selectedItem.ItemClass)
          {
            if (previousContainer.ItemClass == selectedItem.ItemClass)
            {
              RemainingItemsToClassify++;
            }
            ClassificationError(previousContainer, container, selectedItem);
          }
          else
          {
            RemainingItemsToClassify--;
            ClassificationSuccess(previousContainer, container, selectedItem);
          }

          // Move the selected item only if it's a different and not full container
          if (!container.IsFull)
          {
            previousContainer.RemoveElement(selectedItem);
            container.AddElement(selectedItem);
          }

          // Deselect the item
          selectedItem.SetSelected(false);
          selectedItem = null;

          // Call Finished if all items are classified
          if (RemainingItemsToClassify == 0)
          {
            CompleteSync();
          }
        }
      }
    }

    protected virtual void Item_Selected(Item item)
    {
      Container container;

      // Deselect the previous selected item
      if (selectedItem != null)
      {
        if (selectedItem != item)
        {
          selectedItem.SetSelected(false);
        }

        container = GetContainer(selectedItem);
        ItemDeselected(container, selectedItem);

        selectedItem = null;
      }

      // Update selectedItem with the new item
      if (item.IsSelected)
      {
        selectedItem = item;

        container = GetContainer(selectedItem);
        ItemSelected(container, selectedItem);
      }
    }

    protected virtual IEnumerator SetContainersItemsInteractable(bool value)
    {
      if (value == true)
      {
        yield return null; // wait a frame before reactivacting the containers and items
      }

      foreach (var container in Elements)
      {
        container.SetInteractable(value);
        foreach (var item in container.Elements)
        {
          item.SetInteractable(value);

          if (value == false)
          {
            item.SetFocused(value);
          }
        }
      }
    }

    protected virtual Container GetContainer(Item item)
    {
      Container parentContainer = null;
      foreach (var container in Elements)
      {
        if (container.Elements.Contains(selectedItem))
        {
          parentContainer = container;
        }
      }
      return parentContainer;
    }

    protected virtual void UpdateTransformRanges()
    {
      var itemSize = Elements[0].ElementScale.x;

      ScaleRange[0].Minimum = ScaleRange[1].Minimum = scaleFactor * Mathf.Max(ElementScale.x / Scale.x, ElementScale.y / Scale.y);
      ScaleRange[0].Maximum = ScaleRange[1].Maximum = scaleFactor * Mathf.Min(ElementScale.x, ElementScale.y) / itemSize;

      Vector2 positionMax = (Vector2.Scale(transform.localScale, Scale) - scaleFactor * ElementScale) / 2;
      PositionRange[0].Minimum = -positionMax.x;
      PositionRange[0].Maximum = positionMax.x;
      PositionRange[1].Minimum = -positionMax.y;
      PositionRange[1].Maximum = positionMax.y;
    }
  }
}
