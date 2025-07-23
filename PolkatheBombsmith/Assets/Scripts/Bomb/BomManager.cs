using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BomManager : MonoBehaviour
{
    //�萔:�G���S�T�E���h�p
    private const int SLIME = 0;
    private const int BAT = 1;
    private const int GHOST = 2;
    private const int DEVIL = 3;

    private Vector3 bombPos;                             //���e�̃|�W�V����
    private Rigidbody2D rb;
    private CircleCollider2D circle;
    private BomSelect bomSelect;
    private SpriteRenderer bombSprite;
    private AudioSource audioSource;                     //����炷�p�v���[���[
    [SerializeField] private LayerMask groundChecklayer; //���ڒn����p�̃��C���[
    [SerializeField] private AudioClip bombSound;        //�I�[�f�B�I�Z�b�g�p
    [SerializeField] private AudioClip[] enemyDead;      //�G��|�������z��
    [SerializeField] private AudioClip ghostDead;        //���΂��|������
    [SerializeField] private AudioClip devilDead;        //�f�r���|������
    [SerializeField] private GameObject bombPrefab;      //���e�G�t�F�N�g�p
    [SerializeField] private GameObject bomb;            //���e���g�擾 
    [SerializeField] private GameObject blockPrefab;     //�u���b�N�j��G�t�F�N�g�p
    [SerializeField] private Sprite[] bombType;          //���e�^�C�v�摜�؂�ւ��\���p
    //[SerializeField] private List<AudioClip> audioClip = new List<AudioClip>(); //�I�[�f�B�I�Z�b�g�p
    private float timer;                                 //��������
    private float timerBlock;
    private float timerSet;
    private int bomNo = 0;                               //���e���
    private bool isCeiling;                              //�V��p�t���O
    //private bool isGrounded;                             //�n�ʗp�t���O
    private bool exploded;                               //�����ς݃t���O
    private float count;
    private float setCount;

    public int myBombType;                               //�v���C���[����󂯎�����ʂ̔��e�^�C�v

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
        if (myBombType == 0)//�ʏ�
        {
            timer = 2f;
            rb.gravityScale = 1f;
            bomNo = 0;
            bombSprite.sprite = bombType[0];
        }
        else if (myBombType == 1)//�V��
        {
            timer = 2f;
            rb.gravityScale = -1f;
            bomNo = 1;
            bombSprite.sprite = bombType[1];
        }
        else if (myBombType == 2)//��
        {
            timer = 2f;
            rb.gravityScale = 1f;
            transform.Rotate(0, 0, 180);
            bomNo = 2;
            bombSprite.sprite = bombType[1];
        }
        else if (myBombType == 3)//�n��
        {
            timer = 1000f;
            rb.gravityScale = 1f;
            colliderTest.enabled = true;
            bomNo = 3;
            bombSprite.sprite = bombType[0];
        }
        else if (myBombType == 4)//����
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
        //StageLoader�̃C���X�^���X�o�R�ŃA�N�Z�X
        Tilemap tilemap = StageLoader.Instance.FloorTilemap; //Tilemap�ɃA�N�Z�X
        //GameObject enemy = StageLoader.Instance.EnemyPrefab; //EnemyPrefab�ɃA�N�Z�X

        //���݂̔��e�̃��[���h���W���ATilemap �̃Z�����W�i�}�X�ځj�ɕϊ�
        Vector3Int baseCell = tilemap.WorldToCell(transform.position);

        //�\�������i��A���A�E�A���A�����j
        //���e�𒆐S�ɁA�㉺���E�{�������g��5�}�X���̑��Έʒu���w��
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),  //���S
            new Vector3Int(0, 1, 0),  //��
            new Vector3Int(0, -1, 0), //��
            new Vector3Int(1, 0, 0),  //�E
            new Vector3Int(-1, 0, 0)  //��
        };

        //foreach�́A�u�z��⃊�X�g�̂��ׂĂ̗v�f��1���A�N�Z�X�������Ƃ��ɕ֗��vfor�����V���v���ň��S
        /*�e���v��
        foreach (�^ �ϐ��� in �R���N�V����)
        {
            // �ϐ��� ���g��������
        }
        /*�E�^�F�z��̒��̗v�f�̌^�i��Fint, Vector3Int�Ȃǁj

          �E�ϐ����F���̃��[�v���Ɏg���ϐ��̖��O

          �E�R���N�V�����F�z��⃊�X�g�Ȃǁi��Fdirections�j*/
        foreach (var dir in directions)
        {
            //��Z���ʒu�ibaseCell�j�ɕ����������āA�U���ΏۃZ��������
            Vector3Int targetCell = baseCell + dir;

            //�ΏۃZ���Ƀ^�C�������銎�A���e�^�C�v���V��p�����p�̎��ɔj��
            if (tilemap.HasTile(targetCell) && (myBombType == 1 || myBombType == 2))
            {
                timerBlock = 0;
                //�^�C�����폜�inull ���Z�b�g�j���j��
                tilemap.SetTile(targetCell, null);
                Vector3 pos = new Vector3(targetCell.x + 0.5f, targetCell.y + 1f, targetCell.z);
                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);
                Destroy(block, 1f);

/*                timerBlock = timerSet;
                if (timerBlock <= 0)
                {
                    timerBlock = 0;
                    //�^�C�����폜�inull ���Z�b�g�j���j��
                    tilemap.SetTile(targetCell, null);
                    Vector3 pos = new Vector3(targetCell.x + 0.5f, targetCell.y + 1f, targetCell.z);
                    GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);
                    Destroy(block, 1f);
                }*/
            }
            //�͈͓��ɓG�����銎�A���e���ʏ�p�̎��|���i�폜�j
            /*if (enemy.targetCell && myBombType == 0)
            {

            }*/
            if (myBombType == 0)
            {
                Vector2 worldPos = tilemap.GetCellCenterWorld(targetCell);
                float radius = 0.2f; //�������l�Ńs���|�C���g�ɂ���

                Collider2D hit = Physics2D.OverlapCircle(worldPos, radius, LayerMask.GetMask("Enemy")); // "Enemy" ���C���[�݂̂ɔ���

                if (hit != null && hit.CompareTag("Enemy"))
                {
                    Destroy(hit.gameObject); //�G��|��
                    audioSource.PlayOneShot(enemyDead[SLIME]);
                    GameManager.enemyRest -= 1;
                }
                if (hit != null && hit.CompareTag("EnemyGhost"))
                {
                    Destroy(hit.gameObject); //�G��|��
                    //�� to do
                    audioSource.PlayOneShot(enemyDead[GHOST]);
                    GameManager.enemyRest -= 1;
                }
                if (hit != null && hit.CompareTag("EnemyDevil"))
                {
                    Destroy(hit.gameObject); //�G��|��
                    //�� to do
                    audioSource.PlayOneShot(enemyDead[DEVIL]);
                    GameManager.enemyRest -= 1;
                }
            }
            if (myBombType == 1)
            {
                Vector2 worldPos = tilemap.GetCellCenterWorld(targetCell);
                float radius = 0.2f; //�������l�Ńs���|�C���g�ɂ���

                Collider2D hit = Physics2D.OverlapCircle(worldPos, radius, LayerMask.GetMask("Enemy")); // "Enemy" ���C���[�݂̂ɔ���
                if (hit != null && hit.CompareTag("EnemyBat"))
                {
                    Destroy(hit.gameObject); //�G��|��
                    //�� to do
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
            exploded = true;                           //1�����
            timer = 0;                                 //�^�C�}�[��0�ȉ��ɂ��Ȃ��悤�ɃZ�b�g
            //Explode();
            StartCoroutine(ExplodePlace());            //���������i���e�^�C�v���V��p�����p�̎�Tilemap�j��j
            bombSprite.enabled = false;                //���e���\���ɂ���
            if(myBombType != 1)circle.enabled = false; //�����蔻�������
            audioSource.PlayOneShot(bombSound);        //������
            GameObject bomb = Instantiate(bombPrefab, bombPos, Quaternion.identity);
            Destroy(bomb, 1f);
            Destroy(gameObject, 3f);                   //��\���ɂ������e�폜
        }
    }

    /*    IEnumerator DestroyAfterSound(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }*/

    //������Ray���΂��Ēn�ʂƂ̐ڒn������s��
    private bool IsCeiling()
    {
        float rayLength = 0.7f; //��̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //��������Raycast�igroundChecklayer�ɓ���������ڒn�j
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLength, groundChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hit.collider != null; //�V��ɓ���������true
    }

/*    //������Ray���΂��Ēn�ʂƂ̐ڒn������s��
    private bool IsGrounded()
    {
        float rayLength = 0.4f; //�����̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //��������Raycast�igroundChecklayer�ɓ���������ڒn�j
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //�n�ʂɓ���������true
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
                Destroy(collider.gameObject); //�G��|��
                audioSource.PlayOneShot(enemyDead[SLIME]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     Destroy(collider.gameObject); //�G��|��
                     audioSource.PlayOneShot(enemyDead[SLIME]);
                     count = 0;
                 }*/
            }
            if (collider.gameObject.CompareTag("EnemyGhost"))
            {
                timer = 0;
                Destroy(collider.gameObject); //�G��|��
                audioSource.PlayOneShot(enemyDead[GHOST]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     count = 0;
                     Destroy(collider.gameObject); //�G��|��
                     audioSource.PlayOneShot(enemyDead[GHOST]);
                 }*/
            }
            if (collider.gameObject.CompareTag("EnemyDevil"))
            {
                timer = 0;
                Destroy(collider.gameObject); //�G��|��
                audioSource.PlayOneShot(enemyDead[DEVIL]);
                GameManager.enemyRest -= 1;
                /* count = setCount;
                 if (count <= 0)
                 {
                     count = 0;
                     Destroy(collider.gameObject); //�G��|��
                     audioSource.PlayOneShot(enemyDead[DEVIL]);
                 }*/
            }
        }
    }
}
