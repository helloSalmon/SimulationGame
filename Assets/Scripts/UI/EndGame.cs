using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    private void Start()
    {
        GameObject go = GameObject.Find("Score Text");
        go.GetComponent<Text>().text = $"Score : {Score.currentScore}";
    }
    public void GameEnd()
    {
        SceneManager.LoadScene("TimeTamplate");
        Score.currentScore = 0;
    }
}
