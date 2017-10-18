using NormandErwan.MasterThesisExperiment.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.IndependentVariables
{
    public enum ClassificationDistance
    {
        Near,
        Far
    }

    public enum Technique
    {
        MobileDeviceOnly,
        Peephole,
        MobileDeviceInput_VESADOutput,
        DirectInput_VESADOuput,
        IndirectInput_VESADOutput
    }

    public enum TextSize
    {
        Small,
        Large
    }

    public class IndependentVariablesManager : MonoBehaviour
    {
        // Editor fields

        [Header("Classification Distance Ranges")]
        public float MinNearClassificationDistance;
        public float MaxNearClassificationDistance;
        public float MinFarClassificationDistance;
        public float MaxFarClassificationDistance;

        [Header("Text Sizes")]
        public int SmallTextSize;
        public int LargeTextSize;

        // Properties

        public static Range<float> ClassificationDistanceNearRange { get; protected set; }
        public static Range<float> ClassificationDistanceFarRange { get; protected set; }

        public static Dictionary<TextSize, int> TextSizes { get; protected set; }

        // Methods

        protected void Start()
        {
            ClassificationDistanceNearRange = new Range<float>(MinNearClassificationDistance, MaxNearClassificationDistance);
            ClassificationDistanceFarRange = new Range<float>(MinFarClassificationDistance, MaxFarClassificationDistance);
            TextSizes = new Dictionary<TextSize, int>()
            {
                { TextSize.Small, SmallTextSize },
                { TextSize.Large, LargeTextSize }
            };
        }
    }
}
