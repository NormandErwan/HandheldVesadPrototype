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
      DeviceController.ConfigureExperiment += DeviceController_ConfigureExperiment;
      DeviceController.RequestActivateTask += DeviceController_RequestActivateTask;
    }

    protected virtual void OnDestroy()
    {
      DeviceController.ConfigureExperiment -= DeviceController_ConfigureExperiment;
      DeviceController.RequestActivateTask -= DeviceController_RequestActivateTask;
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

    protected virtual void DeviceController_ConfigureExperiment()
    {
      currentMessage.configureExperiment = true;
      currentMessage.participantIsRightHanded = deviceController.ParticipantIsRightHanded;
      SendToServer(currentMessage);
    }

    protected virtual void DeviceController_RequestActivateTask()
    {
      currentMessage.activateTask = true;
      SendToServer(currentMessage);
    }

    protected virtual void ProcessReceivedMessage()
    {
      if (currentMessage.configureExperiment)
      {
        deviceController.SetParticipantIsRightHanded(currentMessage.participantIsRightHanded);
      }

      if (currentMessage.activateTask)
      {
        deviceController.ActivateTask();
      }
    }
  }
}
