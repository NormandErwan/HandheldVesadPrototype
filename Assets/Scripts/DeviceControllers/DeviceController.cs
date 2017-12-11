using NormandErwan.MasterThesis.Experiment.Experiment.States;
using System;
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

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }
    public float MaxSelectableDistance { get { return maxSelectableDistance; } set { maxSelectableDistance = value; } }

    // Events

    public event Action RequestActivateTask = delegate { };

    // Methods

    public virtual void ActivateTask()
    {
      grid.Configure(StateController);
      grid.gameObject.SetActive(true);
    }

    protected virtual void OnEnable()
    {
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;
      grid.Finished += Grid_Finished;
    }

    protected virtual void OnDisable()
    {
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;
      grid.Finished -= Grid_Finished;
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      grid.gameObject.SetActive(false);
    }

    protected virtual void Grid_Finished()
    {
      stateController.NextState();
    }

    protected virtual void OnRequestActivateTask()
    {
      RequestActivateTask();
    }
  }
}
