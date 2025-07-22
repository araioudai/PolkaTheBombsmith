using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public static StageLoader Instance { get; private set; } //他のスクリプトから StageLoader.Instance でアクセスできるようにする

    [SerializeField] private Tilemap floorTilemap;   //床用Tilemap
    [SerializeField] private Tilemap wallTilemap;    //壁用Tilemap
    [SerializeField] private TileBase floorTreeTile; //木床タイル
    [SerializeField] private TileBase floorRockTile; //石床タイル
    [SerializeField] private TileBase floorWoodTile; //木床タイル
    [SerializeField] private TileBase floorIceTile;  //氷床タイル
    [SerializeField] private TileBase wallIronTile;  //鉄壁タイル
    [SerializeField] private TileBase wallMagmaTile; //マグマ壁タイル
    [SerializeField] private TileBase wallBrickTile; //レンガ壁タイル
    [SerializeField] private TileBase wallGoldTile;  //金壁タイル
    [SerializeField] private GameObject[] backStage; //ステージの背景
    [SerializeField] private GameObject enemyPrefab; //スライム配置
    [SerializeField] private GameObject batPrefab;   //コウモリ配置
    [SerializeField] private GameObject ghostPrefab; //おばけ配置
    [SerializeField] private GameObject devilPrefab; //デビル配置

    //他のスクリプトから読み取り専用でアクセスできるようにする
    public Tilemap FloorTilemap => floorTilemap;
    //public GameObject EnemyPrefab => enemyPrefab;

    private int index;

    public string csvFileName;                       //csvファイル名

    //シングルトンの初期化
    void Awake()
    {
        //最初に読み込まれた StageLoader をグローバルアクセス可能にする
        Instance = this;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        index = TitleManager.stageIndex;
        //LoadMapFromCSV(csvFileName);
        // ステージ番号に応じてファイル名を切り替える（例: "Stage0", "Stage1"...）
        csvFileName = "Stage" + index;
        LoadMapFromCSV(csvFileName);
        StageInit();
        StageBack();
    }

    void StageInit()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            for (int i = 0; i < backStage.Length; i++)
            {
                backStage[i].SetActive(false);
            }
        }
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            backStage[0].SetActive(false);
        }
    }

    void StageBack()
    {
        //Debug.Log(index);
        switch (index)
        {
            case 0:
                backStage[index].SetActive(true);
                break;
            case 1:
                backStage[index].SetActive(true);
                break;
            case 2:
                backStage[index].SetActive(true);
                break;
            case 3:
                backStage[index].SetActive(true);
                break;
            case 4:
                backStage[index].SetActive(true);
                break;
            case 5:
                backStage[index].SetActive(true);
                break;
        }
    }

    void LoadMapFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSVファイルが見つかりません: " + fileName);
            return;
        }

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        string[] lines = csvFile.text.Trim().Split('\n');

        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;
        Vector3 topLeftWorld = cam.transform.position;
        topLeftWorld.x -= camHalfWidth;
        topLeftWorld.y += camHalfHeight;

        Vector3Int topLeftCell = floorTilemap.WorldToCell(topLeftWorld);

        for (int y = 0; y < lines.Length; y++)
        {
            string[] values = lines[y].Trim().Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (int.TryParse(values[x], out int value))
                {
                    Vector3Int cellPos = new Vector3Int(topLeftCell.x + x, topLeftCell.y - y, 0);
                    Vector3 batPos = new Vector3(topLeftCell.x + x, topLeftCell.y - y + 0.5f, 0);

                    switch (value)
                    {
                        //0は何も置かない
                        //1～9はステージ用タイルマップ
                        //10～19は敵用

                        case 1:
                            floorTilemap.SetTile(cellPos, floorTreeTile);
                            break;
                        case 2:
                            wallTilemap.SetTile(cellPos, wallIronTile);
                            break;
                        case 3:
                            floorTilemap.SetTile(cellPos, floorRockTile);
                            break;
                        case 4:
                            wallTilemap.SetTile(cellPos, wallMagmaTile);
                            break;
                        case 5:
                            floorTilemap.SetTile(cellPos, floorWoodTile);
                            break;
                        case 6:
                            wallTilemap.SetTile(cellPos, wallBrickTile);
                            break;
                        case 7:
                            floorTilemap.SetTile(cellPos, floorIceTile);
                            break;
                        case 8:
                            wallTilemap.SetTile(cellPos, wallGoldTile);
                            break;
                        case 10:
                            Instantiate(enemyPrefab, cellPos, Quaternion.identity);
                            GameManager.enemyRest += 1;
                            break;
                        case 11:
                            Instantiate(batPrefab, batPos, Quaternion.identity);
                            break;
                        case 12:
                            Instantiate(ghostPrefab, cellPos, Quaternion.identity);
                            GameManager.enemyRest += 1;
                            break;
                        case 13:
                            Instantiate(devilPrefab, cellPos, Quaternion.identity);
                            GameManager.enemyRest += 1;
                            break;
                    }
                }
            }
        }
    }
}


/*using UnityEngine;
using UnityEngine.Tilemaps;

public class StageLoader : MonoBehaviour
{
    [SerializeField] private Transform player;      //プレイヤーのTransform
    [SerializeField] private Tilemap floorTilemap;  //床用Tilemap
    [SerializeField] private Tilemap wallTilemap;   //壁用Tilemap
    [SerializeField] private TileBase floorTile;    //床タイル
    [SerializeField] private TileBase wallTile;     //壁タイル
    public string csvFileName = "Stage";            //読み込むCSVファイル名（Resourcesフォルダ内）
    public int stageIndex = 3;                      //ステージ番号（高さオフセットに使用）

    public float stageHeightOffset = 100f;           //ステージ間の高さ差（ユニット）

    void Start()
    {
        Init();
    }

    void Init()
    {
        LoadMapFromCSV(csvFileName);

        // ステージ番号に応じてファイル名を切り替える（例: "Stage0", "Stage1"...）
        *//* csvFileName = "Stage" + stageIndex;
         LoadMapFromCSV(csvFileName);*//*
    }

    /// <summary>
    /// CSVファイルからマップを読み込み、Tilemapに反映する
    /// </summary>
    void LoadMapFromCSV(string fileName)
    {
        //CSVファイルをResourcesから読み込む
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSVファイルが見つかりません: " + fileName);
            return;
        }

        //既存のタイルを消去
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        //CSVの各行を取得
        string[] lines = csvFile.text.Trim().Split('\n');

        //カメラの左上のワールド座標を基準にしてマップを配置
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector3 topLeftWorld = cam.transform.position;
        topLeftWorld.x -= camHalfWidth;
        topLeftWorld.y += camHalfHeight;

        //ステージごとの高さを反映（上方向にずらす）
        topLeftWorld.y += stageHeightOffset * stageIndex;

        //左上ワールド座標をセル座標に変換
        Vector3Int topLeftCell = floorTilemap.WorldToCell(topLeftWorld);

        //CSVの内容をTilemapに設定
        for (int y = 0; y < lines.Length; y++)
        {
            string[] values = lines[y].Trim().Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (int.TryParse(values[x], out int value))
                {
                    //タイルを配置するセル座標
                    Vector3Int cellPos = new Vector3Int(topLeftCell.x + x, topLeftCell.y - y, 0);

                    switch (value)
                    {
                        case 1:
                            floorTilemap.SetTile(cellPos, floorTile);  //床タイルを配置
                            break;
                        case 2:
                            wallTilemap.SetTile(cellPos, wallTile);    //壁タイルを配置
                            break;
                            // 0やその他の値は何も配置しない
                    }
                }
            }
        }
    }
}
*/


/* void MoveCameraToTopLeft()
 {
     Camera cam = Camera.main;
     if (cam == null || !cam.orthographic) return;

     // Tilemapの左上セルのワールド座標を取得
     int width = tilemap.cellBounds.size.x;
     int height = tilemap.cellBounds.size.y;

     Vector3Int topLeftCell = new Vector3Int(0, height - 1, 0);
     Vector3 topLeftWorld = tilemap.CellToWorld(topLeftCell);

     // カメラの左上が topLeftWorld に一致するように調整
     float camHalfHeight = cam.orthographicSize;
     float camHalfWidth = camHalfHeight * cam.aspect;

     Vector3 camPos = new Vector3(
         topLeftWorld.x + camHalfWidth,
         topLeftWorld.y - camHalfHeight,
         -10f
     );

     cam.transform.position = camPos;
 }*/

