using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Grid : Grid<Grid, Container>, IDraggable, IZoomable
  {
    // Editor fields

    [Header("Task grid")]
    [SerializeField]
    private float scaleFactor = 0.0001f;

    [SerializeField]
    private StateController stateController;

    [SerializeField]
    private GameObject background;

    // Interfaces properties

    public bool IsInteractable { get; protected set; }

    public bool IsDragging { get; protected set; }
    public float DistanceToStartDragging { get; protected set; }

    public bool IsZooming { get; protected set; }

    // Interfaces events

    public event Action<IInteractable> Interactable = delegate { };

    public event Action<IDraggable> DraggingStarted = delegate { };
    public event Action<IDraggable> Dragging = delegate { };
    public event Action<IDraggable> DraggingStopped = delegate { };

    public event Action<IZoomable> ZoomingStarted = delegate { };
    public event Action<IZoomable> Zooming = delegate { };
    public event Action<IZoomable> ZoomingStopped = delegate { };

    // Variables

    protected new BoxCollider collider;
    protected List<HoverCursorController> triggeredFingers = new List<HoverCursorController>();
    protected Vector3 fingerPanningLastPosition;

    protected Item selectedItem;

    protected IVTextSize ivTextSize;
    protected IVClassificationDifficulty iVClassificationDifficulty;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();
      collider = GetComponent<BoxCollider>();
      SetInteractable(true);
    }

    /// <summary>
    /// Gets and subscribes to the independent variables, and calls <see cref="ConfigureGrid"/>.
    /// </summary>
    protected virtual void Start()
    {
      ivTextSize = stateController.GetIndependentVariable<IVTextSize>();
      iVClassificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();

      foreach (var independentVariable in stateController.independentVariables)
      {
        independentVariable.CurrentConditionUpdated += IIndependentVariable_CurrentConditionUpdated;
      }

      transform.localScale = scaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      Configure(); // TODO: remove, only call when state is training or trial
    }

    protected virtual void OnDestroy()
    {
      foreach (var independentVariable in stateController.independentVariables)
      {
        independentVariable.CurrentConditionUpdated -= IIndependentVariable_CurrentConditionUpdated;
      }

      foreach (var container in Elements)
      {
        container.Selected2 -= Container_Selected;
        foreach (var item in container.Elements)
        {
          item.SelectedItem -= Item_Selected;
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
      if (IsInteractable)
      {
        Interactable(this);
      }
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

    public override void Configure()
    {
      // Configure the grid
      CleanGrid();
      base.Configure();
      BuildGrid();
      
      // Generate a grid generator with average distance in current condition classification distance range
      GridGenerator gridGenerator;
      do
      {
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, Elements[0].ElementsInstantiatedAtConfigure,
        iVClassificationDifficulty.CurrentCondition.IncorrectlyClassifiedContainersFraction,
        (GridGenerator.DistanceTypes)iVClassificationDifficulty.CurrentConditionIndex);
      }
      while (!iVClassificationDifficulty.CurrentCondition.Range.ContainsValue(gridGenerator.AverageDistance));

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
          item.SelectedItem += Item_Selected;
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

      DistanceToStartDragging = 0.5f * scaleFactor * itemSize; // Activate panning only if the finger has moved more than half the size of an item
    }

    protected virtual void Container_Selected(Container container)
    {
      if (selectedItem != null)
      {
        foreach (var previouContainer in Elements)
        {
          if (previouContainer.Elements.Contains(selectedItem))
          {
            previouContainer.RemoveElement(selectedItem);
          }
        }

        if (!container.IsFull)
        {
          container.AddElement(selectedItem);
        }

        selectedItem.SetSelected(false);
        selectedItem = null;
      }
    }

    protected virtual void Item_Selected(Item item)
    {
      if (selectedItem != null)
      {
        selectedItem.SetSelected(false);
      }
      selectedItem = item;
    }

    protected virtual void IIndependentVariable_CurrentConditionUpdated()
    {
      base.Configure(); // TODO: only call when state is training or trial
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
  }
}
