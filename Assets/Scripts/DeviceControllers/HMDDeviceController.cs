using DevicesSyncUnity;
using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Loggers;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;
using System.Collections;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class HMDDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private ParticipantLogger participantLogger;

    [SerializeField]
    private HMDDeviceHUD hmdDeviceHUD;

    [SerializeField]
    private GameObject leftLeapMotionHand;

    [SerializeField]
    private GameObject leftHandCursors;

    [SerializeField]
    private GameObject rightLeapMotionHand;

    [SerializeField]
    private GameObject rightHandCursors;

    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    // Properties

    public ParticipantLogger ParticipantLogger { get { return participantLogger; } set { participantLogger = value; } }
    public HMDDeviceHUD HMDDeviceHUD { get { return hmdDeviceHUD; } set { hmdDeviceHUD = value; } }
    public LeapFingerCursorsInput LeapFingerCursorsInput { get { return leapFingerCursorsInput; } set { leapFingerCursorsInput = value; } }

    // Methods

    protected override void Start()
    {
      base.Start();

      LeapFingerCursorsInput.Configure(maxSelectableDistance);
      LeapFingerCursorsInput.gameObject.SetActive(false);
      ActivateHand(true, false);
      ActivateHand(false, false);

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    private void Update()
    {
      if (Input.GetKeyUp(KeyCode.C))
      {
        Grid_Completed();
        StateController.NextState();
        ActivateTask();
        Grid.Configure();
      }
    }

    private IEnumerator StartTaskDebug()
    {
      yield return null;
      OnConfigureExperimentSync();
      StateController.BeginExperiment();

      yield return null;
      StateController.NextState();

      yield return null;
      ActivateTask();
      Grid.Configure();
    }

    // DeviceController methods

    public override void ConfigureExperiment(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      base.ConfigureExperiment(participantId, conditionsOrdering, participantIsRightHanded);

      ParticipantLogger.ParticipantId = ParticipantId;
      ParticipantLogger.StartLogger();
    }

    public override void ActivateTask()
    {
      base.ActivateTask();
      HMDDeviceHUD.ShowContent(false);
    }

    public override void ToggleZoom(bool activated)
    {
      base.ToggleZoom(activated);
      // TODO
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      bool useLeapInput = ivTechnique.CurrentCondition.useLeapInput;
      LeapFingerCursorsInput.gameObject.SetActive(currentState.ActivateTask && useLeapInput);
      ActivateHand(ParticipantIsRightHanded, currentState.ActivateTask && useLeapInput);

      // TODO: deactivate zoom mode

      HMDDeviceHUD.ShowContent(true);
      HMDDeviceHUD.UpdateInstructionsProgress(StateController);
    }

    // Methods

    protected virtual void ActivateHand(bool rightHand, bool value)
    {
      if (rightHand)
      {
        rightLeapMotionHand.SetActive(value);
        rightHandCursors.SetActive(value);
      }
      else
      {
        leftLeapMotionHand.SetActive(value);
        leftHandCursors.SetActive(value);
      }
    }

    protected override void Grid_Configured()
    {
      base.Grid_Configured();

      ParticipantLogger.PrepareNextRow();
      ParticipantLogger.Technique = ivTechnique.CurrentCondition.id;
      ParticipantLogger.TextSize = ivTextSize.CurrentCondition.id;
      ParticipantLogger.ClassificationDistance = ivClassificationDifficulty.CurrentCondition.id;
      ParticipantLogger.TrialNumber = StateController.CurrentTrial;
    }

    protected override void Grid_Completed()
    {
      base.Grid_Completed();
      ParticipantLogger.WriteRow();
    }
  }
}