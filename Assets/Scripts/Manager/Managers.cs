using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { init(); return s_instance; } }

    ResourceManager _resource = new ResourceManager();
    static TimeManager _time;

    public static ResourceManager Resource { get { return Instance._resource; } }
    public static TimeManager Time { get { init();  return _time; } }

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
    }

    static void init()
    {
        if (s_instance == null || _time == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
                go.AddComponent<TimeManager>();
            }

            DontDestroyOnLoad(go);
            _time = go.GetComponent<TimeManager>();
            s_instance = go.GetComponent<Managers>();
        }
    }
}
