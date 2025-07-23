using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BomManager : MonoBehaviour
{
    //定数:敵死亡サウンド用
    private const int SLIME = 0;
    private const int BAT = 1;
    private const int GHOST = 2;
    private const int DEVIL = 3;

    private Vector3 bombPos;                             //爆弾のポジション
    private Rigidbody2D rb;
    private CircleCollider2D circle;
    private BomSelect bomSelect;
    private SpriteRenderer bombSprite;
    private AudioSource audioSource;                     //音を鳴らす用プレーヤー
    [SerializeField] private LayerMask groundChecklayer; //床接地判定用のレイヤー
    [SerializeField] private AudioClip bombSound;        //オーディオセット用
    [SerializeField] private AudioClip[] enemyDead;      //敵を倒した時配列
    [SerializeField] private AudioClip ghostDead;        //おばけ倒した時
    [SerializeField] private AudioClip devilDead;        //デビル倒した時
    [SerializeField] private GameObject bombPrefab;      //爆弾エフェクト用
    [SerializeField] private GameObject bomb;            //爆弾自身取得 
    [SerializeField] private GameObject blockPrefab;     //ブロック破壊エフェクト用
    [SerializeField] private Sprite[] bombType;          //爆弾タイプ画像切り替え表示用
    //[SerializeField] private List<AudioClip> audioClip = new List<AudioClip>(); //オーディオセット用
    private float timer;                                 //爆発時間
    private float timerBlock;
    private float timerSet;
    private int bomNo = 0;                               //爆弾種類
    private bool isCeiling;                              //天井用フラグ
    //private bool isGrounded;                             //地面用フラグ
    private bool exploded;                               //爆発済みフラグ
    private float count;
    private float setCount;

    public int myBombType;                               //プレイヤーから受け取った個別の爆弾タイプ

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        var colliderTest = GetComponent<CapsuleCollider2D>();
        circle = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        bombSprite = GetComponent<SpriteRenderer>();
        GameObject obj = GameObject.Find("BomSelecter");
        bomSelect = obj.GetComponent<BomSelect>();
        bombPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        blockPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //timerSet = 1f;
        colliderTest.enabled = false;
        exploded = false;
        count = 0;
        setCount = 0.5f;
        if (myBombType == 0)//通常
        {
            timer = 2f;
            rb.gravityScale = 1f;
            bomNo = 0;
            bombSprite.sprite = bombType[0];
        }
        else if (myBombType == 1)//天井
        {
            timer = 2f;
            rb.gravityScale = -1f;
            bomNo = 1;
            bombSprite.sprite = bombType[1];
        }
        else if (myBombType == 2)//床
        {
            timer = 2f;
            rb.gravityScale = 1f;
            transform.Rotate(0, 0, 180);
            bomNo = 2;
            bombSprite.sprite = bombType[1];
        }
        else if (myBombType == 3)//地雷
        {
            timer = 1000f;
            rb.gravityScale = 1f;
            colliderTest.enabled = true;
            bomNo = 3;
            bombSprite.sprite = bombType[0];
        }
        else if (myBombType == 4)//強化
        {
            timer = 5f;
            rb.gravityScale = 1f;
            bomNo = 4;
            bombSprite.sprite = bombType[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        DestroyBomb();
        CountTimer();
        if (myBombType == 1)
        {
            PutOnTrigger();
        }
        //Debug.Log(myBombType);
    }

    void Explode()
    {
        //StageLoaderのインスタンス経由でアクセス
        Tilemap tilemap = StageLoader.Instance.FloorTilemap; //Tilemapにアクセス
        //GameObject enemy = StageLoader.Instance.EnemyPrefab; //EnemyPrefabにアクセス

        //現在の爆弾のワールド座標を、Tilemap のセル座標（マス目）に変換
        Vector3Int baseCell = tilemap.WorldToCell(transform.position);

        //十字方向（上、下、右、左、自分）
        //爆弾を中心に、上下左右＋自分自身の5マス分の相対位置を指定
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),  //中心
            new Vector3Int(0, 1, 0),  //上
            new Vector3Int(0, -1, 0), //下
            new Vector3Int(1, 0, 0),  //右
            new Vector3Int(-1, 0, 0)  //左
        };

        //foreachは、「配列やリストのすべての要素に1つずつアクセスしたいときに便利」for文よりシンプルで安全
        /*テンプレ
        foreach (型 変数名 in コレクション)
        {
            // 変数名 を使った処理
        }
        /*・型：配列の中の要素の型（例：int, Vector3Intなど）

          ・変数名：そのループ中に使う変数の名前

          ・コレクション：配列やリストなど（例：directions）*/
        foreach (var dir in directions)
        {
            //基準セル位置（baseCell）に方向を加えて、攻撃対象セルを決定
            Vector3Int targetCell = baseCell + dir;

            //対象セルにタイルがある且、爆弾タイプが天井用か床用の時に破壊
            if (tilemap.HasTile(targetCell) && (myBombType == 1 || myBombType == 2))
            {
                timerBlock = 0;
                //タイルを削除（null をセット）＝破壊
                tilemap.SetTile(targetCell, null);
                Vector3 pos = new Vector3(targetCell.x + 0.5f, targetCell.y + 1f, targetCell.z);
                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);
                Destroy(block, 1f);

/*                timerBlock = timerSet;
                if (timerBlock <= 0)
                {
                    timerBlock = 0;
                    //タイルを削除（null をセット）＝破壊
                    tilemap.SetTile(targetCell, null);
                    Vector3 pos = new Vector3(targetCell.x + 0.5f, targetCell.y + 1f, targetCell.z);
                    GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);
                    Destroy(block, 1f);
                }*/
            }
            //範囲内に敵がいる且、爆弾が通常用の時倒す（削除）
            /*if (enemy.targetCell && myBombType == 0)
            {

            }*/
            if (myBombType == 0)
            {
                Vector2 worldPos = tilemap.GetCellCenterWorld(targetCell);
                float radius = 0.2f; //小さい値でピンポイントにする

                Collider2D hit = Physics2D.OverlapCircle(worldPos, radius, LayerMask.GetMask("Enemy")); // "Enemy" レイヤーのみに反応

                if (hit != null && hit.CompareTag("Enemy"))
                {
                    Destroy(hit.gameObject); //敵を倒す
                    audioSource.PlayOneShot(enemyDead[SLIME]);
                    GameManager.enemyRest -= 1;
                }
                if (hit != null && hit.CompareTag("EnemyGhost"))
                {
                    Destroy(hit.gameObject); //敵を倒す
                    //音 to do
                    audioSource.PlayOneShot(enemyDead[GHOST]);
                    GameManager.enemyRest -= 1;
                }
                if (hit != null && hit.CompareTag("EnemyDevil"))
                {
                    Destroy(hit.gameObject); //敵を倒す
                    //音 to do
                    audioSource.PlayOneShot(enemyDead[DEVIL]);
                    GameManager.enemyRest -= 1;
                }
            }
            if (myBombType == 1)
            {
                Vector2 worldPos = tilemap.GetCellCenterWorld(targetCell);
                float radius = 0.2f; //小さい値でピンポイントにする

                Collider2D hit = Physics2D.OverlapCircle(worldPos, radius, LayerMask.GetMask("Enemy")); // "Enemy" レイヤーのみに反応
                if (hit != null && hit.CompareTag("EnemyBat"))
                {
                    Destroy(hit.gameObject); //敵を倒す
                    //音 to do
                    audioSource.PlayOneShot(enemyDead[BAT]);
                }
            }
        }
    }

    IEnumerator ExplodePlace()
    {
        for(int i = 0; i < 75; i++)
        {
            yield return null;
        }
        //yield return new WaitForSeconds(0.75f);
        Explode();
    }

    void CountTimer()
    {
        if (count > 0)
        {
            count -= Time.deltaTime;
        }
        Debug.Log(count);
    }

    void DestroyBomb()
    {
        bombPos = bomb.transform.position;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0 && !exploded)
        {
            exploded = true;                           //1回限り
            timer = 0;                                 //タイマーを0以下にしないようにセット
            //Explode();
            StartCoroutine(ExplodePlace());            //爆発処理（爆弾タイプが天井用か床用の時Tilemap破壊）
            bombSprite.enabled = false;                //爆弾を非表示にする
            if(myBombType != 1)circle.enabled = false; //当たり判定も消す
            audioSource.PlayOneShot(bombSound);        //爆発音
            GameObject bomb = Instantiate(bombPrefab, bombPos, Quaternion.identity);
            Destroy(bomb, 1f);
            Destroy(gameObject, 3f);                   //非表示にした爆弾削除
        }
    }

    /*    IEnumerator DestroyAfterSound(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }*/

    //足元にRayを飛ばして地面との接地判定を行う
    private bool IsCeiling()
    {
        float rayLength = 0.7f; //上の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //下方向にRaycast（groundChecklayerに当たったら接地）
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLength, groundChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hit.collider != null; //天井に当たったらtrue
    }

/*    //足元にRayを飛ばして地面との接地判定を行う
    private bool IsGrounded()
    {
        float rayLength = 0.4f; //足元の距離
        Vector2 origin = transform.position; //Rayの始点（プレイヤーの位置）

        //下方向にRaycast（groundChecklayerに当たったら接地）
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //地面に当たったらtrue
    }*/

    void PutOnTrigger()
    {
        circle.isTrigger = true;

        isCeiling = IsCeiling();

        if (isCeiling)
        {
            circle.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.gravityScale = 0;
            circle.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (myBombType == 3)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                timer = 0;
                Destroy(collider.gameObject); //敵を倒す
                audioSource.PlayOneShot(enemyDead[SLIME]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     Destroy(collider.gameObject); //敵を倒す
                     audioSource.PlayOneShot(enemyDead[SLIME]);
                     count = 0;
                 }*/
            }
            if (collider.gameObject.CompareTag("EnemyGhost"))
            {
                timer = 0;
                Destroy(collider.gameObject); //敵を倒す
                audioSource.PlayOneShot(enemyDead[GHOST]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     count = 0;
                     Destroy(collider.gameObject); //敵を倒す
                     audioSource.PlayOneShot(enemyDead[GHOST]);
                 }*/
            }
            if (collider.gameObject.CompareTag("EnemyDevil"))
            {
                timer = 0;
                Destroy(collider.gameObject); //敵を倒す
                audioSource.PlayOneShot(enemyDead[DEVIL]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     count = 0;
                     Destroy(collider.gameObject); //敵を倒す
                     audioSource.PlayOneShot(enemyDead[DEVIL]);
                 }*/
            }
        }
    }
}
