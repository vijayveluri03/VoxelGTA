
using System.Collections;

namespace Core.old 
{
	public class StateMachine<OwnerType>
	{
		// StateMachine that contains list of states of System.Types,
		// so they can be constructed automatically from whatever ID
		
		// It's a shame that C# doesn't let you use Enums as generic types
		// (Well you can sorta, but it involves boxing/unboxing with IConvertible, and we don't want to litter the Garbage Collector)

		#region public sub-types
		public class State
		{		
			public virtual void Begin( int previous ) {}
			public virtual void Begin( int previous, params object[] list ) {}
			public virtual void Update() {}
			public virtual void End( int next ) {}		
			public virtual void Notify( int messageID, params object[] messageParams ) {}
			
			public StateMachine<OwnerType> StateMachine	{ get; internal set; }
			public OwnerType Owner						{ get { return StateMachine.m_owner; } }
			public int ID								{ get { return StateMachine.m_currentID; } }
			
			// A couple of short-hand functions for setting the state
			public void SetState( int newStateID, params object[] paramList )
			{
				StateMachine.SetState( newStateID, paramList );
			}
			public void SetState( int newStateID )
			{
				StateMachine.SetState( newStateID );
			}
		}
		#endregion
		
		#region public interface	
		public StateMachine( OwnerType owner, int numStates )
		{
			m_owner = owner;
			m_stateTypes = new System.Type[ numStates ];
		}
		
		public OwnerType Owner 		{ get { return m_owner; } }
		public int PrevStateID		{ get { return m_prevID; } }
		public int CurrentStateID	{ get { return m_currentID; } }
		public State CurrentState	{ get { return m_current; } }
		
		public void SetState( int newStateID ) { SetState( newStateID, null ); }
		public virtual void SetState( int newStateID, params object[] paramList )
		{
			// if( s_logAllStateChanges )
			// {
			//     Core.QLogger.LogInfo( "StateChange: "+typeof(OwnerType)+"  prev:"+m_currentID+"  new:"+newStateID );
			//     L.LogCallStack( true );
			// }

			// Defer the state change if we're in Update
			if( m_isInUpdate )
			{
				m_nextID = newStateID;
				m_next_params = paramList;
				return;
			}
			
			int prevID = m_currentID;
			SetStateInternal( newStateID );
			
			if( paramList != null )
			{
				m_current.Begin( prevID, paramList );
			}
			else
			{
				m_current.Begin( prevID );
			}
		}

		// Stop the current state, go back to "-1"
		public void Stop()
		{
			SetStateInternal( -1 );
		}
		
		public void Notify( int messageID, params object[] messageParams )
		{
			if (m_current != null)
			{
				m_current.Notify( messageID, messageParams );
			}
		}
		
		public void Update()
		{
			m_isInUpdate = true;
			m_current.Update();
			m_isInUpdate = false;
			
			// Handle if a state change was triggered inside of Update
			if( m_nextID != -1 )
			{
				int prevID = m_currentID;
				SetStateInternal( m_nextID );
			
				if( m_next_params != null )
				{
					m_current.Begin( prevID, m_next_params );
					m_next_params = null;
				}
				else
				{
					m_current.Begin( prevID );
				}
				m_nextID = -1;
			}
		}
		
		public void RegisterState<StateType>( int ID ) where StateType : State
		{
			m_stateTypes[ ID ] = typeof(StateType);
		}
		
		public void RegisterState( int ID, System.Type type )
		{
			if( !type.IsSubclassOf( typeof(State) ) )	
			{
				Core.QLogger.LogError( "RegisterState only accepts State types!");
			}
			m_stateTypes[ ID ] = type;
		}
		#endregion
		
		void SetStateInternal( int newStateID )
		{
			if( m_current != null )
			{
				m_current.End( newStateID );
			}
			
			if( newStateID >= 0 )
			{
				// Create instance of state, from System.Type
				// Assume that object is of type State
				m_current = (State)System.Activator.CreateInstance( m_stateTypes[ newStateID ] );
				m_current.StateMachine = this;
			}
			else
			{
				m_current = null;
			}

			m_prevID = m_currentID;
			m_currentID = newStateID;
		}
		
		#region privates
		internal OwnerType m_owner;
		internal int m_prevID = -1;
		internal int m_currentID = -1;
		State m_current;
		
		System.Type[] m_stateTypes;
		
		bool m_isInUpdate;
		int m_nextID = -1;
		object[] m_next_params;
		#endregion
	}
}