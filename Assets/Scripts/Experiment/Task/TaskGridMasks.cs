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
    protected Vector3 maskPositionOffset;

    protected GameObject centerMask;

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
      maskPositionOffset = new Vector3(0f, 0f, 40f / 100f * maskScale.z);

      centerMask = Instantiate(maskPrefab, transform);
      centerMask.transform.localPosition = maskPositionOffset;
      centerMask.transform.localRotation = Quaternion.identity;
      centerMask.name = "MobileDeviceMask";

      for (int i = 0; i < sideMasks.Length; i++)
      {
        var sideMask = Instantiate(maskPrefab, transform);
        sideMask.transform.localPosition = Vector3.zero;
        sideMask.transform.localRotation = Quaternion.identity;
        sideMask.name = "SideMask (" + i + ")";
        sideMasks[i] = sideMask;
      }

      Hide();
    }

    public virtual void Configure()
    {
      centerMask.transform.localScale = Vector3.Scale(taskGrid.ElementScale + taskGrid.ElementMargin, taskGrid.transform.lossyScale) + maskScale;
      for (int i = 0; i < sideMasks.Length; i++)
      {
        sideMasks[i].transform.localPosition = Vector3.Scale((i / 2 == 0 ? 1 : -1) * sideMasksPositions[i % 2], taskGrid.LossyScale) + maskPositionOffset;
        sideMasks[i].transform.localScale = Vector3.Scale(sideMaskScales[i % 2], taskGrid.LossyScale) + maskScale;
      }

      SetMaskActives(true);
    }

    public virtual void Hide()
    {
      SetMaskActives(false);
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