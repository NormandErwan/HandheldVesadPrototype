using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Variables
{
    public abstract class IndependentVariable<T> : IIndependentVariable where T : IndependentVariableCondition
    {
        // Editor Fields

        [SerializeField]
        private T[] conditions;

        // Properties

        public SortedList<string, T> Conditions;
        public T CurrentCondition { get; protected set; }

        // Events

        public Action<T> CurrentConditionUpdated = delegate { };

        // Variables

        private int currentConditionIndex;

        // Methods

        void Start()
        {
            // TODO: remove
            CurrentConditionUpdated += (currentcondtiion) =>
            {
                print(typeof(T) + " updated : " + CurrentCondition.title);
            };
        }

        public override void NextCondition()
        {
            int requestConditionIndex = (CurrentConditionIndex + 1) % Conditions.Count;
            RequestCurrentConditionSync(id, Conditions.Values[requestConditionIndex].id);
        }

        internal override void SetCondition(string currentConditionId)
        {
            CurrentCondition = Conditions[currentConditionId];
            CurrentConditionIndex = Conditions.IndexOfKey(CurrentCondition.id);
            CurrentConditionId = currentConditionId;
            CurrentConditionUpdated(CurrentCondition);
        }

        protected virtual void Awake()
        {
            Conditions = new SortedList<string, T>();
            foreach (var condition in conditions)
            {
                Conditions.Add(condition.id, condition);
            }
            ConditionsCount = Conditions.Count;
        }
    }
}
