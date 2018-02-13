namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public abstract class Counter<T>
  {
    public Counter()
    {
      Reset();
    }

    public bool Started { get; private set; }
    public T Total { get; protected set; }
    public T Current { get; private set; }
    public T Previous { get; private set; }

    public void Reset()
    {
      Started = false;
      Total = default(T);
      Current = default(T);
      Previous = default(T);
    }

    public void Update()
    {
      if (UpdateThisFrame())
      {
        Current = GetCurrent();
        if (!Started)
        {
          Previous = Current;
          Started = true;
        }
        UpdateTotal();
        Previous = Current;
      }
    }

    protected abstract bool UpdateThisFrame();
    protected abstract T GetCurrent();
    protected abstract void UpdateTotal();
  }
}