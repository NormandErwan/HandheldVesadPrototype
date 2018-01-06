namespace NormandErwan.MasterThesis.Experiment.Utilities
{
  public class GenericVector3<T>
  {
    // Constructor

    public GenericVector3()
    {
      X = default(T);
      Y = default(T);
      Z = default(T);
    }

    public GenericVector3(T x, T y, T z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    // Properties

    public T X { get { return values[0]; } set { values[0] = value; } }
    public T Y { get { return values[1]; } set { values[1] = value; } }
    public T Z { get { return values[2]; } set { values[2] = value; } }

    // Variables

    private T[] values = new T[3];

    // Methods

    public T this[int index] { get { return values[index]; } set { values[index] = value; } }
  }
}