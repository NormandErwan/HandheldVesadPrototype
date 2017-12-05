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

    IVTextSize ivTextSize;

    // Methods

    public override void ConfigureGrid()
    {
      base.ConfigureGrid();

      // Subscribe to ivTextSize
      if (ivTextSize != null)
      {
        ivTextSize.CurrentConditionUpdated -= IvTextSize_CurrentConditionUpdated;
      }

      foreach (var independentVariable in stateManager.independentVariables)
      {
        ivTextSize = independentVariable as IVTextSize;
        if (ivTextSize != null)
        {
          ivTextSize.CurrentConditionUpdated += IvTextSize_CurrentConditionUpdated;
          break;
        }
      }

      // Scales the canvas as it's in world reference
      canvas.GetComponent<RectTransform>().localScale = scale * Vector3.one;

      // Setup the cells
      foreach (var cell in GetCells())
      {
        cell.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
        cell.ConfigureGrid();
      }
    }

    /// <summary>
    /// Calls <see cref="CleanConfigureGrid"/>.
    /// </summary>
    protected virtual void Start()
    {
      StartCoroutine(CleanConfigureGrid());
    }

    /// <summary>
    /// Removes the cells in the <see cref="GridLayoutController.GridLayout"/> then calls <see cref="ConfigureGrid"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator CleanConfigureGrid()
    {
      foreach (Transform cell in GridLayout.transform)
      {
        Destroy(cell.gameObject);
      }

      yield return null;

      ConfigureGrid();
    }

    /// <summary>
    /// Updates the <see cref="Item.FontSize"/> when the <see cref="IVTextSize"/> current condition has been updated.
    /// </summary>
    protected virtual void IvTextSize_CurrentConditionUpdated(IVTextSizeCondition condition)
    {
      foreach (var cell in GetCells())
      {
        cell.ItemFontSize = ivTextSize.CurrentCondition.fontSize;
      }
    }
  }
}
