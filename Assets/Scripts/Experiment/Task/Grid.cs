using NormandErwan.MasterThesisExperiment.Experiment.States;
using NormandErwan.MasterThesisExperiment.Experiment.Variables;
using System.Collections;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class Grid : GridLayoutController<Cell>
  {
    // Editor fields

    [SerializeField]
    private Vector2Int cellGridSize;

    [Header("Canvas")]
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private float cavnasScale = 0.0001f;

    [Header("References")]
    [SerializeField]
    private StateController stateController;

    // Variables

    protected IVTextSize ivTextSize;
    protected IVClassificationDifficulty iVClassificationDifficulty;

    // Properties

    public Item SelectedItem { get; protected set; }

    // Methods

    /// <summary>
    /// Calls <see cref="CleanConfigureGrid"/>.
    /// </summary>
    public override void ConfigureGrid()
    {
      StartCoroutine(CleanConfigureGrid());
    }

    public virtual void SetSelectedItem(Item item)
    {
      if (SelectedItem != null)
      {
        SelectedItem.ToggleSelected();
      }

      SelectedItem = item;

      if (SelectedItem != null)
      {
        item.ToggleSelected();
      }
    }

    public virtual void MoveCurrentItemSelected(Cell newCell)
    {
      int itemsMaxNumber = newCell.GridSize.x * newCell.GridSize.y;
      if (newCell.GetCells().Length < itemsMaxNumber)
      {
        SelectedItem.transform.SetParent(newCell.GridLayout.transform);
        SelectedItem.SetCorrectlyClassified(SelectedItem.ItemClass == newCell.ItemClass);
      }
      SetSelectedItem(null);
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

      canvas.GetComponent<RectTransform>().localScale = cavnasScale * Vector3.one; // Scales the canvas as it's in world reference

      ConfigureGrid();
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
      var itemsPerCell = 0;
      foreach (var cell in GetCells())
      {
        cell.GridSize = cellGridSize;
        cell.ConfigureGrid();
        itemsPerCell = cell.CellsNumberInstantiatedAtConfigure;
      }
      yield return null;

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

    private void IIndependentVariable_CurrentConditionUpdated()
    {
      ConfigureGrid();
    }
  }
}
