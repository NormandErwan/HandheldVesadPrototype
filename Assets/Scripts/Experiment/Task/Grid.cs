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
    private Canvas canvas;

    [SerializeField]
    private float scale = 0.0001f;

    [SerializeField]
    private StateManager stateManager;

    // Variables

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
    /// Gets and subscribes to the independent variables, and calls <see cref="ConfigureGrid"/>.
    /// </summary>
    protected virtual void Start()
    {
      ivTextSize = stateManager.GetIndependentVariable<IVTextSize>();
      iVClassificationDifficulty = stateManager.GetIndependentVariable<IVClassificationDifficulty>();

      foreach (var independentVariable in stateManager.independentVariables)
      {
        independentVariable.CurrentConditionUpdated += IIndependentVariable_CurrentConditionUpdated;
      }

      canvas.GetComponent<RectTransform>().localScale = scale * Vector3.one; // Scales the canvas as it's in world reference

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
      foreach (Transform cell in GridLayout.transform)
      {
        Destroy(cell.gameObject);
      }
      yield return null;

      // Configure the default grid one frame later after previous cells have been removed
      base.ConfigureGrid();

      // Generates a grid generator with average distance in current condition classification distance range
      GridGenerator gridGenerator;
      do
      {
        gridGenerator = new GridGenerator(GridSize.y, GridSize.x, CellPrefab.GetCellsNumber(),
        iVClassificationDifficulty.CurrentCondition.IncorrectlyClassifiedCellsFraction,
        (GridGenerator.DistanceTypes)iVClassificationDifficulty.CurrentConditionIndex);
      }
      while (!iVClassificationDifficulty.CurrentCondition.Range.ContainsValue(gridGenerator.AverageDistance));

      // Configures each cell
      int cellRow = 0, cellColumn = 0;
      foreach (var cell in GetCells())
      {
        cell.ItemClass = (ItemClass)gridGenerator.Cells[cellRow, cellColumn].GetMainItemId();
        cell.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
        cell.ConfigureGrid();
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
