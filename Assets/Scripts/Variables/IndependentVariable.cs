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

        // Methods

        public override void NextCondition()
        {
            int requestConditionIndex = (CurrentConditionIndex + 1) % Conditions.Count;
            RequestCurrentConditionSync(id, Conditions.Values[requestConditionIndex].id);
        }

        internal override void SetCurrentCondition(string currentConditionId)
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
            CurrentConditionIndex = 0;
            CurrentConditionId = Conditions.Values[CurrentConditionIndex].id;
            SetCurrentCondition(CurrentConditionId);
        }
    }
}
