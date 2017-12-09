using System;
using System.Collections.Generic;

namespace NormandErwan.MasterThesis.Experiment.Utilities
{
  /// <summary>
  /// Written by Michael J. McGuffin.
  /// </summary>
  public static class ShuffleList
  {
    /// <summary>
    /// Performs a random permutation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(IList<T> list)
    {
      Random random = new Random();

      int size = list.Count;
      for (int i = 0; i < size - 1 /*skip last iteration*/; ++i)
      {
        int j = random.Next(size - i); // a random integer in [0,size-i-1]
        j += i; // now j is in [i,size-1]

        // Swap elements i and j.
        // Note that i and j may be equal, which is okay
        // (we're not required to generate a derangement of the input).
        // Usually, however, i and j won't be equal,
        // hence we don't bother checking for it.
        T tmp = list[i];
        list[i] = list[j];
        list[j] = tmp;
      }
    }
  }
}
