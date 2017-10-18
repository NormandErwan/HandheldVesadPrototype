using NormandErwan.MasterThesisExperiment.Utilities;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Conditions
{
    public enum ClassificationDistance
    {
        Near,
        Far
    }

    public enum Input
    {
        Touch,
        Direct,
        Indirect
    }

    public enum ReferenceFrame
    {
        Device,
        World
    }

    public enum Output
    {
        DeviceOnly,
        VESAD
    }

    public enum TextSize
    {
        Small = 12,
        Large = 24
    }

    public class ConditionsManager : MonoBehaviour
    {
        // Editor fields

        public float MinNearClassificationDistance;
        public float MaxNearClassificationDistance;
        public float MinFarClassificationDistance;
        public float MaxFarClassificationDistance;

        // Properties

        public static Range<float> ClassificationDistanceNearRange;
        public static Range<float> ClassificationDistanceFarRange;

        // Methods

        protected void Start()
        {
            ClassificationDistanceNearRange = new Range<float>(MinNearClassificationDistance, MaxNearClassificationDistance);
            ClassificationDistanceFarRange = new Range<float>(MinFarClassificationDistance, MaxFarClassificationDistance);
        }
    }
}
