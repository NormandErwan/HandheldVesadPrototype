using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class Cell : GridLayoutController<Item>
  {
    // Editor fields

    [Header("References")]
    [SerializeField]
    private new BoxCollider collider;

    // Properties

    public ItemClass ItemClass { get; set; }

    public int ItemFontSize { get; set; }

    // Methods

    public override void ConfigureGrid()
    {
      // Compute the item size
      var rectSizeDelta = GetComponent<RectTransform>().sizeDelta;
      int cellSize = Mathf.Min((int)rectSizeDelta.x / GridSize.x, (int)rectSizeDelta.y / GridSize.y) - 2 * CellMargins;
      CellSize = new Vector2Int(cellSize, cellSize);

      // Configure the collider
      collider.center = Vector3.zero;
      collider.size =  new Vector3(rectSizeDelta.x, rectSizeDelta.y, 0.5f * cellSize);

      // Configure the grid
      CellsNumberInstantiatedAtConfigure = (GridSize.x * GridSize.y) - 1; // -1 because we want to let space for the user to replace each item in its good cell
      base.ConfigureGrid();
    }

    public virtual void ConfigureItems(int[] itemValues)
    {
      int index = 0;
      foreach (var item in GetCells())
      {
        item.ItemClass = (ItemClass)itemValues[index];
        item.FontSize = ItemFontSize;
        item.Configure();
        item.SetCorrectlyClassified(item.ItemClass == ItemClass);
        index++;
      }
    }
  }
}
