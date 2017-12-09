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

    public struct Container
    {
      // items[i] stores the item values of the container.
      // i is between 0 and (ItemsPerContainer-1).
      // item value is between 0 and ((RowsNumber * ColumnsNumber) - 1) for a valid item, or itemId==EMPTY_ITEM_ID for "empty".
      public int[] items;

      public int GetMainItemId()
      {
        return items[0];
      }
    };

    protected struct CandidateContainer
    {
      public int containerId;
      public int row;
      public int column;
      public float distance;
    };

    // Constants

    public const int EMPTY_ITEM_ID = -1;

    // These are necessary to compute the average distance between containers. These should be measured between the centers of containers.
    // For example, the horizontal spacing is equal to the width of a container plus the margin between containers.
    // If the caller doesn't care about the effect of aspect ratio of containers
    // on geometric distance, just pass in 1.0f, 1.0f
    protected readonly float verticalSpacingBetweenContainers = 1f;
    protected readonly float horizontalSpacingBetweenContainers = 1f;

    // Properties

    public int RowsNumber { get; protected set; }
    public int ColumnsNumber { get; protected set; }
    public int ItemsPerContainer { get; protected set; }

    // Containers[r,c] stores the container in the (r)th row and (c)th column.
    // r is between 0 and (NumberRows-1).
    // c is between 0 and (NumberColumns-1).
    public Container[,] Containers { get; protected set; }

    public float InitiallyIncorrectContainersFraction { get; protected set; } // between 0.0f and 1.0f
    public int IncorrectContainersNumber { get; protected set; }
    
    public float AverageDistance { get; protected set; } // This is the average distance to move all mis-classified items to their correct container.
    public DistanceTypes DistanceType { get; protected set; }

    protected int containersNumber; // example value: 5*3 == 15 (we assume one unique itemId per container)
    protected Random random = new Random();

    // Constructor

    public GridGenerator(int rowsNumber, int columnsNumber, int itemsPerContainer, float incorrectContainersFraction, DistanceTypes distanceType)
    {
      RowsNumber = rowsNumber;
      ColumnsNumber = columnsNumber;
      ItemsPerContainer = itemsPerContainer;

      Containers = new Container[RowsNumber, ColumnsNumber];

      InitiallyIncorrectContainersFraction = incorrectContainersFraction;
      IncorrectContainersNumber = 0;

      AverageDistance = 0;
      DistanceType = distanceType;

      containersNumber = RowsNumber * ColumnsNumber;

      // Compute the itemId of each container.
      // Start with alphabetical ordering ...
      List<int> containersItemValues = new List<int>();
      for (int itemId = 0; itemId < containersNumber; ++itemId)
      {
        containersItemValues.Add(itemId);
      }
      // ... and then randomly permute
      ShuffleList.Shuffle(containersItemValues);

      // Now fill the containers with items of the appropriate itemId.
      // In each container, the first few items will be of the correct itemId,
      // and the last item will be empty.
      // Later on, the last item of some containers will be permuted.
      for (int r = 0; r < RowsNumber; ++r)
      {
        for (int c = 0; c < ColumnsNumber; ++c)
        {
          Containers[r, c] = new Container();
          Containers[r, c].items = new int[ItemsPerContainer];

          for (int i = 0; i < ItemsPerContainer; ++i)
          {
            Containers[r, c].items[i] = containersItemValues[r * ColumnsNumber + c];
          }
        }
      }

      // BEGIN: We now want to permute the last items of *some* of the containers.
      // InitiallyIncorrectContainersFraction tells us in how many containers to do this.

      // We first build a list of containers, shuffle it, and then choose a fraction
      // of the containers in that list on which to do more work.
      // To make it easier to identify each container in the list,
      // rather than storing the (row, column) of each container in the list,
      // we store a single (containerID), where containerID == (row*NumberColumns + column)
      // uniquely identifies each container.
      List<int> containerIds = new List<int>();
      for (int i = 0; i < containersNumber; ++i)
      {
        containerIds.Add(i);
      }
      // randomly permute
      ShuffleList.Shuffle(containerIds);

      // Now imagine the list of containerIDs having length N,
      // and imagine splitting this list into two sublists
      // with indices [0..(k-1)] and [k..N]
      // where N = NumberRows * NumberColumns,
      // and k = InitiallyIncorrectContainersFraction * N.
      // The first sublist is the one where we want to permute the last items
      // of the containers.
      // The second sublist is the one where the containers should remain correct.
      int k = (int)(containerIds.Count * InitiallyIncorrectContainersFraction);
      // Since no further work is required on the containers of the second sublist,
      // forget about it, and just store the first sublist.
      containerIds.RemoveRange(k, containerIds.Count - k);

      // Iterate over the containers in the list (now containing only the first sublist),
      // and set their last items to be empty.
      for (int j = 0; j < containerIds.Count; ++j)
      {
        int r = containerIds[j] / ColumnsNumber;
        int c = containerIds[j] % ColumnsNumber;
        Containers[r, c].items[ItemsPerContainer - 1] = EMPTY_ITEM_ID;
      }

      // Now, for each (source) container remaining in the list containerIDs,
      // we build a list of candidate target containers to which the
      // original last item of the source could be moved.
      // We randomly select among the candidate targets and put the item there.
      for (int i_source = 0; i_source < containerIds.Count; ++i_source)
      {
        int r_source = containerIds[i_source] / ColumnsNumber;
        int c_source = containerIds[i_source] % ColumnsNumber;

        List<CandidateContainer> candidates = new List<CandidateContainer>();
        for (int i_target = 0; i_target < containerIds.Count; ++i_target)
        {
          if (i_target == i_source)
            continue;

          CandidateContainer cc = new CandidateContainer();
          cc.containerId = containerIds[i_target];
          cc.row = cc.containerId / ColumnsNumber;
          cc.column = cc.containerId % ColumnsNumber;

          if (Containers[cc.row, cc.column].items[ItemsPerContainer - 1] != EMPTY_ITEM_ID)
            continue;

          float dx = (cc.column - c_source) * horizontalSpacingBetweenContainers;
          float dy = (cc.row - r_source) * verticalSpacingBetweenContainers;
          cc.distance = (float)Math.Sqrt(dx * dx + dy * dy);

          candidates.Add(cc);
        }

        if (candidates.Count == 0)
        {
          // there are no other containers to move the item
          Containers[r_source, c_source].items[ItemsPerContainer - 1] = containersItemValues[containerIds[i_source]];
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
          CandidateContainer chosen = candidates[i];
          Containers[chosen.row, chosen.column].items[ItemsPerContainer - 1] = containersItemValues[containerIds[i_source]];
          AverageDistance += candidates[i].distance;
          IncorrectContainersNumber++;
        }
      }

      AverageDistance /= IncorrectContainersNumber;
    }

    // Methods

    public bool AreAllContainerItemsCorrect(int row, int column)
    {
      for (int i = 1; i < ItemsPerContainer; ++i)
      {
        if (Containers[row, column].items[i] == EMPTY_ITEM_ID || Containers[row, column].items[i] != Containers[row, column].GetMainItemId())
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
          for (int i = 0; i < ItemsPerContainer; ++i)
          {
            if (i > 0) s += ",";
            s += string.Format("{0,2}", Containers[r, c].items[i]);
          }
        }
        s += "\n";
      }
      s += "Average distance: " + AverageDistance;
      s += "; Initially incorrect containers fraction: " + InitiallyIncorrectContainersFraction;
      s += "; Incorrect containers number: " + IncorrectContainersNumber;
      s += "\n";

      return s;
    }
  }
}
