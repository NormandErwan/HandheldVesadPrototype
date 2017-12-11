using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public abstract class DeviceController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private StateController stateController;

    [SerializeField]
    private Experiment.Task.Grid grid;

    [SerializeField]
    [Range(0f, 0.02f)]
    private float maxSelectableDistance = 0.001f;

    // Properties

    public float MaxSelectableDistance { get { return maxSelectableDistance; } set { maxSelectableDistance = value; } }

    // Methods

    protected virtual void OnEnable()
    {
      stateController.CurrentStateUpdated += StateController_CurrentStateUpdated;
      grid.Finished += Grid_Finished;
    }

    protected virtual void OnDisable()
    {
      grid.Finished -= Grid_Finished;
    }

    private void StateController_CurrentStateUpdated(State currentState)
    {
      if (currentState.ActivateTask)
      {
        grid.Configure(stateController);
        grid.gameObject.SetActive(true);
      }
      else
      {
        grid.gameObject.SetActive(false);
      }
    }

    private void Grid_Finished()
    {
      stateController.NextState();
    }
  }
}
