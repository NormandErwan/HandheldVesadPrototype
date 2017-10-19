using NormandErwan.MasterThesisExperiment.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.IndependentVariables
{
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

        public static Dictionary<ClassificationDistance, Range<float>> ClassificationDistanceRanges { get; protected set; }
        public static Dictionary<TextSize, int> TextSizes { get; protected set; }

        // Methods

        protected void Start()
        {
            ClassificationDistanceRanges = new Dictionary<ClassificationDistance, Range<float>>()
            {
                { ClassificationDistance.Near, new Range<float>(MinNearClassificationDistance, MaxNearClassificationDistance) },
                { ClassificationDistance.Far, new Range<float>(MinFarClassificationDistance, MaxFarClassificationDistance) }
            };

            TextSizes = new Dictionary<TextSize, int>()
            {
                { TextSize.Small, SmallTextSize },
                { TextSize.Large, LargeTextSize }
            };
        }
    }
}
