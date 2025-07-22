using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    #region 変数
    public TurnCheckGnd turnCheckGnd; // 地面判定スクリプト

    // 移動変数
    [SerializeField] float speed = -1f; // 移動速度
    int direction; // 移動方向
    Vector3 scale; // エネミーのスケールを取得

    // レイキャスト変数
    Vector3 origin; // Rayの開始位置
    Vector3 dir;    // Rayの方向
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
        speed = -1f;
    }

    /// <summary>
    /// Raycast関数
    /// </summary>
/*    void Raycast()
    {
        // Rayの開始位置を現在地に設定
        origin = transform.localPosition;
        // Rayの方向を左向きに設定 * 移動方向
        dir = Vector2.left * direction;
        // Rayの飛距離を設定
        distance = 1f;

        // Rayを実行(WallレイヤーにRayが当たるか判定)
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, LayerMask.GetMask("Wall"));

        // 壁に当たった場合
        if (hit.collider != null)
        {
            // 進行方向を反転
            direction *= -1;
            //Debug.Log("壁に当たった");
        }
    }*/

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
        //Raycast();     // Raycast関数
        GroundCheck(); // 地面判定関数
        Move();        // 移動関連関数
    }
    #endregion
}
