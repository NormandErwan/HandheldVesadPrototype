using NormandErwan.MasterThesis.Experiment.Experiment.States;
using System.Collections;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class MobileDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private StateController stateController;

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Methods

    protected void Start()
    {
      StartCoroutine(SetupCamera());
    }

    protected IEnumerator SetupCamera()
    {
      yield return null;

      // TODO: if in phone only condition
      if (grid.ElementScale.x > 0 && grid.ElementScale.y > 0)
      {
        // TODO: fix
        /*mainCamera.aspect = mobileDeviceSize.x / (float)mobileDeviceSize.y;
        float maxSideLength = Mathf.Max(rect.rect.width / mainCamera.aspect, rect.rect.height);
        float distance = -maxSideLength * scale / (2 * Mathf.Tan(mainCamera.fieldOfView / 2 * Mathf.Deg2Rad));
        mainCamera.transform.localPosition = new Vector3(rect.transform.localPosition.x, rect.transform.localPosition.y, distance);*/
      }
    }
  }
}