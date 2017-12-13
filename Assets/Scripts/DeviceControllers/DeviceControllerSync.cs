using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class DeviceControllerSync : DevicesSync
  {
    // Editor Fields

    [SerializeField]
    private DeviceController deviceController;

    // Properties

    public DeviceController DeviceController { get { return deviceController; } set { deviceController = value; } }
    protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

    // Variables

    protected DeviceControllerMessage currentMessage = new DeviceControllerMessage();

    // Methods

    protected override void Awake()
    {
      base.Awake();

      MessageTypes.Add(currentMessage.MessageType);
      DeviceController.ConfigureExperimentSync += DeviceController_ConfigureExperimentSync;
      DeviceController.ActivateTaskSync += DeviceController_ActivateTaskSync;
      DeviceController.ToogleZoomModeSync += DeviceController_ToogleZoomModeSync;
    }

    protected virtual void OnDestroy()
    {
      DeviceController.ConfigureExperimentSync -= DeviceController_ConfigureExperimentSync;
      DeviceController.ActivateTaskSync -= DeviceController_ActivateTaskSync;
      DeviceController.ToogleZoomModeSync -= DeviceController_ToogleZoomModeSync;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      currentMessage = netMessage.ReadMessage<DeviceControllerMessage>();
      ProcessReceivedMessage();
      return currentMessage;
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      currentMessage = netMessage.ReadMessage<DeviceControllerMessage>();
      if (!isServer)
      {
        ProcessReceivedMessage();
      }
      return currentMessage;
    }

    protected virtual void DeviceController_ConfigureExperimentSync()
    {
      currentMessage.participantId = deviceController.ParticipantId;
      currentMessage.conditionsOrdering = deviceController.ConditionsOrdering;
      currentMessage.participantIsRightHanded = deviceController.ParticipantIsRightHanded;

      currentMessage.configureExperiment = true;
      currentMessage.activateTask = false;
      currentMessage.toggleZoomMode = false;
      SendToServer(currentMessage);
    }

    protected virtual void DeviceController_ActivateTaskSync()
    {
      currentMessage.configureExperiment = false;
      currentMessage.activateTask = true;
      currentMessage.toggleZoomMode = false;
      SendToServer(currentMessage);
    }

    protected virtual void DeviceController_ToogleZoomModeSync(bool zoomModeActivated)
    {
      currentMessage.zoomModeActivated = zoomModeActivated;

      currentMessage.configureExperiment = false;
      currentMessage.activateTask = false;
      currentMessage.toggleZoomMode = true;
      SendToServer(currentMessage);
    }

    protected virtual void ProcessReceivedMessage()
    {
      if (currentMessage.configureExperiment)
      {
        deviceController.ConfigureExperiment(currentMessage.participantId, currentMessage.conditionsOrdering,
          currentMessage.participantIsRightHanded);
      }

      if (currentMessage.activateTask)
      {
        deviceController.ActivateTask();
      }

      if (currentMessage.zoomModeActivated)
      {
        deviceController.ZoomModeActivated(currentMessage.zoomModeActivated);
      }
    }
  }
}
