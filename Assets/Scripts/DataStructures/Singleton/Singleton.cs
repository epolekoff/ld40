using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    var foundCount = FindObjectsOfType(typeof(T)).Length;

                    if (foundCount > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                    }
                    else if(foundCount == 0)
                    {
                        // If the object does not exist but someone calls it, make a new one.
                        var go = new GameObject(typeof(T).Name, new[] { typeof(T) });
                        _instance = go.GetComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Clean up the singleton instance.
    /// </summary>
    public void OnDestroy()
    {
        _instance = null;
    }
}