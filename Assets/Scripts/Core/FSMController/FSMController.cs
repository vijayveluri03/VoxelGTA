using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
	namespace FSMController
	{
		
		public class FSMController<Owner, T>
				where T : struct
		{
			public class FSMState
			{
				public void Init ( FSMController<Owner, T> controller ) 
				{ 
					this.controller = controller; 
				}
				
				public virtual void FixedUpdate() { }
				public virtual void Update() { }
				public virtual void LateUpdate() { }

				public virtual void OnEnter( params object[] arguments ) { }
				public virtual void OnExit() { }

				public virtual void Notify( params object[] arguments ) { }

				public FSMController<Owner, T> controller { get; internal set; }
				public Owner Owner { get { return controller.Ownr; } }
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

			public Owner Ownr { get { return owner;} }

			public FSMController( Owner owner)
			{
				if (!typeof(T).IsEnum)
				{
					QLogger.LogErrorAndThrowException("Error. TEnum must be an enum.");
					return;
				}
				this.owner = owner;

				statesDictionary = new Dictionary<T, FSMState>();
			}
			~FSMController()
			{
			}

			public void RegisterState(T key, FSMState state)
			{
				if (statesDictionary.ContainsKey(key))
				{
					QLogger.LogErrorAndThrowException("Already contains key " + key.ToString());
				}

				state.Init( this );
				statesDictionary.Add(key, state);
			}
			public void AddMapping(T state1, T state2)
			{
				if (GetKeyValueMap(state1, state2) != null) 
					QLogger.LogErrorAndThrowException("We already have a mapping between " + state1.ToString() + state2.ToString());

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
			public T ActiveState {
				get { return currentState != null ? currentState.Value : default(T); }
			}

			public FSMState GetState(T key)
			{
				FSMState state;
				statesDictionary.TryGetValue(key, out state);
				return state;
			}

			public void SetState(T newState, params System.Object[] args)
			{
				if (currentState.HasValue && currentState.Value.Equals(newState))
				{
					if (QLogger.CanLogWarning) QLogger.LogWarning(" Setting same state again " + newState.ToString());
					return;
				}

				if (QLogger.CanLogInfo) QLogger.LogInfo(string.Format("FSM:Queued \"{0}\" state ", newState));
				nextState = newState;
				contextForNextState = args;
			}

			public void Notify( params object[] arguments ) 
			{ 
				if (currentState.HasValue)
					statesDictionary[currentState.Value].Notify ( arguments );
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
						QLogger.LogErrorAndThrowException("There is no mapping between " + currentState.Value.ToString() + " and " + nextState.Value.ToString());
						nextState = null;
						return;
					}
				}
				//set current to peviousState
				previousState = currentState;
				//exit previousState
				if (previousState.HasValue)
				{
					if (QLogger.CanLogInfo) QLogger.LogInfo(string.Format("FSM:On Exit called for \"{0}\" state ", previousState.Value));
					statesDictionary[previousState.Value].OnExit();
				}

				//set currentState to Next
				currentState = nextState;
				//enter current state
				if (currentState.HasValue)
				{
					if (QLogger.CanLogInfo) QLogger.LogInfo(string.Format("FSM:On Enter called for \"{0}\" state ", currentState.Value));
					statesDictionary[currentState.Value].OnEnter(contextForNextState);
				}

				//set nextState to Null
				nextState = null;
				contextForNextState = null;
			}

			T? previousState = null;
			T? currentState = null;
			T? nextState = null;

			Dictionary<T, FSMState> statesDictionary;
			List<MyKeyValuePair<T, T>> stateMapping = new List<MyKeyValuePair<T, T>>();
			System.Object[] contextForNextState = null;
			private Owner owner;
		}
	}
}
