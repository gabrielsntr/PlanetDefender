using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject bgVideo;

    // Start is called before the first frame update
    void Start()
    {
        TransparentAll();
        bgVideo.GetComponent<VideoPlayer>().prepareCompleted += VideoPrepared;
        bgVideo.GetComponent<VideoPlayer>().Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        canvas.transform.Find("MainMenu").gameObject.SetActive(!canvas.transform.Find("MainMenu").gameObject.activeSelf);
        canvas.transform.Find("SettingsMenu").gameObject.SetActive(!canvas.transform.Find("SettingsMenu").gameObject.activeSelf);
    }

    public void QuitDialog()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        canvas.transform.Find("MainMenu").gameObject.SetActive(!canvas.transform.Find("MainMenu").gameObject.activeSelf);
        canvas.transform.Find("QuitWindow").gameObject.SetActive(!canvas.transform.Find("MainMenu").gameObject.activeSelf);
    }

    public void Quit()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        Application.Quit();
    }


    void VideoPrepared(VideoPlayer vPlayer)
    {
        vPlayer.Play();
        StartCoroutine(FadeUi());
    }

    private IEnumerator FadeUi()
    {
        Color white = new Color(255, 255, 255, 1f);
        
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1.5f)
        {
            Color newColor = new Color(255, 255, 255, Mathf.Lerp(0, 1.0f, t));
            ColorAll(newColor);
            yield return null;
        }
    }

    private void TransparentAll()
    {
        Color transparent = new Color(255, 255, 255, 0f);
        GameObject pauseMenu = canvas.transform.Find("MainMenu").gameObject;
        canvas.transform.Find("PlanetDefender").GetComponent<Text>().color = transparent;
        for (int i = 0; i < pauseMenu.transform.childCount; i++)
        {
            pauseMenu.transform.GetChild(i).GetComponent<Image>().color = transparent;
            pauseMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().color = transparent;
        }
    }

    private void ColorAll(Color newColor)
    {
        GameObject pauseMenu = canvas.transform.Find("MainMenu").gameObject;
        canvas.transform.Find("PlanetDefender").GetComponent<Text>().color = newColor;
        for (int i = 0; i < pauseMenu.transform.childCount; i++)
        {
            pauseMenu.transform.GetChild(i).GetComponent<Image>().color = newColor;
            pauseMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().color = newColor;
        }
    }

    public void LevelSelector()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        canvas.transform.Find("LevelSelectMenu").gameObject.SetActive(!canvas.transform.Find("LevelSelectMenu").gameObject.activeSelf);
        canvas.transform.Find("MainMenu").gameObject.SetActive(!canvas.transform.Find("MainMenu").gameObject.activeSelf);
    }

    public void OnClicked(Button button)
    {
        LevelSelected.Level = Convert.ToInt32(button.transform.Find("LevelId").GetComponent<Text>().text);
        Play();
    }
}
