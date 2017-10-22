using System.Collections.Generic;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment
{
    /// <summary>
    /// Container class for networking messages types used in this experiment.
    /// </summary>
    public class MessageType : DevicesSyncUnity.Messages.MessageType
    {
        // Constants

        /// <summary>
        /// See <see cref="DevicesSyncUnity.Messages.MessageType.Smallest"/>.
        /// </summary>
        public static new short Smallest { get { return smallest; } set { smallest = value; } }

        /// <summary>
        /// Networking message for communicating <see cref="States.StateManagerSyncMessage"/>
        /// </summary>
        public static short StateManager { get { return (short)(Smallest + 1); } }

        /// <summary>
        /// Networking message for communicating <see cref="Variables.IndependentVariablesSyncMessage"/>
        /// </summary>
        public static short IndependentVariableManagers { get { return (short)(Smallest + 2); } }

        /// <summary>
        /// See <see cref="DevicesSyncUnity.Messages.MessageType.Highest"/>.
        /// </summary>
        public static new short Highest { get { return IndependentVariableManagers; } }

        // Variables

        private static short smallest = (short)(DevicesSyncUnity.Examples.Messages.MessageType.Highest + 1);
        private static Dictionary<short, string> messageTypeStrings = new Dictionary<short, string>()
        {
            { StateManager, "State" },
            { IndependentVariableManagers, "IndependentVariableManagers" }
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
