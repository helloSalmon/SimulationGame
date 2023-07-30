using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeLabel : MonoBehaviour
{
    public string Code { get; set; }
    Text _text;
    public string Text
    {
        get { return _text.text; }
        set { _text.text = value; }
    }

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        // 평면의 방향을 카메라의 앞 방향 벡터로 설정
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        // transform.LookAt(Camera.main.transform);
    }

    public void SetLabel(string str)
    {
        Code = str;
        _text.text = str;
    }
}
