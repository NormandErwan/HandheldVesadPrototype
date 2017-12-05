using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class Cell : GridLayoutController<Item>
  {
    // Properties

    public ItemClass ItemClass { get; set; }

    public int ItemFontSize { get; set; }

    // Methods

    public override void ConfigureGrid()
    {
      // Computes the item size
      Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
      int cellSize = Mathf.Min((int)sizeDelta.x / GridSize.x, (int)sizeDelta.y / GridSize.y) - 2 * CellMargins;
      CellSize = new Vector2Int(cellSize, cellSize);

      // Configures the grid
      base.ConfigureGrid();
    }

    public virtual void ConfigureItems(int[] itemValues)
    {
      int index = 0;
      foreach (var item in GetCells())
      {
        item.ItemClass = (ItemClass)itemValues[index];
        item.FontSize = ItemFontSize;
        item.SetCorrectlyClassified(item.ItemClass == ItemClass);
        index++;
      }
    }

    public override int GetCellsNumber()
    {
      return (GridSize.x * GridSize.y) - 1; // -1 because we want to let space for the user to replace each item in its good cell
    }
  }
}
