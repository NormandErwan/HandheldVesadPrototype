using NormandErwan.MasterThesisExperiment.States;
using System;
using UnityEditor;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Editor
{
    [CustomEditor(typeof(StatesManager))]
    [CanEditMultipleObjects]
    public class StatesManagerEditor : UnityEditor.Editor
    {
        private readonly int stateInstructionsMaxHeigth = 75;

        public override void OnInspectorGUI()
        {
            EditorStyles.textField.wordWrap = true;

            var statesManager = (StatesManager)target;

            // Initialize the arrays
            var states = Enum.GetValues(typeof(State));
            if (statesManager.stateTitles == null || statesManager.stateTitles.Length != states.Length)
            {
                statesManager.stateTitles = new string[states.Length];
                statesManager.stateInstructions = new string[states.Length];
            }

            // Draw the inspector GUI
            var stateNames = Enum.GetNames(typeof(State));
            var stateNameStyle = new GUIStyle() { fontStyle = FontStyle.Bold };
            for (int i = 0; i < states.Length; i++)
            {
                var state = (State)states.GetValue(i);

                EditorGUILayout.LabelField(stateNames[i], stateNameStyle);
                statesManager.stateTitles[i] = EditorGUILayout.TextField("Title", statesManager.stateTitles[i]);
                EditorGUILayout.LabelField("Instructions");
                statesManager.stateInstructions[i] = EditorGUILayout.TextArea(statesManager.stateInstructions[i], GUILayout.MaxHeight(stateInstructionsMaxHeigth));
                EditorGUILayout.Space();
            }
        }
    }
}
