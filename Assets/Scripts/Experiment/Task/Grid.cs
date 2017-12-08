using NormandErwan.MasterThesisExperiment.Experiment.States;
using NormandErwan.MasterThesisExperiment.Experiment.Variables;
using NormandErwan.MasterThesisExperiment.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Grid : GridLayoutController<Cell>, IDraggable, IZoomable
  {
    // Editor fields

    [SerializeField]
    private Vector2Int cellGridSize;

    [Header("Canvas")]
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private float canvasScaleFactor = 0.0001f;

    [Header("References")]
    [SerializeField]
    private StateController stateController;

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

      canvas.GetComponent<RectTransform>().localScale = canvasScaleFactor * Vector3.one; // Scales the canvas as it's in world reference

      ConfigureGrid(); // TODO: remove, only call when state is training or trial
    }

    protected virtual void OnDestroy()
    {
      foreach (var independentVariable in stateController.independentVariables)
      {
        independentVariable.CurrentConditionUpdated -= IIndependentVariable_CurrentConditionUpdated;
      }

      foreach (var cell in GetCells())
      {
        cell.SelectedCell -= Cell_Selected;
        foreach (var item in cell.GetCells())
        {
          item.SelectedItem -= Item_Selected;
        }
      }
    }

    // GridLayoutController methods

    /// <summary>
    /// Calls <see cref="CleanConfigureGrid"/>.
    /// </summary>
    public override void ConfigureGrid()
    {
      StartCoroutine(CleanConfigureGrid());
    }

    // Interfaces methods

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

      ActivateInteractableCells(!IsDragging && !IsZooming);
    }

    public void Drag(Vector3 translation)
    {
      transform.position += Vector3.ProjectOnPlane(translation, transform.up);
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

      ActivateInteractableCells(!IsDragging && !IsZooming);
    }

    public void Zoom(Vector3 distance, Vector3 previousDistance, Vector3 translation, Vector3 previousTranslation)
    {
      var distanceProjected = Vector3.ProjectOnPlane(distance, transform.up);
      var previousDistanceProjected = Vector3.ProjectOnPlane(previousDistance, transform.up);
      var scaleFactor = distanceProjected.magnitude / previousDistanceProjected.magnitude;
      transform.localScale = new Vector3(transform.localScale.x * scaleFactor, transform.localScale.y, transform.localScale.z * scaleFactor);

      var translationProjected = Vector3.ProjectOnPlane(translation, transform.up);
      var previousTranslationProjected = Vector3.ProjectOnPlane(previousTranslation, transform.up);
      transform.position += translationProjected - previousTranslationProjected;
    }

    // Methods

    /// <summary>
    /// Removes the cells in the <see cref="GridLayoutController.GridLayout"/>, calls <see cref="ConfigureGrid"/> and setup the cells with a
    /// <see cref="GridGenerator"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator CleanConfigureGrid()
    {
      // Removes the previous cells
      foreach (var cell in GetCells())
      {
        Destroy(cell.gameObject);
      }
      yield return null;

      // Configure the grid
      base.ConfigureGrid();
      yield return null;

      // Configure the grid of each cell
      int itemsPerCell = 0, itemSize = 0;
      foreach (var cell in GetCells())
      {
        cell.GridSize = cellGridSize;
        cell.ConfigureGrid();

        itemsPerCell = cell.CellsNumberInstantiatedAtConfigure;
        itemSize = cell.CellSize.x;
      }
      yield return null;

      // Configure the collider
      var rectSizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
      collider.center = canvasScaleFactor * new Vector3(0f, -itemSize, 0f);
      collider.size = canvasScaleFactor * new Vector3(rectSizeDelta.x, 3f * itemSize, rectSizeDelta.y);

      DistanceToStartDragging = 0.5f * canvasScaleFactor * itemSize; // Activate panning only if the finger has moved more than half the size of an item

      // Generate a grid generator with average distance in current condition classification distance range
      GridGenerator gridGenerator;
      do
      {
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, itemsPerCell,
        iVClassificationDifficulty.CurrentCondition.IncorrectlyClassifiedCellsFraction,
        (GridGenerator.DistanceTypes)iVClassificationDifficulty.CurrentConditionIndex);
      }
      while (!iVClassificationDifficulty.CurrentCondition.Range.ContainsValue(gridGenerator.AverageDistance));

      // Configure the items of each cell and subscribes to cell and items
      int cellRow = 0, cellColumn = 0;
      foreach (var cell in GetCells())
      {
        cell.ItemClass = (ItemClass)gridGenerator.Cells[cellRow, cellColumn].GetMainItemId();
        cell.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
        cell.ConfigureItems(gridGenerator.Cells[cellRow, cellColumn].items);

        cell.SelectedCell += Cell_Selected;
        foreach (var item in cell.GetCells())
        {
          item.SelectedItem += Item_Selected;
        }

        cellColumn = (cellColumn + 1) % GridSize.x;
        if (cellColumn == 0)
        {
          cellRow = (cellRow + 1) % GridSize.y;
        }
      }
    }

    protected virtual void Cell_Selected(Cell cell)
    {
      if (selectedItem != null)
      {
        foreach (var previouCell in GetCells())
        {
          if (previouCell.Contains(selectedItem))
          {
            previouCell.RemoveItem(selectedItem);
          }
        }

        int itemsMaxNumber = cell.GridSize.x * cell.GridSize.y;
        if (cell.GetCells().Length < itemsMaxNumber)
        {
          cell.AddItem(selectedItem);
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
      ConfigureGrid(); // TODO: only call when state is training or trial
    }

    protected virtual void ActivateInteractableCells(bool value)
    {
      foreach (var cell in GetCells())
      {
        cell.SetInteractable(value);
        foreach (var item in cell.GetCells())
        {
          item.SetInteractable(value);
        }
      }
    }
  }
}
