using UnityEngine;
using System.Collections;
using System.Reflection;
using System;


namespace Core
{

    public class Singleton<T> where T : class, new()
    {
        // Prevent the class from behind created by the client
        public Singleton()
        {
            if (mUniqueInstance != null)
            {
                Core.QLogger.LogErrorAndThrowException( "Singleton [" + typeof(T) + "] cannot be manually instantiated and Singleton.Instance can only be called from the main thread.");
            }
        }

        static public bool Exists()
        {
            return mUniqueInstance != null;
        }

        static T mUniqueInstance = null;
        public static T Instance
        {
            get
            {
                if (mUniqueInstance == null)
                {
                    mUniqueInstance = new T();
                    if ( Core.QLogger.CanLogInfo ) Core.QLogger.LogInfo (string.Format("{0} instantiated.", typeof(T)));
                }
                return mUniqueInstance;
            }
        }

        public virtual void ClearInstance()
        {
            mUniqueInstance = null;
        }

        public static T Get()
        {
            return Instance;
        }
    }

    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        // Prevent the class from behind created by the client
        protected virtual void Awake()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = this;
                if (!DestroyOnSceneSwitch && transform.parent == null)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }

            if (uniqueInstance != this)
            {
                Core.QLogger.LogErrorAndThrowException("Cannot have two instances of a SingletonMonoBehaviour "+ typeof(T).ToString() + "." );
            }

        }

        protected virtual void OnDestroy()
        {
            if (uniqueInstance == this)
            {
                uniqueInstance = null;
            }
        }

        protected virtual bool DestroyOnSceneSwitch
        {
            get { return false; }
        }

        static public bool Exists()
        {
            return uniqueInstance != null;
        }

        static private SingletonMonoBehaviour<T> uniqueInstance;
        public static T Instance
        {
            get
            {
                if (uniqueInstance == null)        // thread-safe
                {
                    Core.QLogger.LogErrorAndThrowException( "ERROR: Singleton [" + typeof(T) + "] accessed before construction.  Note that this can happen if you use OnEnable as that runs at construction of your other object, possibly before this object.");
                }
                return (T)uniqueInstance;
            }
        }
    }

    public class SingletonSpawningMonoBehaviour<T> : MonoBehaviour where T : SingletonSpawningMonoBehaviour<T>
    {
        // Prevent the class from behind created by the client
        protected virtual void Awake()
        {

            if ( Core.QLogger.CanLogInfo ) Core.QLogger.LogInfo ( string.Format("{0} instantiated.", typeof(T)));

            if (uniqueInstance == null)
            {
                uniqueInstance = this;
            }
            else if (uniqueInstance != this)
            {
                Core.QLogger.LogErrorAndThrowException( "Cannot have two instances of a SingletonMonoBehaviour : " + typeof(T).ToString() + "." );
            }
        }

        protected virtual void OnDestroy()
        {
            if (uniqueInstance == this)
            {
                uniqueInstance = null;
            }
        }

        static public bool Exists()
        {
            return uniqueInstance != null;
        }

        static private SingletonSpawningMonoBehaviour<T> uniqueInstance;
        public static T Instance
        {
            get
            {
                if (uniqueInstance == null)
                {
                    if (!Application.isPlaying)
                        return null;

                    // Old example code to construct an object from nothing
                    GameObject go = new GameObject("Singleton " + typeof(T).ToString(), typeof(T));
                    uniqueInstance = (SingletonSpawningMonoBehaviour<T>)go.GetComponent<T>();
                }
                return (T)uniqueInstance;
            }
        }

        // Get(), so you can do stuff like MySingletonType.Get(); to make sure it's been created..
        public static T Get()
        {
            return Instance;
        }
    }

}