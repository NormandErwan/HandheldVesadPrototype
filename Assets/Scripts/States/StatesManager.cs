using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.States
{
    public class StatesManager : MonoBehaviour
    {
        // Editor fields managed by StatesManagerEditor

        public string[] stateTitles;
        public string[] stateInstructions;

        // Properties

        public static Dictionary<State, string> StateTitles { get; protected set; }
        public static Dictionary<State, string> StateInstructions { get; protected set; }
        
        // Methods

        protected void Awake()
        {
            var states = Enum.GetValues(typeof(State));

            StateTitles = new Dictionary<State, string>(states.Length);
            StateInstructions = new Dictionary<State, string>(states.Length);

            for (int i = 0; i < states.Length; i++)
            {
                var state = (State)states.GetValue(i);
                StateTitles[state] = stateTitles[i];
                StateInstructions[state] = stateInstructions[i];
            }
        }
    }
}
