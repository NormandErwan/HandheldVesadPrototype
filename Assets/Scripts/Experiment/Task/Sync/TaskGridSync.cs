using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSync : DevicesSyncInterval
  {
    // Editor fields

    [SerializeField]
    private TaskGrid taskGrid;

    // Properties
    
    protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }
    public TaskGrid TaskGrid { get { return taskGrid; } set { taskGrid = value; } }

    // Variables

    protected TaskGridSyncConfigureMessage gridConfigureMessage;
    protected TaskGridSyncEventsMessage gridEventsMessage;
    protected TaskGridSyncDragMessage gridDragMessage;
    protected TaskGridSyncZoomMessage gridZoomMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      gridConfigureMessage = new TaskGridSyncConfigureMessage(TaskGrid, () =>
      {
        SendToServer(gridConfigureMessage);
      });
      gridEventsMessage = new TaskGridSyncEventsMessage(TaskGrid, () =>
      {
        SendToServer(gridEventsMessage);
      });
      gridDragMessage = new TaskGridSyncDragMessage(TaskGrid);
      gridZoomMessage = new TaskGridSyncZoomMessage(TaskGrid);

      MessageTypes.Add(gridConfigureMessage.MessageType);
      MessageTypes.Add(gridEventsMessage.MessageType);
      MessageTypes.Add(gridDragMessage.MessageType);
      MessageTypes.Add(gridZoomMessage.MessageType);
    }

    // DevicesSync methods

    protected override void OnSendToServerIntervalIteration(bool shouldSendThisFrame)
    {
      if (shouldSendThisFrame)
      {
        if (gridDragMessage.ReadyToSend)
        {
          SendToServer(gridDragMessage);
          gridDragMessage.Sent();
        }

        if (gridZoomMessage.ReadyToSend)
        {
          SendToServer(gridZoomMessage);
          gridZoomMessage.Sent();
        }
      }
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, false);
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, true);
    }

    // Methods

    protected DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
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
          if (gridEventsReceived.SenderConnectionId != ConnectionId)
          {
            gridEventsReceived.SyncGrid(TaskGrid);
          }
          return gridEventsReceived;
        }

        TaskGridSyncDragMessage gridDragReceived;
        if (TryReadMessage(netMessage, gridDragMessage.MessageType, out gridDragReceived))
        {
          if (gridDragReceived.SenderConnectionId != ConnectionId)
          {
            gridDragReceived.SyncGrid(TaskGrid);
          }
          return gridDragReceived;
        }

        TaskGridSyncZoomMessage gridZoomReceived;
        if (TryReadMessage(netMessage, gridZoomMessage.MessageType, out gridZoomReceived))
        {
          if (gridZoomReceived.SenderConnectionId != ConnectionId)
          {
            gridZoomReceived.SyncGrid(TaskGrid);
          }
          return gridZoomReceived;
        }
      }

      return null;
    }
  }
}
