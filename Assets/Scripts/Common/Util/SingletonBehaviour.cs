using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Normal singleton base class
public abstract class Singleton<T> where T : class, new()
{

    /// <summary>
    /// The m_ instance.
    /// </summary>
    protected static T _Instance = null;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = new T();
            }
            return _Instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XHEngine.Singleton`1"/> class.
    /// </summary>
    protected Singleton()
    {
        if (null != _Instance)
            throw new SingletonException("This " + (typeof(T)).ToString() + " Singleton Instance is not null !!!");
        Init();
    }


    /// <summary></summary>
    /// Init this Singleton.
    /// </summary>
    public virtual void Init() { }
}

// Singleton base class for MonoBehaviours
public abstract class SingletonBehaviour<T> : MonoBehaviour
                                              where T : MonoBehaviour
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public static bool hasInstance
    {
        get { return (SingletonBehaviour<T>._instance != null); }
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            _instance = null;
        }
    }
}


public abstract class SingletonNetworkLobbyManager<T> : NetworkLobbyManager where T : NetworkLobbyManager
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (null == _instance)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            _instance = null;
        }
    }
}

public abstract class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{
    protected static T _Instance = null;

    public static T Instance
    {
        get
        {
            if (null == _Instance)
            {
                GameObject go = GameObject.Find("DDOLGameObject");
                if (null == go)
                {
                    go = new GameObject("DDOLGameObject");
                    DontDestroyOnLoad(go);
                }
                _Instance = go.AddComponent<T>();

            }
            return _Instance;
        }
    }

    /// <summary>
    /// Raises the application quit event.
    /// </summary>
    private void OnApplicationQuit()
    {
        _Instance = null;
    }
}

public class SingletonException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonException"/> class.
    /// </summary>
    /// <param name="msg">Message.</param>
    public SingletonException(string msg)
        : base(msg)
    {
    }
}

// Singleton base class for ScriptableObject
public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            //DebugUtils.Log("Loading " + typeof(T).Name + " from resource folder");
            _instance = Resources.Load(typeof(T).Name, typeof(T)) as T;

            if (_instance == null)
            {
#if UNITY_EDITOR
                //DebugUtils.LogWarning(typeof(T).Name + " resource does not exist. Creating in Assets/Resources");
                _instance = ScriptableObject.CreateInstance<T>();

                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo("Assets/Resources");
                if (!directory.Exists)
                {
                    directory.Create();
                }

                AssetDatabase.CreateAsset(_instance, "Assets/Resources/" + typeof(T).Name + ".asset");
                AssetDatabase.SaveAssets();
#else		
				//DebugUtils.LogError("Error getting the " + typeof(T).Name + " resource");
#endif
            }

            return _instance;
        }
    }
}

// A singleton behaviour that will create itself if it doesn't already exist
public abstract class AutoSingletonBehaviour<T> : MonoBehaviour
                                                  where T : MonoBehaviour
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null && (!Application.isEditor || Application.isPlaying))
            {
                CreateInstance();
            }
            return _instance;
        }
    }

    public static bool hasInstance
    {
        get { return (AutoSingletonBehaviour<T>._instance != null); }
    }

    public static void EnsureInstanceExists()
    {
        if (_instance == null && (!Application.isEditor || Application.isPlaying))
        {
            CreateInstance();
        }
    }

    static void CreateInstance()
    {
        GameObject go = GameObject.Find("_Singletons");
        if (go == null)
        {
            go = new GameObject("_Singletons");
            DontDestroyOnLoad(go);
        }
        GameObject newGo = new GameObject("" + typeof(T));
        newGo.transform.parent = go.transform;
        _instance = newGo.AddComponent<T>();
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            GameObject.Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}