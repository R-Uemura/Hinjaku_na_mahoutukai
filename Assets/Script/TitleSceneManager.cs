using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 30; // FPS30‚ÉŒÅ’è
    }

    public void ClickSceneChengeButton()
    {
        SceneManager.LoadScene("MainGame");
    }
}
