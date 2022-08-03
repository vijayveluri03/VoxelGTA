using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Core
{
	public class QGUILogger : MonoBehaviour
	{
		public void Awake()
		{
			DontDestroyOnLoad(gameObject);
			log = new string[maxTracks];
			for( int i = 0; i < maxTracks; i++ )
			{
				log[i] = "";
			}
		}

		public void InvertVisibility ()
		{
			visible = !visible;
		}
		public void SetLog ( int track, string log )
		{
			Core.QLogger.Assert ( track >= 0 && track < maxTracks);
			this.log[track] = log;

			strLog.Length = 0; 	//clearing

			for (int i = 0; i < maxTracks; i++)
			{
				if (!string.IsNullOrEmpty(this.log[i]))
					strLog.Append( string.Format("[{0}] -> {1}\n", i, this.log[i]) );
			}
		}

		void OnGUI () 
		{
			if ( !visible )
				return;

			if ( log != null )
			{
				{
						GUI.TextArea (new Rect (Screen.width/2, 10, Screen.width/2, Screen.height/4),
							strLog.ToString() );
				}
			}
		}

		string[] log;
		StringBuilder strLog = new StringBuilder();
		const int maxTracks = 20;
		bool visible = true;
	}
}