using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using System;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class DeviceControllerSyncMessage : DevicesSyncMessage
  {
    public enum Type
    {
      Configure,
      ActivateTask,
      ToggleMode
    }

    // Constructors and destructor

    public DeviceControllerSyncMessage(DeviceController deviceController, Action sendToServer)
    {
      DeviceController = deviceController;
      SendToServer = sendToServer;

      DeviceController.ConfigureSync += DeviceController_ConfigureSync;
      DeviceController.ActivateTaskSync += DeviceController_ActivateTaskSync;
      DeviceController.SetTaskGridModeSync += DeviceController_SetTaskGridModeSync;
    }

    public DeviceControllerSyncMessage()
    {
    }

    ~DeviceControllerSyncMessage()
    {
      DeviceController.ConfigureSync -= DeviceController_ConfigureSync;
      DeviceController.ActivateTaskSync -= DeviceController_ActivateTaskSync;
      DeviceController.SetTaskGridModeSync -= DeviceController_SetTaskGridModeSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.DeviceController; } }

    protected DeviceController DeviceController { get; }
    protected Action SendToServer { get; }

    // Variables

    public int senderConnectionId;

    public Type type;

    public TaskGrid.InteractionMode interactionMode;

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
      else if (type == Type.ToggleMode)
      {
        deviceController.TaskGrid.SetMode(interactionMode);
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

    protected virtual void DeviceController_SetTaskGridModeSync(TaskGrid.InteractionMode interactionMode)
    {
      type = Type.ToggleMode;
      this.interactionMode = interactionMode;
      SendToServer();
    }
  }
}
