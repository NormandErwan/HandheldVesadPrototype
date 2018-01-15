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

    protected DeviceControllerSyncMessage deviceControllerMessage;

    // Methods

    protected override void Awake()
    {
      base.Awake();

      deviceControllerMessage = new DeviceControllerSyncMessage(DeviceController, () => 
      {
        SendToServer(deviceControllerMessage, Channels.DefaultReliable);
      });
      MessageTypes.Add(deviceControllerMessage.MessageType);
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, false);
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, true);
    }

    protected virtual DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
    {
      DevicesSyncMessage devicesSyncMessage = null;
      if (!onClient || (onClient && !isServer))
      {
        devicesSyncMessage = ProcessReceivedMessage<DeviceControllerSyncMessage>(netMessage, deviceControllerMessage.MessageType,
          (deviceControllerMessage) =>
          {
            deviceControllerMessage.Sync(DeviceController);
          });
      }
      return devicesSyncMessage;
    }
  }
}
