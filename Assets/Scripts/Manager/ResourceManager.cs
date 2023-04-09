using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    static ResourceManager s_instance;
    static ResourceManager Instance { get { init(); return s_instance; } }

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
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, parent);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        Object.Destroy(go);
    }
}
