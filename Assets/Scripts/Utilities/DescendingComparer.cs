using System;
using System.Collections.Generic;

namespace NormandErwan.MasterThesis.Experiment.Utilities
{
  public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
  {
    public int Compare(T x, T y)
    {
      return y.CompareTo(x);
    }
  }
}
