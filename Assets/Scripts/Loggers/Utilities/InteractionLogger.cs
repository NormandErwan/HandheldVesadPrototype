using System.Collections.Generic;
using System.Diagnostics;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class InteractionLogger
  {
    public string Name { get; protected set; }
    public Stopwatch Time { get; protected set; }

    public int Count { get; set; }
    public float Distance { get; set; }
    public float ProjectedDistance { get; set; }

    public InteractionLogger(string name)
    {
      Name = name;
      Time = new Stopwatch();
      Reset();
    }

    public void Reset()
    {
      Time.Reset();

      Count = 0;
      Distance = 0;
      ProjectedDistance = 0;
    }

    public List<string> Columns()
    {
      return new List<string>() { Name + "_count", Name + "_time", Name + "_distance", Name + "_projected_distance" };
    }
  }
}
