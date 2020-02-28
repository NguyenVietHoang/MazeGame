using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingUI : MonoBehaviour
{
    public Button playBtn;
    public Toggle map5_10Tgl;
    public Toggle map4_8Tgl;
    // Start is called before the first frame update
    void Start()
    {
        playBtn.onClick.RemoveAllListeners();
        playBtn.onClick.AddListener(PlayBtnPress);
    }

    void PlayBtnPress()
    {
        if(map5_10Tgl.isOn)
        {
            PlayerPrefs.SetInt("width", 10);
            PlayerPrefs.SetInt("height", 5);
        }
        else if (map4_8Tgl.isOn)
        {
            PlayerPrefs.SetInt("width", 8);
            PlayerPrefs.SetInt("height", 4);
        }

        SceneManager.LoadScene("MainGame");
    }

}
