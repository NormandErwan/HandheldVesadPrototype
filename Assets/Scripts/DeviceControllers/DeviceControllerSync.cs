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
      DeviceController.RequestActivateTask += DeviceController_RequestActivateTask;
    }

    protected virtual void OnDestroy()
    {
      DeviceController.RequestActivateTask -= DeviceController_RequestActivateTask;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      currentMessage = netMessage.ReadMessage<DeviceControllerMessage>();
      if (currentMessage.activateTask)
      {
        deviceController.ActivateTask();
      }
      return currentMessage;
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      currentMessage = netMessage.ReadMessage<DeviceControllerMessage>();
      if (!isServer)
      {
        if (currentMessage.activateTask)
        {
          deviceController.ActivateTask();
        }
      }
      return currentMessage;
    }

    protected virtual void DeviceController_RequestActivateTask()
    {
      currentMessage.activateTask = true;
      SendToServer(currentMessage);
    }
  }
}
