using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    //public GameObject arrowObj;
    private int step = 0;
    private bool waiting = false;

    void Start()
    {
        ShowStep(step);
    }

    void Update()
    {
        if (waiting) return;

        switch (step)
        {
            case 0:
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    AdvanceStep();
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                    AdvanceStep();
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
                    AdvanceStep();
                break;
        }
    }

    void ShowStep(int s)
    {
        switch (s)
        {
            case 0:
                tutorialText.text = "<color=#00FF00>A/Dキー</color>で移動してみよう";
                break;
            case 1:
                tutorialText.text = "<color=#00FF00>Space</color>でジャンプ！";
                break;
            case 2:
                tutorialText.text = "<color=#00FF00>1,2,3</color>キーで爆弾を切り替えよう";
                break;
        }

        //arrowObj.SetActive(true); // 矢印など演出をON
    }

    void AdvanceStep()
    {
        waiting = true;
        //arrowObj.SetActive(false);

        // エフェクト演出後に次へ（コルーチンなども可）
        Invoke(nameof(NextStep), 1.0f);
    }

    void NextStep()
    {
        step++;
        waiting = false;
        ShowStep(step);
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }
}
