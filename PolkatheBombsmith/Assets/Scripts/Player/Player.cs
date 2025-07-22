/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] private GameManager manager; // ゴール到達時にゲームクリアを知らせるマネージャー参照

    Rigidbody2D rb;                                       //Rigidbody2D 物理挙動用
    Animator animator;
    Vector2 lscale;                                       //向きの制御

    [SerializeField] private GameObject bombCeiling;      //爆弾プレファブ天井用

    [SerializeField] private LayerMask groundChecklayer;  //接地判定用のレイヤー
    [SerializeField] private LayerMask ceilingChecklayer; //
    private float dir;                                    //プレイヤーの進行方向
    private bool isGround;                                //地面に接地しているか
    private bool isCeiling;
    private bool isJump;                                  //ジャンプ中かどうか
    private int count;                                    //スペースキーが押された回数（デバッグ用）
    private float move;

    private float speed;                                  //左右移動速度
    private float jumpPow;                                //ジャンプ力（AddForce の力）
    private float jumpPowCeiling;                         //天井がある時のジャンプ力

    void Start()
    {
        Init();              //初期化処理
        PlayerPos();         //プレイヤーのスタート地点
    }

    void Update()
    {
        PlayerMove();        //プレイヤーの左右移動処理
        PlayerJump();        //ジャンプ処理
        PlayerCeilingBomb(); //天井爆弾発射
    }

    //初期化処理
    void Init()
    {
        rb = GetComponent<Rigidbody2D>(); //Rigidbody2D取得
        //bombCeiling = (GameObject)Resources.Load("Circle"); //CircleプレハブをGameObject型で取得
        animator = GetComponent<Animator>();
        lscale = transform.localScale;

        isGround = false;
        isCeiling = false;
        count = 0;
        speed = 2f;     //移動（必要に応じて調整）
        jumpPow = 470f; //ジャンプ力
        jumpPowCeiling = 300f;
    }

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
                transform.position = new Vector3(0, -5, 0);
                break;
        }
    }

    //プレイヤーの左右移動
    void PlayerMove()
    {
        move = 0;
        //右矢印 or D キーで右移動
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            lscale.x = 1f;
            move = 1f;
            
            animator.SetTrigger("Run");  //歩くアニメーション再生
            if (isJump)
            {
                animator.SetTrigger("Idle"); //止まっているアニメーション再生
            }
        }
        //左矢印 or A キーで左移動
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
            lscale.x = -1f;
            move = 1f;
            
            animator.SetTrigger("Run");  //歩くアニメーション再生
            if (isJump)
            {
                animator.SetTrigger("Idle"); //止まっているアニメーション再生
            }
        }
        else
        {
            animator.SetTrigger("Idle"); //止まっているアニメーション再生
        }
        if (transform.position.x < -7.7)
        {
            transform.position = new Vector3(-7.7f, transform.position.y, transform.position.z);
        }
        transform.localScale = lscale;
    }

    //足元にRayを飛ばして地面との接地判定を行う
    public bool IsGrounded()
    {
        float rayLength = 0.7f; //足元の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //下方向にRaycast（groundChecklayerに当たったら接地）
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //地面に当たったらtrue
    }

    private bool IsCeiling()
    {
        float rayLength = 3f; //足元の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //下方向にRaycast（ceilingChecklayerに当たったら接地）
        RaycastHit2D hitCeiling = Physics2D.Raycast(origin, Vector2.up, rayLength, ceilingChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hitCeiling.collider != null; //地面に当たったらtrue
    }

    //ジャンプ処理
    void PlayerJump()
    {
        isGround = IsGrounded(); //毎フレーム、地面にいるか判定

        //スペースキーが押され、かつ地面にいて、ジャンプ中でない場合
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJump && !isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPow)); //上向きに力を加える
            isJump = true;                        //ジャンプ中にする
            Debug.Log("ジャンプ！");
        }
        //スペースキーが押され、かつ地面にいて、ジャンプ中でなく天井が上にある場合
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJump && isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPowCeiling)); //上向きに力を加える
            isJump = true;                        //ジャンプ中にする
            Debug.Log("ジャンプ！");
        }

        //スペースキーが押された回数を記録（デバッグ用）
        *//*if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            Debug.Log(count);
        }*//*
    }

    void PlayerCeilingBomb()
    {
        isCeiling = IsCeiling();
        if (Input.GetKeyDown(KeyCode.E) && isCeiling)
        {
            //Debug.Log("bakudan");
            // Cubeプレハブを元に、インスタンスを生成、
            //Instantiate(bombCeiling, transform.position, Quaternion.identity);
            *//* Instantiate(bombCeiling, new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z), Quaternion.identity);
             Debug.Log(Mathf.RoundToInt(transform.position.x));*//*
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
            //Vector3 snappedPosition = new Vector3(snappedX, playerPos.y, playerPos.z);

            //爆弾を生成（スナップされた位置に配置）
            Instantiate(bombCeiling, snappedPosition, Quaternion.identity);

            Debug.Log($"爆弾生成（スナップ位置）: {snappedPosition}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;  //ジャンプ終了
            isGround = true; //接地中
            count = 0;       //カウントリセット（任意）
        }

    *//*    if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero; //速度をゼロにする
        }*//*
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }
}
*/