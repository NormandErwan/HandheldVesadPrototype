using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class GridLayoutController<T> : MonoBehaviour where T : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private GridLayoutGroup gridLayout;

    [SerializeField]
    private Vector2Int gridSize;

    [SerializeField]
    private Vector2Int cellSize;

    [SerializeField]
    private int cellMargins;

    [SerializeField]
    private T cellPrefab;

    // Methods

    public GridLayoutGroup GridLayout { get { return gridLayout; } protected set { gridLayout = value; } }

    public Vector2Int GridSize { get { return gridSize; } protected set { gridSize = value; } }

    public Vector2Int CellSize { get { return cellSize; } protected set { cellSize = value; } }

    public int CellMargins { get { return cellMargins; } protected set { cellMargins = value; } }

    public T CellPrefab { get { return cellPrefab; } protected set { cellPrefab = value; } }

    // Methods

    public virtual void ConfigureGrid()
    {
      // Setup grid layout
      gridLayout.padding = new RectOffset(cellMargins, cellMargins, cellMargins, cellMargins);
      gridLayout.spacing = cellMargins * Vector2.one;
      gridLayout.cellSize = cellSize;
      gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
      gridLayout.constraintCount = gridSize.x;

      // Creates the cells
      for (int i = 0; i < GetCellsNumber(); i++)
      {
        Instantiate(cellPrefab, gridLayout.transform);
      }
    }

    public virtual T[] GetCells()
    {
      return GridLayout.transform.GetComponentsInChildren<T>();
    }

    public virtual int GetCellsNumber()
    {
      return gridSize.x * gridSize.y;
    }
  }
}
