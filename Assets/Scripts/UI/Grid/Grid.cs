using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.UI.Grid
{
  public abstract class Grid<T, U> : MonoBehaviour, IGrid<T, U>, IGridElement<T>
    where T : IGridElement<T>
    where U : IGridElement<U>
  {
    // Constants

    public readonly int ElementsZOffset = 5;

    // Editor fields

    [Header("Grid")]
    [SerializeField]
    private Transform elementsParent;

    [SerializeField]
    private Vector2Int gridSize;

    [Header("Elements")]
    [SerializeField]
    private Vector2 elementScale;

    [SerializeField]
    private Vector2 elementMargin;

    [SerializeField]
    private U elementPrefab;

    // Interfaces properties

    public GameObject GameObject { get { return gameObject; } }
    public Vector2 Scale { get; set; }
    public Vector2 Margin { get; set; }

    public Vector2Int GridSize { get { return gridSize; } set { gridSize = value; } }
    public Vector2 ElementScale { get { return elementScale; } set { elementScale = value; } }
    public Vector2 ElementMargin { get { return elementMargin; } set { elementMargin = value; } }
    public U ElementPrefab { get { return elementPrefab; } set { elementPrefab = value; } }

    IEnumerable<U> IGrid<T, U>.Elements { get { return Elements; } }
    public bool IsFull { get { return Elements.Count >= GridSize.x * GridSize.y; } }

    // Properties

    public Transform ElementsParent { get { return elementsParent; } set { elementsParent = value; } }
    public int ElementsInstantiatedAtConfigure { get; protected set; }
    public List<U> Elements { get; protected set; }

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      ElementsInstantiatedAtConfigure = GridSize.x * GridSize.y;
      Elements = new List<U>(ElementsInstantiatedAtConfigure);

      foreach (Transform element in ElementsParent)
      {
        Destroy(element.gameObject);
      }
    }

    // IGridElement methods

    public abstract T Instantiate();

    // Methods

    public virtual void Configure()
    {
      Scale = Vector2.Scale(GridSize, ElementScale) + new Vector2((GridSize.x + 1) * ElementMargin.x, (GridSize.y + 1) * ElementMargin.y);

      Elements.Clear();
      for (int i = 0; i < ElementsInstantiatedAtConfigure; i++)
      {
        var element = elementPrefab.Instantiate();
        element.GameObject.transform.SetParent(ElementsParent);
        element.Scale = ElementScale;
        element.Margin = ElementMargin;
        Elements.Add(element);
      }
    }

    public virtual void BuildGrid()
    {
      // Configure grid placement
      var gridPosition = 0.5f * Scale - ElementMargin - 0.5f * ElementScale;
      ElementsParent.localPosition = new Vector3(-gridPosition.x, gridPosition.y, -ElementsZOffset);
      ElementsParent.localScale = Vector3.one;

      // Configure elements placement
      Vector2Int elementPositionOnGrid = Vector2Int.zero;
      foreach (var element in Elements)
      {
        var elementPosition = Vector2.Scale(elementPositionOnGrid, ElementScale + ElementMargin);
        element.GameObject.transform.localPosition = new Vector3(elementPosition.x, -elementPosition.y, 0);
        element.GameObject.transform.localRotation = Quaternion.identity;
        element.GameObject.transform.localScale = Vector3.one;

        elementPositionOnGrid.x = (elementPositionOnGrid.x + 1) % GridSize.x;
        if (elementPositionOnGrid.x == 0)
        {
          elementPositionOnGrid.y = (elementPositionOnGrid.y + 1) % GridSize.y;
        }
      }
    }

    public virtual void CleanGrid()
    {
      foreach (var element in Elements)
      {
        Destroy(element.GameObject);
      }
    }

    public virtual void AddElement(U element)
    {
      element.GameObject.transform.SetParent(ElementsParent);
      Elements.Add(element);
      BuildGrid();
    }

    public virtual void RemoveElement(U element)
    {
      Elements.Remove(element);
      BuildGrid();
    }
  }
}
