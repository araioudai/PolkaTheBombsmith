using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //ステージの状態
    const int STAGE_ROCK = 0;
    const int STAGE_CANCELL = 1;

    const int TITLE_SELECT_MODE = 0;
    const int TITLE_SELECT_STAGE = 1;
    const int TITLE_SELECT_SETTING = 2;

    private AudioSource audioSource;                                           //音を鳴らす用プレーヤー
    private PlayerAction controls;                                             //InputSystem用
    private GameObject objctName;                                              //オブジェクト名
    [SerializeField] private GameObject selectMode;                            //モード選択用パネル
    [SerializeField] private GameObject selectStage;                           //ステージ選択用パネル
    [SerializeField] private GameObject selectSetting;                         //設定用パネル
    [SerializeField] private GameObject[] stageRock;                           //ステージ選択鍵付き
    [SerializeField] private GameObject[] stageCancell;                        //ステージ選択鍵なし
    [SerializeField] private GameObject TextRock;                              //ロック表示テキスト
    [SerializeField] private GameObject currentSelected;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private Image buttonOn;                                   //設定画面On用
    [SerializeField] private Image buttonOff;                                  //設定画面Off用
    [SerializeField] private List<AudioClip> buttonSE = new List<AudioClip>(); //オーディオセット用ボタン関係

    private float targetLight = 1f;                                            //設定ボタン濃くする
    private float targetDeep = 0.5f;                                           //設定ボタン薄くする
    private int stageState;                                                    //ステージの状態保存
    private int cancellNumber;                                                 //解放されているステージ
    private int titleNunber;                                                   //今タイトルのどこにいるか
    private bool isKDown;                                                      //Kキーが押された時フラグ
    private bool isEDown;                                                      //Eキーが押された時フラグ
    private bool isYDown;                                                      //Yキーが押された時フラグ

    public static int stageIndex;                                              //ステージ番号
    public static bool virtualPad;
    //初期選択ボタン
    public GameObject pouseFirstbutton;
    public GameObject stageFirstbutton;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        ButtonSet();
        StageCancell();
    }  

    // Update is called once per frame
    void Update()
    {
        CheckCheatKey();
        UpdateSelectUI();
    }

    void Init()
    {
        audioSource = GetComponent<AudioSource>();
        buttonOn.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetLight);
        buttonOff.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetDeep);
        //stageIndex = 0;
        //cancellNumber = 0;
        titleNunber = TITLE_SELECT_MODE;
        stageState = STAGE_ROCK;
        isKDown = false;
        isEDown = false;
        isYDown = false;
        if (stageIndex == 0)
        {
            virtualPad = true;
        }
    }
    void ButtonSet()
    {
        EventSystem.current.SetSelectedGameObject(selectMode.transform.GetChild(1).gameObject);
    }

    void UpdateSelectUI()
    {
        // 何も選ばれていないときは、最初の解除されたステージボタンを選ぶ
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            for (int i = 0; i < stageCancell.Length; i++)
            {
                if (stageCancell[i].activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(stageCancell[i]);
                    break;
                }
            }
        }

        // 現在選ばれているボタンを取得
        currentSelected = EventSystem.current.currentSelectedGameObject;

        // 前回と違うボタンが選ばれた場合だけ処理を行う
        if (currentSelected != lastSelected)
        {
            // 前に選ばれていたボタンのサイズを元に戻す
            if (lastSelected != null)
            {
                lastSelected.transform.localScale = Vector3.one;
            }

            // 今選ばれているボタンを少し大きくする（強調表示）
            if (currentSelected != null)
            {
                currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
            }

            // 今回選んだボタンを「前回選ばれたもの」として記録
            lastSelected = currentSelected;
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

    public void PushTutorial()
    {
        audioSource.PlayOneShot(buttonSE[0]);
        SceneManager.LoadScene("Tutorial");
    }

    public void PushSetting()
    {
        ResetLastSelected(); //初期選択ボタンの初期化
        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pouseFirstbutton); //初期選択ボタンの再指定
        audioSource.PlayOneShot(buttonSE[0]);
        selectMode.SetActive(false);
        selectStage.SetActive(false);
        selectSetting.SetActive(true);
    }

    // 「ステージ選択」ボタンを押したときの処理
    public void PushStage()
    {
        ResetLastSelected(); // 前に選ばれていたボタンの拡大を元に戻す

        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(stageFirstbutton); //初期選択ボタンの再指定

        // モードや設定画面を非表示にして、ステージ選択画面を表示
        selectMode.SetActive(false);
        selectSetting.SetActive(false);
        selectStage.SetActive(true);

/*        // 最初に選ぶステージボタン（解除されている中で一番上）を選択状態にする
        for (int i = 0; i < stageCancell.Length; i++)
        {
            if (stageCancell[i].activeSelf)
            {
                // Unityのイベントシステムで選択状態に設定する
                EventSystem.current.SetSelectedGameObject(stageCancell[i]);
                break;
            }
        }
*/
        //ステージボタンに↑↓←→で移動できるように設定
        SetupStageButtonNavigation();

        audioSource.PlayOneShot(buttonSE[0]);
    }

    // ステージ選択ボタンを上下左右に選べるように設定する関数（2行×3列用）
    void SetupStageButtonNavigation()
    {
        int rowCount = 2;   // 行（縦）の数
        int columnCount = 3; // 列（横）の数

        for (int i = 0; i < stageCancell.Length; i++)
        {
            Button button = stageCancell[i].GetComponent<Button>();
            if (button == null) continue;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            int row = i / columnCount; // 行（0 or 1）
            int col = i % columnCount; // 列（0〜2）

            // 上に移動（1段目以降なら1つ上にあるボタン）
            if (row > 0)
            {
                int upIndex = i - columnCount;
                if (stageCancell[upIndex].activeSelf)
                    nav.selectOnUp = stageCancell[upIndex].GetComponent<Selectable>();
            }

            // 下に移動（最終段の1つ前までなら1つ下にあるボタン）
            if (row < rowCount - 1)
            {
                int downIndex = i + columnCount;
                if (downIndex < stageCancell.Length && stageCancell[downIndex].activeSelf)
                    nav.selectOnDown = stageCancell[downIndex].GetComponent<Selectable>();
            }

            // 左に移動（左端以外なら左にあるボタン）
            if (col > 0)
            {
                int leftIndex = i - 1;
                if (stageCancell[leftIndex].activeSelf)
                    nav.selectOnLeft = stageCancell[leftIndex].GetComponent<Selectable>();
            }

            // 右に移動（右端以外なら右にあるボタン）
            if (col < columnCount - 1)
            {
                int rightIndex = i + 1;
                if (rightIndex < stageCancell.Length && stageCancell[rightIndex].activeSelf)
                    nav.selectOnRight = stageCancell[rightIndex].GetComponent<Selectable>();
            }

            button.navigation = nav;
        }
    }


    public void PushExit()
    {
        ResetLastSelected(); //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(selectMode.transform.GetChild(1).gameObject);
        audioSource.PlayOneShot(buttonSE[1]);
        selectStage.SetActive(false);
        selectSetting.SetActive(false);
        selectMode.SetActive(true);
    }

    public void DisplayPadOn()
    {
        buttonOn.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetLight);
        buttonOff.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetDeep);
        virtualPad  = true;
    }

    public void DisplayPadOff()
    {
        buttonOff.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetLight);
        buttonOn.color = new Color(buttonOn.color.r, buttonOn.color.g, buttonOn.color.b, targetDeep);
        virtualPad = false;
    }

    void CheckCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.K)) isKDown = true;
        if (Input.GetKeyDown(KeyCode.E)) isEDown = true;
        if (Input.GetKeyDown(KeyCode.Y)) isYDown = true;

        if (isKDown && isEDown && isYDown)
        {
            GameManager.stageClear = stageRock.Length; //全ステージ解放
            //stageState = STAGE_CANCELL;
            Debug.Log("ステージ解除");

            //フラグリセット
            isKDown = false;
            isEDown = false;
            isYDown = false;

            StageCancell();
        }
    }

    void StageCancell()
    {
        //ステージがロック状態かつ解除対象が残っている間、すべて反映する
        while (stageState == STAGE_ROCK && GameManager.stageClear-1 > cancellNumber && cancellNumber < stageRock.Length)
        /*while (stageState == STAGE_ROCK && GameManager.stageClear >= cancellNumber + 1 && cancellNumber < stageRock.Length)*/
        {
            //鍵付きボタンを非表示、鍵なしボタンを表示
            stageRock[cancellNumber].SetActive(false);
            stageCancell[cancellNumber].SetActive(true);

            //次のステージへ
            cancellNumber++;
            Debug.Log(cancellNumber);
        }

        //全ステージが解放されたら状態更新
        if (cancellNumber >= stageRock.Length)
        {
            stageState = STAGE_CANCELL;
            Debug.Log("すべてのステージが解除");
        }

        for (int i = 0; i < stageCancell.Length; i++)
        {
            Button button = stageCancell[i].GetComponent<Button>();
            if (button != null)
            {
                button.interactable = (i <= GameManager.stageClear);
            }
        }
    }



    public void PushSelectStage()
    {
        //ResetLastSelected();
        //EventSystem.current.SetSelectedGameObject(null);
        //押されたボタンのオブジェクト名を取得
        objctName = EventSystem.current.currentSelectedGameObject;
        string name = objctName.name;

        //ステージ番号に変換
        if (name.StartsWith("Stage"))
        {
            string numberPart = name.Replace("Stage", "");

            //TryParseで安全に整数に変換（失敗してもクラッシュしない）
            if (int.TryParse(numberPart, out int number))
            {
                //ステージ1は常に選択可能、それ以外はステージが解除（解放）されている状態でのみ選択可能
                /* if (number == 1 || (number <= GameManager.stageClear + 1))*/
                if (number <= GameManager.stageClear + 1)
                {
                    audioSource.PlayOneShot(buttonSE[0]);
                    stageIndex = number; //選択されたステージ番号を保存
                    StartCoroutine(StageLoad());
                }
            }
            else
            {
                //Debug.LogWarning("ステージ名に数値が含まれていません: " + name);
                
                StartCoroutine(TextCountDown());
            }
        }
    }

    IEnumerator StageLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game");
    }

    IEnumerator TextCountDown()
    {
        TextRock.SetActive(true);

        yield return new WaitForSeconds(1.0f); //1秒待つ

        TextRock.SetActive(false);
    }
/*    private void Awake()
    {
        controls = new PlayerAction();
    }

    void PadUp(InputAction.CallbackContext context)
    {
        if (titleNunber == TITLE_SELECT_MODE)
        {

        }
    }

    void PadDown(InputAction.CallbackContext context)
    {

    }

    private void OnEnable()
    {
        //「Title」アクションのアクションマップを有効
        controls.Title.LUp.Enable();
        controls.Title.LDown.Enable();

        //上、下入力時
        controls.Title.LUp.performed -= PadUp;            //入力時
        controls.Title.LDown.performed -= PadDown;            //入力時
    }

    private void OnDisable()
    {
        //上、下入力時
        controls.Title.LUp.performed -= PadUp;            //入力時
        controls.Title.LDown.performed -= PadDown;            //入力時

        //「Title」アクションのアクションマップを無効
        controls.Title.LUp.Disable();
        controls.Title.LDown.Disable();
    }*/
}
