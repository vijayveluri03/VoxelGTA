using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
	namespace FSMStateMachine
	{

		public abstract class FSMState
		{
			public abstract void OnContext(System.Object context);
			public abstract void OnEnter();
			public abstract void OnExit();
			public abstract void Update();
		}

		public class FSMBaseState : FSMState
		{
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

			Dictionary<T, FSMState> statesDictionary;
			List<MyKeyValuePair<T, T>> stateMapping = new List<MyKeyValuePair<T, T>>();
			System.Object contextForNextState = null;

			public FSMStateMachine()
			{
				statesDictionary = new Dictionary<T, FSMState>();
			}
			~FSMStateMachine()
			{
			}

			public void RegisterState(T key, FSMState state)
			{
				if (!typeof(T).IsEnum)
				{
					QLogger.LogErrorAndThrowException("TEnum must be an enum.");
				}

				if (statesDictionary.ContainsKey(key))
				{
					QLogger.LogErrorAndThrowException("Already contains key " + key.ToString());
				}

				statesDictionary.Add(key, state);
			}
			public void AddMapping(T state1, T state2)
			{
				if (GetKeyValueMap(state1, state2) != null) QLogger.LogErrorAndThrowException("We already have a mapping between " + state1.ToString() + state2.ToString());
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

			public void QueueState(T newState, System.Object context)
			{
				if (currentState.HasValue && currentState.Value.Equals(newState))
				{
					if (QLogger.CanLogWarning) QLogger.LogWarning(" Setting same state again " + newState.ToString());
					return;
				}

				if (QLogger.CanLogInfo) QLogger.LogInfo(string.Format("FSM:Queued \"{0}\" state ", newState));
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
					statesDictionary[currentState.Value].OnEnter();

					statesDictionary[currentState.Value].OnContext(contextForNextState /* this can be null if there is no context */ );
				}

				//set nextState to Null
				nextState = null;
				contextForNextState = null;
			}
		}
	}
}
