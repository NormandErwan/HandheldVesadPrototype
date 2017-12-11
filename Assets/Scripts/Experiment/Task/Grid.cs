using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using System.Collections.Generic;
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

    // Interfaces properties

    public bool IsInteractable { get; protected set; }
    public bool IsDragging { get; protected set; }
    public bool IsZooming { get; protected set; }

    // Properties

    public int RemainingItemsToClassify = 0;

    // Interfaces events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IDraggable> DraggingStarted = delegate { };
    public event Action<IDraggable> Dragging = delegate { };
    public event Action<IDraggable> DraggingStopped = delegate { };

    public event Action<IZoomable> ZoomingStarted = delegate { };
    public event Action<IZoomable> Zooming = delegate { };
    public event Action<IZoomable> ZoomingStopped = delegate { };

    // Events

    public event Action<Container, Item> ItemSelected = delegate { };
    public event Action<Container, Item> ItemDeselected = delegate { };
    public event Action<Container, Container, Item> ClassificationSuccess = delegate { };
    public event Action<Container, Container, Item> ClassificationError = delegate { };
    public event Action Finished = delegate { };

    // Variables

    protected new BoxCollider collider;
    protected List<Inputs.Cursor> triggeredFingers = new List<Inputs.Cursor>();
    protected Vector3 fingerPanningLastPosition;

    protected Item selectedItem;

    protected IVTextSize ivTextSize;
    protected IVClassificationDifficulty iVClassificationDifficulty;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      collider = GetComponent<BoxCollider>();

      transform.localScale = scaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      SetInteractable(true);
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

      SetContainersInteractable(!IsDragging && !IsZooming);
    }

    public void Drag(Vector3 translation)
    {
      transform.position += Vector3.ProjectOnPlane(translation, -transform.forward);
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

      SetContainersInteractable(!IsDragging && !IsZooming);
    }

    public void Zoom(Vector3 distance, Vector3 previousDistance, Vector3 translation, Vector3 previousTranslation)
    {
      var distanceProjected = Vector3.ProjectOnPlane(distance, -transform.forward);
      var previousDistanceProjected = Vector3.ProjectOnPlane(previousDistance, -transform.forward);
      var scaleFactor = distanceProjected.magnitude / previousDistanceProjected.magnitude;
      transform.localScale = new Vector3(transform.localScale.x * scaleFactor, transform.localScale.y * scaleFactor, transform.localScale.z);

      var translationProjected = scaleFactor * Vector3.ProjectOnPlane(translation, -transform.forward);
      var previousTranslationProjected = Vector3.ProjectOnPlane(previousTranslation, -transform.forward);
      transform.position += translationProjected - previousTranslationProjected;
    }

    // Methods

    public virtual void Configure(StateController stateController)
    {
      ivTextSize = stateController.GetIndependentVariable<IVTextSize>();
      iVClassificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();

      Configure();
    }

    public override void Configure()
    {
      // Configure the grid
      CleanGrid();
      base.Configure();
      BuildGrid();

      // Generate a grid generator with average distance in current condition classification distance range
      var classificationCondition = iVClassificationDifficulty.CurrentCondition;

      int gridNumber = 0;
      GridGenerator gridGenerator;
      do
      {
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, Elements[0].ElementsInstantiatedAtConfigure,
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
      float itemSize = Elements[0].ElementScale.x;
      collider.center = new Vector3(0f, 0f, itemSize);
      collider.size = new Vector3(Scale.x, Scale.y, 3f * itemSize);

      background.transform.localScale = new Vector3(Scale.x, Scale.y, 1);
    }

    protected virtual void Container_Selected(Container container)
    {
      if (selectedItem != null)
      {
        Container previousContainer = GetItemContainer(selectedItem);
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
            Finished();
          }
        }
      }
    }

    protected virtual void Item_Selected(Item item)
    {
      Container itemContainer;

      if (selectedItem != null)
      {
        // Deselect the previous selected item
        if (selectedItem != item)
        {
          selectedItem.SetSelected(false);
        }

        // Call ItemDeselected
        itemContainer = GetItemContainer(selectedItem);
        ItemDeselected(itemContainer, selectedItem);

        selectedItem = null;
      }

      // Update selectedItem with the new item
      if (item.IsSelected)
      {
        selectedItem = item;

        itemContainer = GetItemContainer(selectedItem);
        ItemSelected(itemContainer, selectedItem);
      }
    }

    protected virtual void SetContainersInteractable(bool value)
    {
      foreach (var container in Elements)
      {
        container.SetInteractable(value);
        foreach (var item in container.Elements)
        {
          item.SetInteractable(value);
        }
      }
    }

    protected virtual Container GetItemContainer(Item item)
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
  }
}
