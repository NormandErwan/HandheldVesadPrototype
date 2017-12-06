using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class GridLayoutController<T> : MonoBehaviour where T : MonoBehaviour
  {
    // Editor fields

    [Header("Grid")]
    [SerializeField]
    private GridLayoutGroup gridLayout;

    [SerializeField]
    private Vector2Int gridSize;

    [Header("Cell")]
    [SerializeField]
    private Vector2Int cellSize;

    [SerializeField]
    private int cellMargins;

    [SerializeField]
    private T cellPrefab;

    // Properties

    public GridLayoutGroup GridLayout { get { return gridLayout; } set { gridLayout = value; } }

    public Vector2Int GridSize { get { return gridSize; } set { gridSize = value; } }

    public Vector2Int CellSize { get { return cellSize; } set { cellSize = value; } }

    public int CellMargins { get { return cellMargins; } set { cellMargins = value; } }

    public T CellPrefab { get { return cellPrefab; } set { cellPrefab = value; } }

    public int CellsNumberInstantiatedAtConfigure { get; set; }

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
      for (int i = 0; i < CellsNumberInstantiatedAtConfigure; i++)
      {
        Instantiate(cellPrefab, gridLayout.transform);
      }
    }

    public virtual T[] GetCells()
    {
      return GridLayout.transform.GetComponentsInChildren<T>();
    }

    public virtual bool Contains(T cell)
    {
      foreach (var currentCell in GetCells())
      {
        if (currentCell == cell)
        {
          return true;
        }
      }
      return false;
    }

    protected virtual void Awake()
    {
      CellsNumberInstantiatedAtConfigure = GridSize.x * GridSize.y;
    }
  }
}
