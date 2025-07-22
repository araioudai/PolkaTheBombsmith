using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    #region 変数
    public TurnCheckGnd turnCheckGnd; // 地面判定スクリプト

    // コンポーネントの取得
    Rigidbody2D rb;

    // ジャンプ変数
    [SerializeField] float jumpForce = 300f; // ジャンプパワー
    [SerializeField] float jumpInterval = 2f; // ジャンプする間隔
    float jumpTimer; //　ジャンプタイマー

    // 移動変数
    [SerializeField] float speed = -5f; // 移動速度
    int direction; // 移動方向
    Vector3 scale; // エネミーのスケールを取得

    // レイキャスト変数
    Vector3 origin;       // Rayの開始位置
    Vector3 dirWall;      // Rayの方向(壁)
    Vector3 dirDown;      // Rayの方向(真下)
    Vector3 dirLeftDown;  // Rayの方向(左斜め下)
    Vector2 dirRightDown; // Rayの方向(右斜め下)
    float distance; // Rayの距離
    #endregion

    #region　関数
    /// <summary>
    /// 初期化関数
    /// </summary>
    void Init()
    {
        direction = 1; // スタート時の方向を右に設定
        scale = transform.localScale; // EnemyのScaleを変数に代入
        distance = 1f; // Rayの飛距離
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D を取得
        jumpTimer = 0; // タイマーの初期値を0に設定
    }

    /// <summary>
    /// Raycast関数
    /// </summary>
    void Raycast()
    {
        // Rayの開始位置を現在地に設定
        origin = transform.localPosition;
        // Rayの方向を左向きに設定 * 移動方向
        dirWall = Vector2.left * direction;
        // Rayの方向を下向きに設定
        dirDown = Vector2.down;
        // Rayの方向を左斜め下向きに設定、正規化
        dirLeftDown = (Vector2.down + Vector2.left).normalized;
        // Rayの方向を右斜め下向きに設定、正規化
        dirRightDown = (Vector2.down + Vector2.right).normalized;
        // Rayの飛距離を設定
        distance = 1f;

        // Rayを実行(WallレイヤーにRayが当たるか判定)
        RaycastHit2D hitWall = Physics2D.Raycast(origin, dirWall, distance, LayerMask.GetMask("Ground"));
        // 下三方向にRayを実行(GroundレイヤーにRayが当たるか判定)
        RaycastHit2D hitDown = Physics2D.Raycast(origin, dirDown, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitLeftDown = Physics2D.Raycast(origin, dirLeftDown, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRightDown = Physics2D.Raycast(origin, dirRightDown, distance, LayerMask.GetMask("Ground"));

        // 壁に当たった場合
        if (hitWall.collider != null)
        {
            // 進行方向を反転
            direction *= -1;
            //Debug.Log("壁に当たった");
        }

        // 地面に触れていた場合
        if (hitDown.collider != null || hitLeftDown.collider != null || hitRightDown.collider != null)
        {
            Jump(); // ジャンプを実行
            GroundCheck(); // 反転処理
        }
    }

    /// <summary>
    /// 地面判定関数
    /// </summary>
    void GroundCheck()
    {
        // 地面の端かどうか判定
        if (turnCheckGnd.turnPointGnd)
        {
            direction *= -1; // 向きを反転
            turnCheckGnd.turnPointGnd = false; // フラグをリセット
        }
    }

    /// <summary>
    /// ジャンプ関数
    /// </summary>
    void Jump()
    {
        jumpTimer += Time.deltaTime; //jumpTimerに毎フレームプラスする

        // jumpTimerがjumpInterval以上になったらジャンプする
        if (jumpTimer >= jumpInterval)
        {
            rb.AddForce(Vector2.up * jumpForce); // jumpForce分上に上がる
            jumpTimer = 0f; // タイマーをリセット
            //Debug.Log("ジャンプ");
        }
    }

    /// <summary>
    /// 移動関連関数
    /// </summary>
    void Move()
    {
        // OnTriggerExit2Dが動いたらEnemyの向きに-1をかける
        transform.localScale = new Vector3(scale.x * direction, scale.y, scale.x);
        // OnTriggerExit2Dが動いたらEnemyの移動ベクトルに-1かける
        transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);

        // 反転したかどうかのデバッグ
        if (scale.x > 0)
        {
            //Debug.Log("反転した");
        }
    }
    #endregion

    #region Unityイベント
    void Start()
    {
        Init(); // 初期化関数
    }

    void Update()
    {
        Raycast();     // Raycast関数
        Move();        // 移動関連関数
    }
    #endregion
}
