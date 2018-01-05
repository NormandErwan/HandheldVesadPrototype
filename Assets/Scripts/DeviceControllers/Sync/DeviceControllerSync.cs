using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers.Sync
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

    protected ConfigureExperimentMessage configureExperimentMessage = new ConfigureExperimentMessage();
    protected ActivateTaskMessage activateTaskMessage = new ActivateTaskMessage();
    protected ToggleZoomMessage toggleZoomMessage = new ToggleZoomMessage();

    // Methods

    protected override void Awake()
    {
      base.Awake();

      MessageTypes.Add(configureExperimentMessage.MessageType);
      MessageTypes.Add(activateTaskMessage.MessageType);
      MessageTypes.Add(toggleZoomMessage.MessageType);

      DeviceController.ConfigureExperimentSync += DeviceController_ConfigureExperimentSync;
      DeviceController.ActivateTaskSync += DeviceController_ActivateTaskSync;
      DeviceController.ToogleZoomSync += DeviceController_ToogleZoomSync;
    }

    protected virtual void OnDestroy()
    {
      DeviceController.ConfigureExperimentSync -= DeviceController_ConfigureExperimentSync;
      DeviceController.ActivateTaskSync -= DeviceController_ActivateTaskSync;
      DeviceController.ToogleZoomSync -= DeviceController_ToogleZoomSync;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, false);
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, true);
    }

    protected virtual void DeviceController_ConfigureExperimentSync()
    {
      configureExperimentMessage.participantId = DeviceController.ParticipantId;
      configureExperimentMessage.conditionsOrdering = DeviceController.ConditionsOrdering;
      configureExperimentMessage.participantIsRightHanded = DeviceController.ParticipantIsRightHanded;
      SendToServer(configureExperimentMessage);
    }

    protected virtual void DeviceController_ActivateTaskSync()
    {
      SendToServer(activateTaskMessage);
    }

    protected virtual void DeviceController_ToogleZoomSync(bool zoomActivated)
    {
      toggleZoomMessage.activated = zoomActivated;
      SendToServer(toggleZoomMessage);
    }

    protected virtual DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
    {
      DevicesSyncMessage devicesSyncMessage = null;
      if (netMessage.msgType == MessageType.DeviceControllerConfigureExperiment)
      {
        if (!onClient || (onClient && !isServer))
        {
          var configureExperimentMessage = netMessage.ReadMessage<ConfigureExperimentMessage>();
          devicesSyncMessage = configureExperimentMessage;

          DeviceController.ConfigureExperiment(configureExperimentMessage.participantId,
            configureExperimentMessage.conditionsOrdering, configureExperimentMessage.participantIsRightHanded);
        }
      }
      else if (netMessage.msgType == MessageType.DeviceControllerActivateTask)
      {
        if (!onClient || (onClient && !isServer))
        {
          devicesSyncMessage = netMessage.ReadMessage<ActivateTaskMessage>();

          DeviceController.ActivateTask();
        }
      }
      else if (netMessage.msgType == MessageType.DeviceControllerToggleZoom)
      {
        if (!onClient || (onClient && !isServer))
        {
          var toggleZoomMessage = netMessage.ReadMessage<ToggleZoomMessage>();
          devicesSyncMessage = toggleZoomMessage;

          DeviceController.ToggleZoom(toggleZoomMessage.activated);
        }
      }
      return devicesSyncMessage;
    }
  }
}
