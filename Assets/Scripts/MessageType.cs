using System.Collections.Generic;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment
{
  /// <summary>
  /// Container class for networking messages types used in this experiment.
  /// </summary>
  // TODO: rename/refacto
  public class MessageType : DevicesSyncUnity.Messages.MessageType
  {
    // Constants

    /// <summary>
    /// See <see cref="DevicesSyncUnity.Messages.MessageType.Smallest"/>.
    /// </summary>
    public static new short Smallest { get { return smallest; } set { smallest = value; } }

    /// <summary>
    /// Networking message for communicating <see cref="States.StateManagerMessage"/>
    /// </summary>
    public static short StateManager { get { return (short)(Smallest + 1); } }

    /// <summary>
    /// Networking message for communicating <see cref="Variables.IndependentVariablesMessage"/>
    /// </summary>
    public static short IndependentVariableManagers { get { return (short)(Smallest + 2); } }

    /// <summary>
    /// Networking message for communicating <see cref="DeviceControllers.DeviceControllerMessage"/>
    /// </summary>
    public static short DeviceController { get { return (short)(Smallest + 3); } }

    /// <summary>
    /// See <see cref="DevicesSyncUnity.Messages.MessageType.Highest"/>.
    /// </summary>
    public static new short Highest { get { return DeviceController; } }

    // Variables

    private static short smallest = (short)(DevicesSyncUnity.Messages.MessageType.Highest + 1);
    private static Dictionary<short, string> messageTypeStrings = new Dictionary<short, string>()
    {
      { StateManager, "State" },
      { IndependentVariableManagers, "IndependentVariableManagers" },
      { DeviceController, "DeviceController" }
    };

    // Methods

    /// <summary>
    /// See <see cref="MsgType.MsgTypeToString(short)"/>.
    /// </summary>
    public static new string MsgTypeToString(short value)
    {
      string messageTypeString;
      if (messageTypeStrings.TryGetValue(value, out messageTypeString))
      {
        return messageTypeString;
      }
      return MsgType.MsgTypeToString(value);
    }
  }
}
