using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSync : DevicesSync
  {
    // Editor fields

    [SerializeField]
    private TaskGrid taskGrid;

    // Properties
    
    public TaskGrid TaskGrid { get { return taskGrid; } set { taskGrid = value; } }

    // Variables

    protected TaskGridSyncConfigureMessage gridConfigureMessage;
    protected TaskGridSyncEventsMessage gridEventsMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      gridConfigureMessage = new TaskGridSyncConfigureMessage(TaskGrid, () =>
      {
        SendToServer(gridConfigureMessage, Channels.DefaultReliable);
      });
      gridEventsMessage = new TaskGridSyncEventsMessage(TaskGrid, () =>
      {
        SendToServer(gridEventsMessage, Channels.DefaultReliable);
      });

      MessageTypes.Add(gridConfigureMessage.MessageType);
      MessageTypes.Add(gridEventsMessage.MessageType);
    }

    // DevicesSync methods

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, false);
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, true);
    }

    // Methods

    protected virtual DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
    {
      if (!onClient || (onClient && !isServer))
      {
        TaskGridSyncConfigureMessage gridConfigureReceived;
        if (TryReadMessage(netMessage, gridConfigureMessage.MessageType, out gridConfigureReceived))
        {
          gridConfigureReceived.ConfigureGrid(TaskGrid);
          return gridConfigureReceived;
        }

        TaskGridSyncEventsMessage gridEventsReceived;
        if (TryReadMessage(netMessage, gridEventsMessage.MessageType, out gridEventsReceived))
        {
          if (gridEventsReceived.SenderConnectionId != gridEventsMessage.SenderConnectionId)
          {
            gridEventsReceived.SyncGrid(TaskGrid);
          }
          return gridEventsReceived;
        }
      }
      return null;
    }
  }
}
