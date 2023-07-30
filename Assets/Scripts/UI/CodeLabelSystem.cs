using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CodeLabelSystem : MonoBehaviour
{
    List<CodeLabel> _labels = new List<CodeLabel>();
    List<DeliveryHolder> _holders = new List<DeliveryHolder>();
    Transform _root = null;
    List<int> _randIdx = new List<int>();

    void Start()
    {
        init();
    }

    void init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            go.SetActive(false);
            _labels.Add(go.GetComponent<CodeLabel>());
        }
        _root = GameObject.Find("SendLocations").transform;
        if (_root == null)
        {
            Debug.Log("SendLocations Missing!");
        }
        for (int i = 0; i < _root.childCount; i++)
        {
            _holders.Add(_root.GetChild(i).GetComponent<DeliveryHolder>());
            _holders[i].id = i;
        }

        for (int i = 0; i < _holders.Count; i++)
        {
            _randIdx.Add(_holders[i].id);
        }
    }

    void Update()
    {
        
    }

    public DeliveryHolder GetRandomHolder()
    {
        if (_randIdx.Count == 0)
            return null;

        int idx = Random.Range(0, _randIdx.Count);
        return _holders[_randIdx[idx]];
    }

    public void AssignHolder(DeliveryHolder holder, string code)
    {
        int idx = holder.id;
        _randIdx.Remove(idx);
        _labels[idx].gameObject.SetActive(true);
        _holders[idx].sendCode = code;
        _labels[idx].Text = code;
    }

    public void ClearHolder(DeliveryHolder holder)
    {
        int idx = holder.id;
        _randIdx.Add(idx);
        _holders[idx].sendCode = null;
        _labels[idx].gameObject.SetActive(false);
        _labels[idx].Text = null;
    }
}
