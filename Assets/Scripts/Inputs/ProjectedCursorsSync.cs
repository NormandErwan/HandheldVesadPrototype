using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursorsSync : DevicesSync
  {
    // Editor fields

    [SerializeField]
    private CursorsInput cursorsInput;

    [SerializeField]
    private ProjectedCursor[] projectedCursors;

    // Properties

    public Dictionary<CursorType, ProjectedCursor> ProjectedCursors { get; protected set; }

    // Variables

    protected ProjectedCursorsSyncMessage projectedCursorsMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      ProjectedCursors = new Dictionary<CursorType, ProjectedCursor>(projectedCursors.Length);
      for (int i = 0; i < projectedCursors.Length; i++)
      {
        ProjectedCursors.Add(projectedCursors[i].Cursor.Type, projectedCursors[i]);
      }

      projectedCursorsMessage = new ProjectedCursorsSyncMessage();
      projectedCursorsMessage.cursors = new CursorType[ProjectedCursors.Count];
      projectedCursorsMessage.localPositions = new Vector3[ProjectedCursors.Count];

      MessageTypes.Add(projectedCursorsMessage.MessageType);

      cursorsInput.Updated += CursorsInput_Updated;
    }

    protected virtual void OnDestroy()
    {
      cursorsInput.Updated -= CursorsInput_Updated;
    }

    // DevicesSync methods

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return netMessage.ReadMessage<ProjectedCursorsSyncMessage>();
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      var projectedCursorsReceived = netMessage.ReadMessage<ProjectedCursorsSyncMessage>();
      if (projectedCursorsReceived.SenderConnectionId != projectedCursorsReceived.SenderConnectionId)
      {
        projectedCursorsReceived.Restore(ProjectedCursors);
        return projectedCursorsReceived;
      }
      return null;
    }

    // Methods

    protected virtual void CursorsInput_Updated()
    {
      foreach (var projectedCursor in ProjectedCursors)
      {
        projectedCursor.Value.UpdateProjection();
      }

      projectedCursorsMessage.Update(ProjectedCursors);
      if (projectedCursorsMessage.TransformChanged)
      {
        SendToServer(projectedCursorsMessage);
      }
    }
  }
}
