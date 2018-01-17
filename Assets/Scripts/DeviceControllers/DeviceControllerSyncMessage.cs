using DevicesSyncUnity.Messages;
using System;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class DeviceControllerSyncMessage : DevicesSyncMessage
  {
    public enum Type
    {
      Configure,
      ActivateTask,
      ToggleZoom
    }

    // Constructors and destructor

    public DeviceControllerSyncMessage(DeviceController deviceController, Action sendToServer)
    {
      DeviceController = deviceController;
      SendToServer = sendToServer;

      DeviceController.ConfigureSync += DeviceController_ConfigureSync;
      DeviceController.ActivateTaskSync += DeviceController_ActivateTaskSync;
      DeviceController.SetDragToZoomSync += DeviceController_ToogleZoomSync;
    }

    public DeviceControllerSyncMessage()
    {
    }

    ~DeviceControllerSyncMessage()
    {
      DeviceController.ConfigureSync -= DeviceController_ConfigureSync;
      DeviceController.ActivateTaskSync -= DeviceController_ActivateTaskSync;
      DeviceController.SetDragToZoomSync -= DeviceController_ToogleZoomSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.DeviceController; } }

    protected DeviceController DeviceController { get; }
    protected Action SendToServer { get; }

    // Variables

    public int senderConnectionId;

    public Type type;

    public bool zoomActivated;

    public int participantId;
    public int conditionsOrdering;
    public bool participantIsRightHanded;

    // Methods

    public void Sync(DeviceController deviceController)
    {
      if (type == Type.Configure)
      {
        deviceController.Configure(participantId, conditionsOrdering, participantIsRightHanded);
      }
      else if (type == Type.ActivateTask)
      {
        deviceController.ActivateTask();
      }
      else if (type == Type.ToggleZoom)
      {
        deviceController.SetDragToZoom(zoomActivated);
      }
    }

    protected virtual void DeviceController_ConfigureSync()
    {
      type = Type.Configure;
      participantId = DeviceController.ParticipantId;
      conditionsOrdering = DeviceController.ConditionsOrdering;
      participantIsRightHanded = DeviceController.ParticipantIsRightHanded;
      SendToServer();
    }

    protected virtual void DeviceController_ActivateTaskSync()
    {
      type = Type.ActivateTask;
      SendToServer();
    }

    protected virtual void DeviceController_ToogleZoomSync(bool zoomActivated)
    {
      type = Type.ToggleZoom;
      this.zoomActivated = zoomActivated;
      SendToServer();
    }
  }
}
