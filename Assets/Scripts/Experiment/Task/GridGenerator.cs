using NormandErwan.MasterThesisExperiment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NormandErwan.MasterThesisExperiment.Experiment.Task
{
  /// <summary>
  /// Written by Michael J. Mcguffin.
  /// </summary>
  public class GridGenerator
  {
    public enum DistanceTypes
    {
      Short,
      Medium,
      Long
    };

    public struct Cell
    {
      // items[i] stores the item values of the cell.
      // i is between 0 and (ItemsPerCell-1).
      // item value is between 0 and ((RowsNumber * ColumnsNumber) - 1) for a valid item, or itemId==EMPTY_ITEM_ID for "empty".
      public int[] items;

      public int GetMainItemId()
      {
        return items[0];
      }
    };

    protected struct CandidateCell
    {
      public int cellId;
      public int row;
      public int column;
      public float distance;
    };

    // Constants

    public const int EMPTY_ITEM_ID = -1;

    // These are necessary to compute the average distance between cells. These should be measured between the centers of cells.
    // For example, the horizontal spacing is equal to the width of a cell plus the margin between cells.
    // If the caller doesn't care about the effect of aspect ratio of cells
    // on geometric distance, just pass in 1.0f, 1.0f
    protected readonly float verticalSpacingBetweenCells = 1f;
    protected readonly float horizontalSpacingBetweenCells = 1f;

    // Properties

    public int RowsNumber { get; protected set; }
    public int ColumnsNumber { get; protected set; }
    public int ItemsPerCell { get; protected set; }

    // Cells[r,c] stores the cell in the (r)th row and (c)th column.
    // r is between 0 and (NumberRows-1).
    // c is between 0 and (NumberColumns-1).
    public Cell[,] Cells { get; protected set; }

    public float InitiallyIncorrectCellsFraction { get; protected set; } // between 0.0f and 1.0f
    public int IncorrectCellsNumber { get; protected set; }
    
    public float AverageDistance { get; protected set; } // This is the average distance to move all mis-classified items to their correct cell.
    public DistanceTypes DistanceType { get; protected set; }

    protected int cellsNumber; // example value: 5*3 == 15 (we assume one unique itemId per cell)
    protected Random random = new Random();

    // Constructor

    public GridGenerator(int rowsNumber, int columnsNumber, int itemsPerCell, float incorrectCellsFraction, DistanceTypes distanceType)
    {
      RowsNumber = rowsNumber;
      ColumnsNumber = columnsNumber;
      ItemsPerCell = itemsPerCell;

      Cells = new Cell[RowsNumber, ColumnsNumber];

      InitiallyIncorrectCellsFraction = incorrectCellsFraction;
      IncorrectCellsNumber = 0;

      AverageDistance = 0;
      DistanceType = distanceType;

      cellsNumber = RowsNumber * ColumnsNumber;

      // Compute the itemId of each cell.
      // Start with alphabetical ordering ...
      List<int> cellsItemValues = new List<int>();
      for (int itemId = 0; itemId < cellsNumber; ++itemId)
      {
        cellsItemValues.Add(itemId);
      }
      // ... and then randomly permute
      ShuffleList.Shuffle(cellsItemValues);

      // Now fill the cells with items of the appropriate itemId.
      // In each cell, the first few items will be of the correct itemId,
      // and the last item will be empty.
      // Later on, the last item of some cells will be permuted.
      for (int r = 0; r < RowsNumber; ++r)
      {
        for (int c = 0; c < ColumnsNumber; ++c)
        {
          Cells[r, c] = new Cell();
          Cells[r, c].items = new int[ItemsPerCell];

          for (int i = 0; i < ItemsPerCell; ++i)
          {
            Cells[r, c].items[i] = cellsItemValues[r * ColumnsNumber + c];
          }
        }
      }

      // BEGIN: We now want to permute the last items of *some* of the cells.
      // InitiallyIncorrectCellsFraction tells us in how many cells to do this.

      // We first build a list of cells, shuffle it, and then choose a fraction
      // of the cells in that list on which to do more work.
      // To make it easier to identify each cell in the list,
      // rather than storing the (row, column) of each cell in the list,
      // we store a single (cellID), where cellID == (row*NumberColumns + column)
      // uniquely identifies each cell.
      List<int> cellIds = new List<int>();
      for (int i = 0; i < cellsNumber; ++i)
      {
        cellIds.Add(i);
      }
      // randomly permute
      ShuffleList.Shuffle(cellIds);

      // Now imagine the list of cellIDs having length N,
      // and imagine splitting this list into two sublists
      // with indices [0..(k-1)] and [k..N]
      // where N = NumberRows * NumberColumns,
      // and k = InitiallyIncorrectCellsFraction * N.
      // The first sublist is the one where we want to permute the last items
      // of the cells.
      // The second sublist is the one where the cells should remain correct.
      int k = (int)(cellIds.Count * InitiallyIncorrectCellsFraction);
      // Since no further work is required on the cells of the second sublist,
      // forget about it, and just store the first sublist.
      cellIds.RemoveRange(k, cellIds.Count - k);

      // Iterate over the cells in the list (now containing only the first sublist),
      // and set their last items to be empty.
      for (int j = 0; j < cellIds.Count; ++j)
      {
        int r = cellIds[j] / ColumnsNumber;
        int c = cellIds[j] % ColumnsNumber;
        Cells[r, c].items[ItemsPerCell - 1] = EMPTY_ITEM_ID;
      }

      // Now, for each (source) cell remaining in the list cellIDs,
      // we build a list of candidate target cells to which the
      // original last item of the source could be moved.
      // We randomly select among the candidate targets and put the item there.
      for (int i_source = 0; i_source < cellIds.Count; ++i_source)
      {
        int r_source = cellIds[i_source] / ColumnsNumber;
        int c_source = cellIds[i_source] % ColumnsNumber;

        List<CandidateCell> candidates = new List<CandidateCell>();
        for (int i_target = 0; i_target < cellIds.Count; ++i_target)
        {
          if (i_target == i_source)
            continue;

          CandidateCell cc = new CandidateCell();
          cc.cellId = cellIds[i_target];
          cc.row = cc.cellId / ColumnsNumber;
          cc.column = cc.cellId % ColumnsNumber;

          if (Cells[cc.row, cc.column].items[ItemsPerCell - 1] != EMPTY_ITEM_ID)
            continue;

          float dx = (cc.column - c_source) * horizontalSpacingBetweenCells;
          float dy = (cc.row - r_source) * verticalSpacingBetweenCells;
          cc.distance = (float)Math.Sqrt(dx * dx + dy * dy);

          candidates.Add(cc);
        }

        if (candidates.Count == 0)
        {
          // there are no other cells to move the item
          Cells[r_source, c_source].items[ItemsPerCell - 1] = cellsItemValues[cellIds[i_source]];
        }
        else
        {
          // sort the candidates by increasing distance
          candidates = candidates.OrderBy(cc => cc.distance).ToList();

          // Now choose one candidate at random.
          int i = 0;
          if (DistanceType == DistanceTypes.Medium)
            i = random.Next(0, candidates.Count);
          else if (DistanceType == DistanceTypes.Short)
            // i = random.Next( 0, 1 + random.Next(0,candidates.Count) );
            i = random.Next(0, Math.Max(1, candidates.Count / 3));
          else if (DistanceType == DistanceTypes.Long)
            // i = random.Next( random.Next(0,candidates.Count), candidates.Count );
            i = random.Next(2 * candidates.Count / 3, candidates.Count);

          // Now move the original item to the chosen candidate.
          CandidateCell chosen = candidates[i];
          Cells[chosen.row, chosen.column].items[ItemsPerCell - 1] = cellsItemValues[cellIds[i_source]];
          AverageDistance += candidates[i].distance;
          IncorrectCellsNumber++;
        }
      }

      AverageDistance /= IncorrectCellsNumber;
    }

    // Methods

    public bool AreAllCellItemsCorrect(int row, int column)
    {
      for (int i = 1; i < ItemsPerCell; ++i)
      {
        if (Cells[row, column].items[i] == EMPTY_ITEM_ID || Cells[row, column].items[i] != Cells[row, column].GetMainItemId())
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      string s = "";

      for (int r = 0; r < RowsNumber; ++r)
      {
        for (int c = 0; c < ColumnsNumber; ++c)
        {
          if (c > 0) s += "  ";
          for (int i = 0; i < ItemsPerCell; ++i)
          {
            if (i > 0) s += ",";
            s += string.Format("{0,2}", Cells[r, c].items[i]);
          }
        }
        s += "\n";
      }
      s += "Average distance: " + AverageDistance;
      s += "; Initially incorrect cells fraction: " + InitiallyIncorrectCellsFraction;
      s += "; Incorrect cells number: " + IncorrectCellsNumber;
      s += "\n";

      return s;
    }
  }
}
