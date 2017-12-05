using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  public class Cell : GridLayoutController<Item>
  {
    // Properties

    public ItemClass ItemClass { get; set; }

    public int ItemFontSize { get; set; }

    // Variables

    private static ItemClass fakeItemClass = ItemClass.A;

    // Methods

    public override void ConfigureGrid()
    {
      // Computes the item size
      Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
      int cellSize = Mathf.Min((int)sizeDelta.x / GridSize.x, (int)sizeDelta.y / GridSize.y) - 2 * CellMargins;
      CellSize = new Vector2Int(cellSize, cellSize);

      base.ConfigureGrid();

      // Setup the items
      ItemClass = fakeItemClass;
      foreach (var item in GetCells())
      {
        item.ItemClass = ItemClass;
        item.FontSize = ItemFontSize;
        item.SetCorrectlyClassified(true);
      }
      fakeItemClass++;
    }
  }
}
