using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Basic singleton and persistent singleton implementation with generic type.
/// </summary>

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this as T;
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}

// Try avoiding using this as much as possible as the execution order of Awake, Start, and OnNetworkSpawn is inconsistent between
// Dynamically spawned objects and objects placed in the scene
public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Network Despawn destroyed: " + this + " Singleton instance");
        Instance = null;
    }
}
