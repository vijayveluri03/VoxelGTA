
// This is an advanced version of FSMStateMachine
// This has an independent owner in the controller. The Controller itself is not an owner. 
// Also has different types of Update loops and contexts.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
    // Template parameters - Owner - is not the FSMController. Owner can be some other object completely. 
    // T - is the ID( or state if you may) of the FSMCState 
    // You can get Controller ( statemachine ) , which is different from Owner

    public class FSMCState<OWNER, T> where T : struct
    {
        public void Init(FSMController<OWNER, T> controller)
        {
            this.controller = controller;
        }

        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void OnEnter(params object[] arguments) { }
        public virtual void OnExit() { }

        public virtual void Notify(params object[] arguments) { }

        public FSMController<OWNER, T> controller { get; internal set; }
        public OWNER Owner { get { return controller.Ownr; } }
        public T ID { get { return controller.ActiveState; } }

        // A couple of short-hand functions for setting the state
        public void SetState(T newStateID, params object[] paramList)
        {
            controller.SetState(newStateID, paramList);
        }
        public void SetState(T newStateID)
        {
            controller.SetState(newStateID);
        }
    }

    public class FSMController<OWNER, T>
            where T : struct
    {
        public OWNER Ownr { get { return owner; } }

        public FSMController(OWNER owner)
        {
            if (!typeof(T).IsEnum)
            {
                Core.QLogger.LogErrorAndThrowException("Error. TEnum must be an enum.");
                return;
            }
            this.owner = owner;

            statesDictionary = new Dictionary<T, FSMCState<OWNER, T>>();
        }
        ~FSMController()
        {
        }

        public void RegisterState(T key, FSMCState<OWNER, T> state)
        {
            if (statesDictionary.ContainsKey(key))
            {
                Core.QLogger.LogErrorAndThrowException("Already contains key " + key.ToString());
            }

            state.Init(this);
            statesDictionary.Add(key, state);
        }
        public void AddMapping(T fromState, params T[] toState)
        {
            foreach (T NextState in toState)
            {
                if (GetKeyValueMap(fromState, NextState) != null)
                    Core.QLogger.LogErrorAndThrowException("We already have a mapping between " + fromState.ToString() + NextState.ToString());

                stateMapping.Add(new MyKeyValuePair<T, T>(fromState, NextState));
            }
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

        public FSMCState<OWNER, T> GetState(T key)
        {
            FSMCState<OWNER, T> state;
            statesDictionary.TryGetValue(key, out state);
            return state;
        }

        public void SetState(T newState, params System.Object[] args)
        {
            if (currentState.HasValue && currentState.Value.Equals(newState))
            {
                if (Core.QLogger.CanLogWarning) Core.QLogger.LogWarning(" Setting same state again " + newState.ToString());
                return;
            }
            if (nextState.HasValue && !nextState.Value.Equals(newState))
            {
                Core.QLogger.LogError(" We are swapping 2 states in 1 phrame (" + nextState.Value + "," + newState + "). This can cause unexpected behavior");
            }

            if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo(string.Format("FSM:Queued \"{0}\" state ", newState));
            nextState = newState;
            contextForNextState = args;
        }

        public void Notify(params object[] arguments)
        {
            if (currentState.HasValue)
                statesDictionary[currentState.Value].Notify(arguments);
        }

        public void FixedUpdate()
        {
            if (currentState.HasValue)
                statesDictionary[currentState.Value].FixedUpdate();
        }

        public void Update()
        {
            if (nextState.HasValue)
            {
                SetNextState_Internal();
            }

            if (currentState.HasValue)
                statesDictionary[currentState.Value].Update();
        }

        public void LateUpdate()
        {
            if (currentState.HasValue)
                statesDictionary[currentState.Value].LateUpdate();
        }

        void SetNextState_Internal()
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

            //set nextState to Null
            nextState = null;
            contextForNextState = null;

            //enter current state
            if (currentState.HasValue)
            {
                if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo(string.Format("FSM:On Enter called for \"{0}\" state ", currentState.Value));
                statesDictionary[currentState.Value].OnEnter(contextForNextState);
            }

            if (logToGUI)
                Core.QLogger.LogToGUI(logToGUIIndex, currentState.HasValue ? currentState.Value.ToString() : " null ");
        }
        public void SetLogToGUI(bool set, int index)
        {
            logToGUI = set;
            logToGUIIndex = index;
        }

        T? previousState = null;
        T? currentState = null;
        T? nextState = null;

        Dictionary<T, FSMCState<OWNER, T>> statesDictionary;
        List<MyKeyValuePair<T, T>> stateMapping = new List<MyKeyValuePair<T, T>>();
        System.Object[] contextForNextState = null;
        private OWNER owner;
        private bool logToGUI = false;
        private int logToGUIIndex = -1;
    }
}
