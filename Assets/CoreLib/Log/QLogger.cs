using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core
{
	public static class QLogger
	{

		public enum Level
		{
			Info = 1,    // Prints everything 
			Warning = 2,   // Prints warnings and errors
			Error = 3      // prints only errors
		}

		public static void Assert ( bool logic, System.Object message = null )
		{
			Debug.Assert( logic, message != null ? message : "Asset was hit");
		}
		public static void SetLoggingLevel(Core.QLogger.Level level)
		{
			m_logLevel = level;
		}

		public static void LogWarning(string message, bool includeTimeStamp = false)
		{
            Log(Level.Warning, message, includeTimeStamp);
        }

        //public static bool CanLogError { get { return m_logLevel <= Level.Errors || IsRunningInEditorMode; }}
        public static void LogError(string message, bool includeTimeStamp = false)
		{
			Log(Level.Error, message, includeTimeStamp);
		}
		public static void LogErrorAndThrowException(string message, bool includeTimeStamp = false)
		{
			Log(Level.Error, message, includeTimeStamp);
			throw new System.InvalidOperationException(message);
		}

		public static void LogInfo(string message, bool includeTimeStamp = false)
		{
  			Log(Level.Info, message, includeTimeStamp);
		}
		public static void LogToGUI ( int index, string message )
		{
			if ( GUILogger == null )
			{
				GUILogger = (new GameObject("QGUILogger")).AddComponent<QGUILogger>();

                if(hideGUILoggerOnStart)
                    GUILogger.InvertVisibility();
			}
			GUILogger.SetLog ( index, message );
		}
		public static void ToggleGUIVisibility ()
		{
			if ( GUILogger != null )
			{
				GUILogger.InvertVisibility();
			}
		}
		public static void Log(Core.QLogger.Level level, string message, bool includeTimeStamp = false)
		{
            if (m_logLevel > level)
                return;

			if (includeTimeStamp)
				message = "[Time:" + Time.realtimeSinceStartup + "]" + message;

			if (level == Core.QLogger.Level.Info)
			{
					Debug.Log("[QLearningSpace:Info] " + message);
			}
			else if (level == Core.QLogger.Level.Warning)
			{
					Debug.LogWarning("[QLearningSpace:Warn] " + message);
			}
			else if (level == Core.QLogger.Level.Error)
			{
				Debug.LogError("[QLearningSpace:Err] " + message);
			}
		}

		public static void SetDebugDrawLineDuration(float duration)
		{
			DrawLineDuration = duration;
		}

		public static float DrawLineDuration { get; private set; }

		private static Core.QLogger.Level m_logLevel = Level.Warning;
		private static QGUILogger GUILogger = null;
        private static bool hideGUILoggerOnStart = true;

	}
}