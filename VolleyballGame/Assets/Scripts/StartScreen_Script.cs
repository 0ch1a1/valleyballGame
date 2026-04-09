using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen_Script : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI blinkText;
    private float ElapsedTime;
    [SerializeField] private float blinkSpeed;
    [SerializeField] private GameObject[] canvas;
    private int nowActiveCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Initializer();
    }

    void Initializer()
    {
        ElapsedTime = 0f;
        for (int i = 0; i < canvas.Length; i++)
        {
            if(i == 0)
            {
                canvas[i].SetActive(true);
                nowActiveCanvas = i;
            }
            else
            {
                canvas[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       blinkText.color = TextBlinker(blinkText);
    }

    Color TextBlinker(TextMeshProUGUI targetText)
    {
        ElapsedTime += Time.deltaTime * blinkSpeed;
        Color targetColor = targetText.color;
        targetColor.a =  Mathf.Sin(ElapsedTime);
        return targetColor;
    }

    public void ChangeCanvas(int afterCanvas)
    {
        canvas[nowActiveCanvas].SetActive(false);
        nowActiveCanvas = afterCanvas - 1;
        canvas[afterCanvas - 1].SetActive(true);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GameOver()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

}
