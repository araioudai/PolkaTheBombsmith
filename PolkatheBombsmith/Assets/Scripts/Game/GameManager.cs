using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //定数:結果の状態
    private const int GAME_OVER = 0;
    private const int GAME_CLEAR = 1;
    private const int GAME_CLEAR_ALL = 2;

    //bgmとseのオーディオを分けてならす
    
    private AudioSource audioSource;                                                   //音を鳴らす用プレーヤーBGM専用
    [SerializeField] private AudioSource seSource;                                     //SE専用
    //[SerializeField] private BomSelect select;
    [SerializeField] private PlayerManager player;                                     //PlayerManager参照
    [SerializeField] private GameObject virtualPad;                                    //バーチャルパッド(表示・非表示)
    [SerializeField] private GameObject bombType;                                      //バーチャルパッドoffの時爆弾の種類表示
    [SerializeField] private GameObject multiple;                                      //残り数(表示・非表示)
    [SerializeField] private GameObject attackButton;                                  //アタックボタン(表示・非表示) / 爆弾タイプ切り替えボタン(image切り替え用)
    [SerializeField] private GameObject gameClearPanel;                                //ゲームクリア時パネル(表示・非表示)
    [SerializeField] private GameObject gameOverPanel;                                 //ゲームオーバー時パネル(表示・非表示)
    //[SerializeField] private GameObject buttonUp;                                      //爆弾タイプ切り替えボタン(image切り替え用)
    [SerializeField] private Text enemyRestText;                                       //残り敵数
    [SerializeField] private Text textMultiple;                                        //残り爆弾数
    [SerializeField] private Text nowBomb;                                             //現在の爆弾タイプ
    [SerializeField] private Text textBombType;                                        //バーチャルパッドoffの時爆弾の種類テキスト
    [SerializeField] private GameObject currentSelected;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private string[] bomb = { "通常", "天井", "地面", "地雷", "強化" }; //ボムの種類に配列
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>();        //オーディオセット用
    [SerializeField] private AudioClip[] resultSE;                                     //ゲームクリアゲームオーバー時のサウンド用 
    [SerializeField] private Sprite[] spriteBombType;                                  //爆弾ボタンの画像切り替え
    private Image imageChange;
    //[SerializeField] private AudioClip bombSound;
    private float m_time;     //残り時間：タイムアタック用
    private bool startTimer;  //trueの時タイムアタック=ゲームオーバーになる（他のモードで0でゲームオーバーにならない用）
    private bool displayTime; //テキスト描画タイム
    private int m_rest;       //残り爆弾数：爆弾制限ステージ用
    private bool startRest;   //trueの時爆弾制限あり=ゲームオーバーになる（他のモードで0でゲームオーバーにならない用）
    private bool displayRest; //テキスト描画爆弾制限あり
    private bool gameClear;   //ゲームクリア用
    private bool clearChack;  //1回だけステージ解放
    private int reduce;       //残り爆弾減らす用
    
    public static int enemyRest;    //敵の数
    public static int stageClear;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        StageBgmStart();
    }

    // Update is called once per frame
    void Update()
    {
        TimeAttack();
        if(displayTime) DisplayTimer();
        if(displayRest) DisplayRest();
        DisplayEnemy();
        AttackText();
        GameState();
        ChangeButton();
        if (TitleManager.virtualPad == false)
        {
            UpdateSelectUI();
        }
        //AttackEnd();
        //Debug.Log(BomSelect.seles);
    }

    void Init()
    {
        //to doここでエネミーのマックスを宣言する or BomManagerでやる
        switch (TitleManager.stageIndex) {
            case 0:
                displayRest = true;
                SetRest(false, 10);
                break;
            case 1:
                displayRest = true;
                SetRest(false, 50);
                break;
            case 2:
                displayRest = true;
                SetRest(false, 40);
                break;
            case 3:
                textMultiple.fontSize = 100;
                displayTime = true;
                SetTimer(true, 60);
                SetRest(false, 1000);
                break;
            case 4:
                displayRest = true;
                SetRest(false, 30);
                break;
            case 5:
                displayRest = true;
                SetRest(true, 25);
                break;
        }
        enemyRest = 0;
        reduce = 1;
        clearChack = false;
        DisplayPad();
        //オブジェクトからTextコンポーネントを取得
        textMultiple = multiple.GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        imageChange = attackButton.GetComponent<Image>();
        Time.timeScale = 1f;
    }

    void DisplayPad()
    {
        if (TitleManager.virtualPad == true)
        {
            virtualPad.SetActive(true);
            bombType.SetActive(false);
        }
        else
        {
            virtualPad.SetActive(false);
            //テキストで今の爆弾の種類を表示を追加する
            bombType.SetActive(true);
        }
    }

    void StageBgmStart()
    {
        audioSource.PlayOneShot(audioClip[TitleManager.stageIndex]); //ステージごと用bgm
    }

    void AttackText()
    {
        if (BomSelect.seles >= bomb.Length)
        {
            BomSelect.seles = 0;
        }
        if (BomSelect.seles < 0)
        {
            BomSelect.seles = bomb.Length - 1;
        }
        nowBomb.text = "" + bomb[BomSelect.seles];
        textBombType.text = "" + bomb[BomSelect.seles];
    }

    void ChangeButton()
    {
        //爆弾タイプが天井か地面の時にくっつく爆弾画像にする
        if(BomSelect.seles == 1 || BomSelect.seles == 2)
        {
            imageChange.sprite = spriteBombType[1];
        }
        else
        {
            imageChange.sprite = spriteBombType[0];
        }
    }

    void ResetLastSelected()
    {
        if (lastSelected != null)
        {
            lastSelected.transform.localScale = Vector3.one;
            lastSelected = null;
        }
    }

    //ゲームクリア、ゲームオーバー
    void GameState()
    {
        //Debug.Log($"enemyRest={enemyRest}, gameOver={player.gameOver}");
        //敵が0 & プレイヤー未死亡 & まだクリア登録してない
        //ゲームクリア判定
        if (enemyRest == 0 && !player.gameOver && !clearChack)
        {
            StartCoroutine(CheckGameClear());
        }

        //ゲームオーバー
        if (!gameClear && player.gameOver)
        {
            //GameOver処理はここで実施（ゲームクリアより優先）
            if (TitleManager.virtualPad == false)
            {
                GameObject firstButton = gameOverPanel.transform.GetChild(0).gameObject;
                EventSystem.current.SetSelectedGameObject(firstButton);
                currentSelected = firstButton;
                lastSelected = firstButton;
                currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
            }

            audioSource.Stop();
            audioSource = null;
            Time.timeScale = 0f;
            seSource.PlayOneShot(resultSE[GAME_OVER]);
            gameOverPanel.SetActive(true);
        }
    }


    IEnumerator CheckGameClear()
    {
        yield return new WaitForSeconds(0.1f); //少し遅らせて正しいgameOverを反映

        if (!player.gameOver && !gameClear)
        {
            //ゲームクリアフラグを立てて、以降の処理を1回だけ行う
            gameClear = true;

            audioSource.Stop();
            audioSource = null;
            Time.timeScale = 0f;
            if (TitleManager.stageIndex == 5)
            {
                seSource.PlayOneShot(resultSE[GAME_CLEAR_ALL]);
            }
            else
            {
                seSource.PlayOneShot(resultSE[GAME_CLEAR]);
            }
            gameClearPanel.SetActive(true);

            //ステージクリア情報を保存（1度だけ行う）
            if (!clearChack && stageClear < TitleManager.stageIndex + 1)
            {
                //現在のステージが過去最高なら、次のステージをアンロック
                stageClear = TitleManager.stageIndex + 1;

                //一度だけステージアンロック処理が走るようにフラグを立てる
                clearChack = true;
            }

            if (TitleManager.virtualPad == false)
            {
                EventSystem.current.SetSelectedGameObject(gameClearPanel.transform.GetChild(0).gameObject);
                ResetLastSelected();
            }
        }
    }

    void UpdateSelectUI()
    {
        //現在選ばれているボタンを取得
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return; // null ならスキップ

        currentSelected = selected;

        //前回と違うボタンが選ばれた場合だけ処理を行う
        if (currentSelected != lastSelected)
        {
            //前に選ばれていたボタンのサイズを元に戻す
            if (lastSelected != null)
            {
                lastSelected.transform.localScale = Vector3.one;
            }
            //選ばれているボタンを少し大きくする
            currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);

            //今回選んだボタンを「前回選ばれたもの」として記録
            lastSelected = currentSelected;
        }
    }

    /*  void AttackEnd()
      {
          if (attackEnd)
          {
              audioSource.Play();
          }
      }*/

    void TimeAttack()
    {
        //Debug.Log(m_time);
       
        if (m_time > 0)
        {
            m_time -= Time.deltaTime;
        }
        if (m_time < 0)
        {
            m_time = 0;
        }
        if ((m_time == 0 && startTimer) || (m_rest == 0 && startRest))//||敵と当ったらゲームオーバー
        {
            //ゲームオーバー
            player.gameOver = true;
        }
    }

    void DisplayTimer()
    {
        //テキストの表示を入れ替える
        textMultiple.text = "00:" + (int)m_time;
        if (m_time < 10)
        {
            textMultiple.text = "00:0" + (int)m_time;
        }
    }

    //カウントが必要な面で使う(タイムアタック)
    void SetTimer(bool isTime, float time)
    {
        m_time = time;
        if (isTime)
        {
            startTimer = true;
            multiple.SetActive(true);
        }
    }


    void DisplayRest()
    {
        if (player.isGenerate)
        {
            m_rest -= reduce;
            if (BomSelect.seles == 3)
            {
                m_rest -= 4;
            }
            player.isGenerate = false;
        }
        //テキストの表示を入れ替える
        textMultiple.text = "残り:" + player.generate;
    }

    void SetRest(bool isRest, int rest)
    {
        m_rest = rest;
        player.generate = rest;
        if (isRest)
        {
            startRest = true;
            multiple.SetActive(true);
        }
        if(!isRest && !startTimer)
        {
            multiple.SetActive(true);
        }
    }

    void DisplayEnemy()
    {
        enemyRestText.text = "敵: " + enemyRest;
    }

    public void NextStage()
    {
        Time.timeScale = 1f;
        TitleManager.stageIndex += 1; //次のステージ番号に進む
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void ReStart()
    {
        Time.timeScale = 1f;
        enemyRest = 0;
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void Title()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
