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
			Warnings = 2,   // Prints warnings and errors
			Errors = 3      // prints only errors
		}

		public static void Assert ( bool logic, Object message = null )
		{
			Debug.Assert( logic, message);
		}
		public static void SetLoggingLevel(QLogger.Level level)
		{
			m_logLevel = level;
		}

		public static bool CanLogWarning { get { return m_logLevel <= Level.Warnings || IsRunningInEditorMode; } }
		public static void LogWarning(string message, bool includeTimeStamp = false)
		{
			Log(Level.Warnings, message, includeTimeStamp);
		}

		//public static bool CanLogError { get { return m_logLevel <= Level.Errors || IsRunningInEditorMode; }}
		public static void LogError(string message, bool includeTimeStamp = false)
		{
			Log(Level.Errors, message, includeTimeStamp);
		}
		public static void LogErrorAndThrowException(string message, bool includeTimeStamp = false)
		{
			Log(Level.Errors, message, includeTimeStamp);
			throw new System.InvalidOperationException(message);
		}

		public static bool CanLogInfo { get { return m_logLevel <= Level.Info || IsRunningInEditorMode; } }
		public static void LogInfo(string message, bool includeTimeStamp = false)
		{
			Log(Level.Info, message, includeTimeStamp);
		}

		public static void Log(QLogger.Level level, string message, bool includeTimeStamp = false)
		{
			bool isEditorMode = IsRunningInEditorMode;
			if (includeTimeStamp)
				message = "[Time:" + Time.realtimeSinceStartup + "]" + message;

			if (level == QLogger.Level.Info)
			{
				if (m_logLevel <= level || isEditorMode)
					Debug.Log("[QLearningSpace:Info] " + message);
			}
			else if (level == QLogger.Level.Warnings)
			{
				if (m_logLevel <= level || isEditorMode)
					Debug.LogWarning("[QLearningSpace:Warn] " + message);
			}
			else if (level == QLogger.Level.Errors)
			{
				Debug.LogError("[QLearningSpace:Err] " + message);
			}
		}

		public static void SetDebugDrawLineDuration(float duration)
		{
			DrawLineDuration = duration;
		}

		public static float DrawLineDuration { get; private set; }

		private static bool IsRunningInEditorMode { get { return !Application.isPlaying; } }
		private static QLogger.Level m_logLevel = Level.Warnings;

	}
}