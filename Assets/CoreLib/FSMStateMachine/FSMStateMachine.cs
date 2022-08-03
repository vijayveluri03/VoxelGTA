using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
    public abstract class FSMState<T> where T : struct
    {
        public FSMState(FSMStateMachine<T> statemachine)
        {
            this.statemachine = statemachine;
        }
        public abstract void OnContext(System.Object context);
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void Update();

        protected FSMStateMachine<T> statemachine;
    }

    public class FSMBaseState<T> : FSMState<T> where T : struct
    {
        public FSMBaseState(FSMStateMachine<T> statemachine) : base(statemachine) { }
        public override void OnContext(System.Object context) { }
        public override void Update() { }
        public override void OnEnter() { }
        public override void OnExit() { }
    }

    public class FSMStateMachine<T> where T : struct
    {
        T? previousState = null;
        T? currentState = null;
        T? nextState = null;

        Dictionary<T, FSMState<T>> statesDictionary;
        List<MyKeyValuePair<T, T>> stateMapping = new List<MyKeyValuePair<T, T>>();
        System.Object contextForNextState = null;

        public FSMStateMachine()
        {
            statesDictionary = new Dictionary<T, FSMState<T>>();
        }
        ~FSMStateMachine()
        {
        }

        public void RegisterState(T key, FSMState<T> state)
        {
            if (!typeof(T).IsEnum)
            {
                Core.QLogger.LogErrorAndThrowException("TEnum must be an enum.");
            }

            if (statesDictionary.ContainsKey(key))
            {
                Core.QLogger.LogErrorAndThrowException("Already contains key " + key.ToString());
            }

            statesDictionary.Add(key, state);
        }
        public void AddMapping(T state1, T state2)
        {
            if (GetKeyValueMap(state1, state2) != null) Core.QLogger.LogErrorAndThrowException("We already have a mapping between " + state1.ToString() + state2.ToString());
            stateMapping.Add(new MyKeyValuePair<T, T>(state1, state2));
        }
        public MyKeyValuePair<T, T> GetKeyValueMap(T state1, T state2)
        {
            if (stateMapping == null) return null;
            foreach (MyKeyValuePair<T, T> map in stateMapping)
            {
                if (map.key.Equals(state1) && map.value.Equals(state2))
                    return map;
            }
            return null;
        }

        // public accessors.
        public T ActiveState
        {
            get { return currentState != null ? currentState.Value : default(T); }
        }

        public FSMState<T> GetState(T key)
        {
            FSMState<T> state;
            statesDictionary.TryGetValue(key, out state);
            return state;
        }

        public void PushNextState(T newState, System.Object context)
        {
            if (currentState.HasValue && currentState.Value.Equals(newState))
            {
                if (Core.QLogger.CanLogWarning) Core.QLogger.LogWarning(" Setting same state again " + newState.ToString());
                return;
            }

            if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo(string.Format("FSM:Queued \"{0}\" state ", newState));
            nextState = newState;
            contextForNextState = context;
        }

        public void Update()
        {
            if (nextState.HasValue)
            {
                SetNextState();
            }

            if (currentState.HasValue)
                statesDictionary[currentState.Value].Update();
        }

        void SetNextState()
        {
            if (currentState.HasValue)
            {
                if (GetKeyValueMap(currentState.Value, nextState.Value) == null)
                {
                    Core.QLogger.LogErrorAndThrowException("There is no mapping between " + currentState.Value.ToString() + " and " + nextState.Value.ToString());
                    nextState = null;
                    return;
                }
            }
            //set current to peviousState
            previousState = currentState;
            //exit previousState
            if (previousState.HasValue)
            {
                if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo(string.Format("FSM:On Exit called for \"{0}\" state ", previousState.Value));
                statesDictionary[previousState.Value].OnExit();
            }

            //set currentState to Next
            currentState = nextState;
            //enter current state
            if (currentState.HasValue)
            {
                if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo(string.Format("FSM:On Enter called for \"{0}\" state ", currentState.Value));
                statesDictionary[currentState.Value].OnEnter();

                statesDictionary[currentState.Value].OnContext(contextForNextState /* this can be null if there is no context */ );
            }

            //set nextState to Null
            nextState = null;
            contextForNextState = null;
        }
    }
}
