using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    public List<Color> colorList;
    public List<string> companyCodeList;
    public List<Sprite> companySprite;
    public Dictionary<string, int> companyIdx = new Dictionary<string, int>();

    HashSet<string> _codeSet = new HashSet<string>();
    Dictionary<string, string> _regularCodeSet = new Dictionary<string, string>();

    void Start()
    {
        for (int i = 0; i < companySprite.Count; ++i)
        {
            companyIdx.Add(companyCodeList[i], i);
        }
    }

    public Sprite GetCompanySprite(string companyCode)
    {
        return companySprite[companyIdx[companyCode]];
    }

    public string GetRegularCode(string code)
    {
        if (_regularCodeSet.ContainsKey(code))
            return _regularCodeSet[code];
        return null;
    }

    public string MakeCode()
    {
        string code = Random.Range(1000, 9999).ToString();
        while (_codeSet.Contains(code))
            code = Random.Range(1000, 9999).ToString();
        _codeSet.Add(code);
        CreateRegularCode(code);
        return code;
    }

    public void RemoveCode(string code)
    {
        _codeSet.Remove(code);
    }

    //컨테이너(화물) 생성 함수
    public GameObject CreateContainer(IContainerInfo containerInfo)
    {
        GameObject go = Managers.Resource.Instantiate("Container");
        go.GetComponent<SettingContainer>().init(GetRegularCode(containerInfo.Code));
        go.GetComponent<Container>().Code = containerInfo.Code;
        //g에 컨테이너 정보 입력 및 컨테이너 외형 생성

        return go;
    }

    public void CreateRegularCode(string idCode)
    {
        string colorCode = string.Format("{0:D3}", Random.Range(1, Managers.Container.colorList.Count));

        int companyNumber = Random.Range(0, Managers.Container.companyCodeList.Count);
        string companyCode = Managers.Container.companyCodeList[companyNumber];
        string regularCode = idCode + "-" + colorCode + "-" + companyCode;

        _regularCodeSet.Add(idCode, regularCode);
    }
}
