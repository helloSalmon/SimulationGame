using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    static ResourceManager s_instance;
    public static ResourceManager Instance { get { init(); return s_instance; } }

    Dictionary<string, GameObject> _cache = new Dictionary<string, GameObject>();

    void Start()
    {
        init();
    }

    void Update()
    {
    }

    static void init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("ResourceManager");
            if (go == null)
            {
                go = new GameObject { name = "ResourceManager" };
                go.AddComponent<ResourceManager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<ResourceManager>();
        }
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        if (!_cache.ContainsKey(path))
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{path}");
            if (prefab == null)
            {
                Debug.Log($"Failed to load prefab : {path}");
                return null;
            }
            _cache[path] = prefab;
        }
        
        GameObject go = Object.Instantiate(_cache[path], parent);
        go.name = path;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        Object.Destroy(go);
    }
}
