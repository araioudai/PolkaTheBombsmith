using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerManager : MonoBehaviour
{
    #region 定数
    //プレイヤーサウンド状態用定数
    private const int JUMP = 0;
    #endregion

    #region private変数
    Rigidbody2D rb;                                                             //Rigidbody2D 物理挙動用
    Animator animator;                                                          //アニメーション用
    PlayerAction controls;                                                      //InputSystem用
    Vector2 moveInput;                                                          //プレイヤーの移動用
    float posY;
    private AudioSource audioSource;                                            //音を鳴らす用プレーヤー
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>(); //playerオーディオ用
    [SerializeField] private BomSelect bomSelect;                               //ボムセレクトクラス参照
    [SerializeField] private GameObject bombCeiling;                            //爆弾プレファブ天井用
    [SerializeField] private LayerMask groundChecklayer;                        //床接地判定用のレイヤー
    [SerializeField] private LayerMask ceilingChecklayer;                       //壁接地判定用のレイヤー
    [SerializeField] private float speed;                                       //プレイヤーの移動スピード
    [SerializeField] private float bufferTime;                                  //先行入力ジャンプタイマーセット用
    //private Transform cameraScale;
    private float jumpBufferTimer;                                              //先行入力ジャンプタイマー
    private bool jumpBuffered;                                                  //先行入力ジャンプ用フラグ
    private bool isGround;                                                      //地面に接地しているか
    private bool isCeiling;                                                     //天井が上にあるか
    private bool isJump;                                                        //ジャンプ中かどうか
    private float jumpPow;                                                      //ジャンプ力（AddForce の力）
    private float jumpPowCeiling;                                               //天井がある時のジャンプ力
    private float interval;                                                     //爆弾を置くインターバル          
    private float setInterval;                                                  //爆弾インターバルセット用
    #endregion

    #region public変数
    public int generate;                                                        //爆弾生成個数上限
    public bool isGenerate;                                                     //生成したかのフラグ
    public bool gameOver;                                                       //ゲームオーバー用
    #endregion

    //public static int bombType;

    #region Unityイベント
    // Start is called before the first frame update
    void Start()
    {
        Init();
        PlayerPos();         //プレイヤーのスタート地点
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        TypeAheadJump();
        AttackCountControl();
        DownInterval();
    }
    #endregion

    #region Start呼び出し関数
    void Init()
    {
        rb = GetComponent<Rigidbody2D>(); //Rigidbody2D取得
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //cameraScale = gameObject.transform;
        posY = 0;
        bufferTime = 0.2f;
        jumpBufferTimer = 0;
        jumpBuffered = false;
        isGround = false;
        isCeiling = false;
        gameOver = false;
        speed = 4f;     //移動
        jumpPow = 650f; //ジャンプ力
        jumpPowCeiling = 400f;
        interval = 0;
        setInterval = 1f;
        //generate = 5;
    }

    //プレイヤーの初期座標
    void PlayerPos()
    {
        switch (TitleManager.stageIndex)
        {
            case 0:
                transform.position = new Vector3(0, 0, 0);
                break;

            case 1:
                transform.position = new Vector3(0, -5, 0);
                break;

            case 2:
                transform.position = new Vector3(0, -9, 0);
                break;
            
            case 3:
                transform.position = new Vector3(0, -5, 0);
                break;

            case 4:
                transform.position = new Vector3(0, -9, 0);
                break;

            case 5:
                transform.position = new Vector3(0, -9, 0);
                break;
        }
    }
    #endregion

    #region Update呼び出し関数
    //プレイヤー移動
    void Move()
    {
        var move = new Vector3(moveInput.x * speed * Time.deltaTime, 0, 0);
        // 向きの制御：移動方向に応じて左右反転
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);  // 右向き
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z); // 左向き
        }

        // 実際の移動
        transform.Translate(move);

        // アニメーション
        if (moveInput.x == 0 || isJump == true)
        {
            animator.SetTrigger("Idle"); //止まっているアニメーション再生
        }
        else
        {
            animator.SetTrigger("Run");  //歩くアニメーション再生
        }
    }

    void OnMovePerformrd(InputAction.CallbackContext context)
    {
        //入力時の動作
        moveInput = context.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext context)
    {
        //離したときの動作
        moveInput = Vector2.zero;
    }

    //爆弾攻撃
    void Attack(InputAction.CallbackContext context)
    {
        isCeiling = IsCeiling();
        if (isCeiling && generate > 0 && bomSelect.sele == 1 && interval == 0)
        {
            float gridSize = 1.0f; // タイルマップの1マスのサイズ（1ユニット = 1タイル）

            Vector3 playerPos = transform.position; // プレイヤーの現在位置を取得

            //X座標とY座標を、それぞれ現在いるマスの左下にスナップ(マス目にピタッと揃える)
            float snappedX = Mathf.Floor(playerPos.x / gridSize) * gridSize;

            float snappedY = Mathf.Floor(playerPos.y / gridSize) * gridSize;

            //マスの中央に来るように、それぞれ半マス分（gridSize / 2）を加算
            snappedX += gridSize / 2f;
            snappedY += gridSize / 2f;

            //スナップ後の位置ベクトルを作成（Zはそのまま）
            Vector3 snappedPosition = new Vector3(snappedX, snappedY, playerPos.z);

            //爆弾を生成（スナップされた位置に配置）
            GameObject bomb = Instantiate(bombCeiling, snappedPosition, Quaternion.identity);
            bomb.GetComponent<BomManager>().myBombType = BomSelect.seles;
            //bombPos = snappedPosition;

            generate -= 1;
            isGenerate = true;
            interval = setInterval;

            //Debug.Log($"爆弾生成（スナップ位置）: {snappedPosition}");
        }
        else if(generate > 0 && bomSelect.sele != 1 && interval == 0)
        {
            float gridSize = 1.0f; // タイルマップの1マスのサイズ（1ユニット = 1タイル）

            Vector3 playerPos = transform.position; // プレイヤーの現在位置を取得

            //X座標とY座標を、それぞれ現在いるマスの左下にスナップ(マス目にピタッと揃える)
            float snappedX = Mathf.Floor(playerPos.x / gridSize) * gridSize;

            float snappedY = Mathf.Floor(playerPos.y / gridSize) * gridSize;

            //マスの中央に来るように、それぞれ半マス分（gridSize / 2）を加算
            snappedX += gridSize / 2f;
            snappedY += gridSize / 2f;

            //スナップ後の位置ベクトルを作成（Zはそのまま）
            Vector3 snappedPosition = new Vector3(snappedX, snappedY, playerPos.z);

            //爆弾を生成（スナップされた位置に配置）
            if(BomSelect.seles == 3 && generate < 5)
            {

            }
            else
            {
                GameObject bomb = Instantiate(bombCeiling, snappedPosition, Quaternion.identity);
                bomb.GetComponent<BomManager>().myBombType = BomSelect.seles;
                generate -= 1;
                if (BomSelect.seles == 3) { generate -= 4; }
                isGenerate = true;
                interval = setInterval;
            }
            //bombPos = snappedPosition;
        }
    }

    void AttackCountControl()
    {
        if (generate < 0)
        {
            generate = 0;
        }
    }

    void DownInterval()
    {
        if(interval > 0)
        {
            interval -= Time.deltaTime;
        }
        if (interval < 0)
        {
            interval = 0;
        }
    }

    //足元にRayを飛ばして地面との接地判定を行う
    private bool IsGrounded()
    {
        float rayLength = 0.5f; //足元の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //下方向にRaycast（groundChecklayerに当たったら接地）
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //地面に当たったらtrue
    }

    //天井があるかないかチェック用
    private bool IsCeiling()
    {
        float rayLength = 3f; //上の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //上方向にRaycast（ceilingChecklayerに当たったら接地）
        RaycastHit2D hitCeiling = Physics2D.Raycast(origin, Vector2.up, rayLength, ceilingChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hitCeiling.collider != null; //地面に当たったらtrue
    }

    //ジャンプ処理
    void Jump(InputAction.CallbackContext context)
    {
        jumpBuffered = true;
        jumpBufferTimer = bufferTime;
        /*isGround = IsGrounded(); //毎フレーム、地面にいるか判定
        isCeiling = IsCeiling(); //天井の判定もここで更新

        //スペースキーが押され、かつ地面にいて、ジャンプ中でない場合
        if (isGround && !isJump && !isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPow)); //上向きに力を加える
            isJump = true;                        //ジャンプ中にする
            Debug.Log("ジャンプ！");
        }
        //スペースキーが押され、かつ地面にいて、ジャンプ中でなく天井が上にある場合
        if (isGround && !isJump && isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPowCeiling)); //上向きに力を加える
            isJump = true;                        //ジャンプ中にする
            Debug.Log("ジャンプ！");
        }*/
    }

    void TypeAheadJump()
    {
        isGround = IsGrounded();  //接地判定
        isCeiling = IsCeiling();  //天井判定

        //先行入力ありジャンプ処理
        if (jumpBuffered)//バッファありジャンプボタンが押された状態
        {
            jumpBufferTimer -= Time.deltaTime;

            if (isGround && !isJump)
            {
                float jumpForce = isCeiling ? jumpPowCeiling : jumpPow; //天井があるかどうかでジャンプ力を変える
                rb.AddForce(new Vector2(0, jumpForce));
                isJump = true;
                jumpBuffered = false;                                   //バッファを消去（ジャンプ成功）
                audioSource.PlayOneShot(audioClip[JUMP]);

                //Debug.Log("ジャンプ実行！");
            }

            //バッファ時間が過ぎたら無効化
            if (jumpBufferTimer <= 0f)
            {
                jumpBuffered = false;
            }
        }
    }

    private void Awake()
    {
        controls = new PlayerAction();
    }

    private void OnEnable()
    {
        //「Player」アクションの「Move」アクションマップを有効
        controls.Player.Move.Enable();
        controls.Player.Jump.Enable();
        controls.Player.Attack.Enable();

        //入力時に呼ばれるイベントを登録
        controls.Player.Move.performed += OnMovePerformrd; //入力時
        controls.Player.Move.canceled += OnMoveCanceled;   //離した時

        //ジャンプ
        controls.Player.Jump.performed += Jump;            //入力時

        //攻撃
        controls.Player.Attack.performed += Attack;        //入力時
    }

    private void OnDisable()
    {
        //入力時に呼ばれるイベントを削除
        controls.Player.Move.performed -= OnMovePerformrd; //入力時
        controls.Player.Move.canceled -= OnMoveCanceled;   //離した時

        //ジャンプ
        controls.Player.Jump.performed -= Jump;            //入力時

        //攻撃
        controls.Player.Attack.performed -= Attack;        //入力時

        //「Player」アクションの「Move」アクションマップを無効
        controls.Player.Move.Disable();
        controls.Player.Jump.Disable();
        controls.Player.Attack.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;  //ジャンプ終了
            isGround = true; //接地中
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero; //速度をゼロにする
        }
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBat") || collision.gameObject.CompareTag("EnemyDevil") || collision.gameObject.CompareTag("EnemyGhost") || collision.gameObject.CompareTag("GameOver"))
        {
            gameOver = true;
        }
    }
    #endregion
}
