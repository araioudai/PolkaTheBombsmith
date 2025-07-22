using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public static StageLoader Instance { get; private set; } //���̃X�N���v�g���� StageLoader.Instance �ŃA�N�Z�X�ł���悤�ɂ���

    [SerializeField] private Tilemap floorTilemap;   //���pTilemap
    [SerializeField] private Tilemap wallTilemap;    //�ǗpTilemap
    [SerializeField] private TileBase floorTreeTile; //�؏��^�C��
    [SerializeField] private TileBase floorRockTile; //�Ώ��^�C��
    [SerializeField] private TileBase floorWoodTile; //�؏��^�C��
    [SerializeField] private TileBase floorIceTile;  //�X���^�C��
    [SerializeField] private TileBase wallIronTile;  //�S�ǃ^�C��
    [SerializeField] private TileBase wallMagmaTile; //�}�O�}�ǃ^�C��
    [SerializeField] private TileBase wallBrickTile; //�����K�ǃ^�C��
    [SerializeField] private TileBase wallGoldTile;  //���ǃ^�C��
    [SerializeField] private GameObject[] backStage; //�X�e�[�W�̔w�i
    [SerializeField] private GameObject enemyPrefab; //�X���C���z�u
    [SerializeField] private GameObject batPrefab;   //�R�E�����z�u
    [SerializeField] private GameObject ghostPrefab; //���΂��z�u
    [SerializeField] private GameObject devilPrefab; //�f�r���z�u

    //���̃X�N���v�g����ǂݎ���p�ŃA�N�Z�X�ł���悤�ɂ���
    public Tilemap FloorTilemap => floorTilemap;
    //public GameObject EnemyPrefab => enemyPrefab;

    private int index;

    public string csvFileName;                       //csv�t�@�C����

    //�V���O���g���̏�����
    void Awake()
    {
        //�ŏ��ɓǂݍ��܂ꂽ StageLoader ���O���[�o���A�N�Z�X�\�ɂ���
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
        // �X�e�[�W�ԍ��ɉ����ăt�@�C������؂�ւ���i��: "Stage0", "Stage1"...�j
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
            Debug.LogError("CSV�t�@�C����������܂���: " + fileName);
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
                        //0�͉����u���Ȃ�
                        //1�`9�̓X�e�[�W�p�^�C���}�b�v
                        //10�`19�͓G�p

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
    [SerializeField] private Transform player;      //�v���C���[��Transform
    [SerializeField] private Tilemap floorTilemap;  //���pTilemap
    [SerializeField] private Tilemap wallTilemap;   //�ǗpTilemap
    [SerializeField] private TileBase floorTile;    //���^�C��
    [SerializeField] private TileBase wallTile;     //�ǃ^�C��
    public string csvFileName = "Stage";            //�ǂݍ���CSV�t�@�C�����iResources�t�H���_���j
    public int stageIndex = 3;                      //�X�e�[�W�ԍ��i�����I�t�Z�b�g�Ɏg�p�j

    public float stageHeightOffset = 100f;           //�X�e�[�W�Ԃ̍������i���j�b�g�j

    void Start()
    {
        Init();
    }

    void Init()
    {
        LoadMapFromCSV(csvFileName);

        // �X�e�[�W�ԍ��ɉ����ăt�@�C������؂�ւ���i��: "Stage0", "Stage1"...�j
        *//* csvFileName = "Stage" + stageIndex;
         LoadMapFromCSV(csvFileName);*//*
    }

    /// <summary>
    /// CSV�t�@�C������}�b�v��ǂݍ��݁ATilemap�ɔ��f����
    /// </summary>
    void LoadMapFromCSV(string fileName)
    {
        //CSV�t�@�C����Resources����ǂݍ���
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV�t�@�C����������܂���: " + fileName);
            return;
        }

        //�����̃^�C��������
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        //CSV�̊e�s���擾
        string[] lines = csvFile.text.Trim().Split('\n');

        //�J�����̍���̃��[���h���W����ɂ��ă}�b�v��z�u
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector3 topLeftWorld = cam.transform.position;
        topLeftWorld.x -= camHalfWidth;
        topLeftWorld.y += camHalfHeight;

        //�X�e�[�W���Ƃ̍����𔽉f�i������ɂ��炷�j
        topLeftWorld.y += stageHeightOffset * stageIndex;

        //���ハ�[���h���W���Z�����W�ɕϊ�
        Vector3Int topLeftCell = floorTilemap.WorldToCell(topLeftWorld);

        //CSV�̓��e��Tilemap�ɐݒ�
        for (int y = 0; y < lines.Length; y++)
        {
            string[] values = lines[y].Trim().Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (int.TryParse(values[x], out int value))
                {
                    //�^�C����z�u����Z�����W
                    Vector3Int cellPos = new Vector3Int(topLeftCell.x + x, topLeftCell.y - y, 0);

                    switch (value)
                    {
                        case 1:
                            floorTilemap.SetTile(cellPos, floorTile);  //���^�C����z�u
                            break;
                        case 2:
                            wallTilemap.SetTile(cellPos, wallTile);    //�ǃ^�C����z�u
                            break;
                            // 0�₻�̑��̒l�͉����z�u���Ȃ�
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

     // Tilemap�̍���Z���̃��[���h���W���擾
     int width = tilemap.cellBounds.size.x;
     int height = tilemap.cellBounds.size.y;

     Vector3Int topLeftCell = new Vector3Int(0, height - 1, 0);
     Vector3 topLeftWorld = tilemap.CellToWorld(topLeftCell);

     // �J�����̍��オ topLeftWorld �Ɉ�v����悤�ɒ���
     float camHalfHeight = cam.orthographicSize;
     float camHalfWidth = camHalfHeight * cam.aspect;

     Vector3 camPos = new Vector3(
         topLeftWorld.x + camHalfWidth,
         topLeftWorld.y - camHalfHeight,
         -10f
     );

     cam.transform.position = camPos;
 }*/

