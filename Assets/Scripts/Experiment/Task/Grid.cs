using NormandErwan.MasterThesisExperiment.Experiment.States;
using NormandErwan.MasterThesisExperiment.Experiment.Variables;
using NormandErwan.MasterThesisExperiment.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  [RequireComponent(typeof(BoxCollider))]
  public class Grid : GridLayoutController<Cell>
  {
    public enum Mode
    {
      Idle,
      Panning,
      Zooming
    }

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

    // Properties

    public Mode CurrentMode { get; protected set; }

    public Item SelectedItem { get; protected set; }

    // Variables

    protected new BoxCollider collider;
    protected List<HoverCursorController> triggeredFingers = new List<HoverCursorController>();
    protected Vector3 fingerPanningLastPosition;

    protected int cellItemSize;

    protected IVTextSize ivTextSize;
    protected IVClassificationDifficulty iVClassificationDifficulty;

    // Methods

    /// <summary>
    /// Calls <see cref="CleanConfigureGrid"/>.
    /// </summary>
    public override void ConfigureGrid()
    {
      StartCoroutine(CleanConfigureGrid());
    }

    /// <summary>
    /// Calls <see cref="Item.SetSelected"/> on the previous selected item, set the <see cref="SelectedItem"/> property with the new value and
    /// calls <see cref="Item.SetSelected"/> on it if not null.
    /// </summary>
    /// <param name="item"></param>
    public virtual void SetSelectedItem(Item item)
    {
      if (SelectedItem != null)
      {
        SelectedItem.SetSelected(false);
      }

      SelectedItem = item;

      if (SelectedItem != null)
      {
        item.SetSelected(true);
      }
    }

    /// <summary>
    /// Moves the <see cref="SelectedItem"/> to the <paramref name="cell"/> if not full, and deselect the selected item.
    /// </summary>
    /// <param name="cell"></param>
    public virtual void MoveSelectedItemTo(Cell cell)
    {
      int itemsMaxNumber = cell.GridSize.x * cell.GridSize.y;
      if (cell.GetCells().Length < itemsMaxNumber)
      {
        SelectedItem.transform.SetParent(cell.GridLayout.transform);
        SelectedItem.SetCorrectlyClassified(SelectedItem.ItemClass == cell.ItemClass);
      }
      SetSelectedItem(null);
    }

    protected override void Awake()
    {
      base.Awake();
      collider = GetComponent<BoxCollider>();
      CurrentMode = Mode.Idle;
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

      ConfigureGrid();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      var cursor = other.GetComponent<HoverCursorController>();
      if (cursor != null && cursor.IsFinger)
      {
        triggeredFingers.Add(cursor);
        if (triggeredFingers.Count == 1)
        {
          fingerPanningLastPosition = Vector3.ProjectOnPlane(cursor.transform.position, transform.up);
        }
        if (triggeredFingers.Count >= 2)
        {
          CurrentMode = Mode.Zooming;
        }
      }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      var cursor = other.GetComponent<HoverCursorController>();
      if (cursor != null && cursor.IsFinger && triggeredFingers.Count == 1)
      {
        var fingerPanningPosition = Vector3.ProjectOnPlane(cursor.transform.position, transform.up);
        var fingerPanningVector = fingerPanningPosition - fingerPanningLastPosition;

        if (CurrentMode == Mode.Panning)
        {
          transform.position += fingerPanningVector;
          fingerPanningLastPosition = fingerPanningPosition;
        }
        else if (fingerPanningVector.magnitude > 0.5f * canvasScaleFactor * cellItemSize) // Activate panning only if the finger has moved more than half the size of an item
        {
          CurrentMode = Mode.Panning;
          fingerPanningLastPosition = fingerPanningPosition;
        }
      }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      var cursor = other.GetComponent<HoverCursorController>();
      if (cursor != null && cursor.IsFinger)
      {
        triggeredFingers.Remove(cursor);
        if (triggeredFingers.Count == 0) // Go back to idle mode only when all the fingers have released from the grid
        {
          CurrentMode = Mode.Idle;
        }
      }
    }

    protected virtual void Update()
    {
      if (CurrentMode == Mode.Zooming) // In update and not in OnTriggerStay to execute only once per frame
      {
        // TODO
      }
    }

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
      int itemsPerCell = 0;
      foreach (var cell in GetCells())
      {
        cell.GridSize = cellGridSize;
        cell.ConfigureGrid();

        itemsPerCell = cell.CellsNumberInstantiatedAtConfigure;
        cellItemSize = cell.CellSize.x;
      }
      yield return null;

      // Configure the collider
      var rectSizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
      collider.center = Vector3.zero;
      collider.size = canvasScaleFactor * new Vector3(rectSizeDelta.x, 0.5f * cellItemSize, rectSizeDelta.y);

      // Generate a grid generator with average distance in current condition classification distance range
      GridGenerator gridGenerator;
      do
      {
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, itemsPerCell,
        iVClassificationDifficulty.CurrentCondition.IncorrectlyClassifiedCellsFraction,
        (GridGenerator.DistanceTypes)iVClassificationDifficulty.CurrentConditionIndex);
      }
      while (!iVClassificationDifficulty.CurrentCondition.Range.ContainsValue(gridGenerator.AverageDistance));

      // Configure the items of each cell
      int cellRow = 0, cellColumn = 0;
      foreach (var cell in GetCells())
      {
        cell.ItemClass = (ItemClass)gridGenerator.Cells[cellRow, cellColumn].GetMainItemId();
        cell.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
        cell.ConfigureItems(gridGenerator.Cells[cellRow, cellColumn].items);

        cellColumn = (cellColumn + 1) % GridSize.x;
        if (cellColumn == 0)
        {
          cellRow = (cellRow + 1) % GridSize.y;
        }
      }
    }

    protected virtual void IIndependentVariable_CurrentConditionUpdated()
    {
      ConfigureGrid();
    }
  }
}
