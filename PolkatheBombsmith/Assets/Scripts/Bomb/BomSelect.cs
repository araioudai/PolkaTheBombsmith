using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BomSelect : MonoBehaviour
{
    PlayerAction controls;                                          //InputSystem用
    public int sele;//配列の中を指定するための変数
    public string[] Boms = { "通常", "天井", "床", "地雷" ,"強化" };//ボムの種類に配列
    public static int seles;
    //public GameObject text;//テキスト表示用

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Select();
    }

    void Init()
    {
        sele = 0;
        seles = sele;
    }

    void Select()
    {
        float wh = Input.GetAxis("Mouse ScrollWheel");//マウスホイールが少数を出すので整数のint型に変換する用
        sele += (int)(wh * 10);
        //色の変更
        if (sele >= Boms.Length)
        {
            sele = 0;
        }
        if (sele < 0)
        {
            sele = Boms.Length - 1;
        }
        //Debug.Log(sele);
        if (sele >= 0 && sele < Boms.Length)
        {
            //Debug.Log(Boms[sele]); // コンソールに表示
            //text.GetComponent<Text>().text = Boms[sele];
            // または、UIテキストに表示
            // GetComponent<TextMesh>().text = words[indexToShow];
        }
    }

    void SelectUp (InputAction.CallbackContext context)
    {
        sele += 1;
        seles = sele;
    }

    void SelectDown(InputAction.CallbackContext context)
    {
        sele -= 1;
        seles = sele;
    }

    private void Awake()
    {
        controls = new PlayerAction();
    }

    private void OnEnable()
    {
        //「Bomd」アクションの「Up.Down」アクションマップを有効
        controls.Bomb.Up.Enable();
        controls.Bomb.Down.Enable();

        //爆弾種類切り替え
        controls.Bomb.Up.performed += SelectUp;            //入力時
        controls.Bomb.Down.performed += SelectDown;        //入力時
    }

    private void OnDisable()
    {
        //爆弾種類切り替え
        controls.Bomb.Up.performed -= SelectUp;            //入力時
        controls.Bomb.Down.performed -= SelectDown;        //入力時

        //「Bomd」アクションの「Up.Down」アクションマップを無効
        controls.Bomb.Up.Disable();
        controls.Bomb.Down.Disable();
    }
}
