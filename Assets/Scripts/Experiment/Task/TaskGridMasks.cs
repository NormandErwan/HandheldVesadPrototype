using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task
{
  public class TaskGridMasks : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private GameObject maskPrefab;

    [SerializeField]
    private TaskGrid taskGrid;

    // Variables

    protected Vector3 maskScale;

    protected GameObject centerMask;

    protected Transform sideMasksParent;
    protected GameObject[] sideMasks = new GameObject[4];
    protected Vector3[] sideMasksPositions = new Vector3[2]
    {
      new Vector3(0, 1.5f, 0), // up and bottom sides
      new Vector3(1.5f, 0, 0) // left and right sides
    };
    protected Vector3[] sideMaskScales = new Vector3[2]
    {
      new Vector3(5, 2, 1), // up and bottom sides
      new Vector3(2, 1, 1) // left and right sides
    };

    // Methods

    protected virtual void Awake()
    {
      maskScale = maskPrefab.transform.localScale;

      centerMask = Instantiate(maskPrefab, transform);
      centerMask.transform.localPosition = Vector3.zero;
      centerMask.transform.localRotation = Quaternion.identity;
      centerMask.name = "MobileDeviceMask";

      sideMasksParent = new GameObject("SideMasks").transform;
      sideMasksParent.SetParent(transform);
      sideMasksParent.transform.localPosition = Vector3.zero;
      sideMasksParent.transform.localRotation = Quaternion.identity;
      sideMasksParent.transform.localScale = Vector3.one;

      for (int i = 0; i < sideMasks.Length; i++)
      {
        var sideMask = Instantiate(maskPrefab, sideMasksParent);
        sideMask.transform.localPosition = Vector3.zero;
        sideMask.transform.localRotation = Quaternion.identity;
        sideMask.name = "SideMask (" + i + ")";
        sideMasks[i] = sideMask;
      }

      Hide();

      taskGrid.Dragging += TaskGrid_Dragging;
    }

    protected virtual void OnDestroy()
    {
      taskGrid.Dragging -= TaskGrid_Dragging;
    }

    public virtual void Configure(bool showGrid)
    {
      if (showGrid)
      {
        centerMask.transform.SetParent(transform);
        centerMask.transform.localScale = Vector3.Scale(taskGrid.ElementScale, taskGrid.transform.lossyScale) + maskScale;
      }
      else
      {
        centerMask.transform.SetParent(sideMasksParent);
        centerMask.transform.localScale = new Vector3(taskGrid.LossyScale.x, taskGrid.LossyScale.y, maskScale.z);
      }
      centerMask.transform.localPosition = Vector3.zero;
      centerMask.transform.localRotation = Quaternion.identity;

      for (int i = 0; i < sideMasks.Length; i++)
      {
        sideMasks[i].transform.localPosition = Vector3.Scale((i / 2 == 0 ? 1 : -1) * sideMasksPositions[i % 2], taskGrid.LossyScale);
        sideMasks[i].transform.localScale = Vector3.Scale(sideMaskScales[i % 2], taskGrid.LossyScale) + maskScale;
      }

      SetMaskActives(true);
    }

    public virtual void Hide()
    {
      SetMaskActives(false);
    }

    protected virtual void TaskGrid_Dragging(IDraggable taskGrid, Vector3 translation)
    {
      sideMasksParent.localPosition += translation;
    }

    protected virtual void SetMaskActives(bool value)
    {
      centerMask.SetActive(value);
      foreach (var sideMask in sideMasks)
      {
        sideMask.SetActive(value);
      }
    }
  }
}