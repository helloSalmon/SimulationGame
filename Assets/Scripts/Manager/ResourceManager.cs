using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager
{
    Dictionary<string, GameObject> _cache = new Dictionary<string, GameObject>();

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null, string name = null)
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
        if (name == null) name = path;
        go.name = name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        Object.Destroy(go);
    }
}
