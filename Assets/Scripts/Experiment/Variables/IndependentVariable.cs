using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.Experiment.Variables
{
    public abstract class IndependentVariable<T> : IIndependentVariable where T : IndependentVariableCondition
    {
        // Editor Fields

        [SerializeField]
        private T[] conditions;

        // Properties

        public SortedList<string, T> Conditions { get; protected set; }
        public T CurrentCondition { get; protected set; }

        // Events

        public event Action<T> CurrentConditionUpdated = delegate { };

        // Methods

        public override void NextCondition()
        {
            int requestConditionIndex = (CurrentConditionIndex + 1) % Conditions.Count;
            OnRequestCurrentConditionSync(id, Conditions.Values[requestConditionIndex].id);
        }

        internal override void SetCurrentCondition(string currentConditionId)
        {
            CurrentCondition = Conditions[currentConditionId];
            CurrentConditionId = CurrentCondition.id;
            CurrentConditionIndex = Conditions.IndexOfKey(CurrentCondition.id);
            CurrentConditionUpdated.Invoke(CurrentCondition);
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
