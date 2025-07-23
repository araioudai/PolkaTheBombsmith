using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerManager : MonoBehaviour
{
    #region �萔
    //�v���C���[�T�E���h��ԗp�萔
    private const int JUMP = 0;
    #endregion

    #region private�ϐ�
    Rigidbody2D rb;                                                             //Rigidbody2D ���������p
    Animator animator;                                                          //�A�j���[�V�����p
    PlayerAction controls;                                                      //InputSystem�p
    Vector2 moveInput;                                                          //�v���C���[�̈ړ��p
    float posY;
    private AudioSource audioSource;                                            //����炷�p�v���[���[
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>(); //player�I�[�f�B�I�p
    [SerializeField] private BomSelect bomSelect;                               //�{���Z���N�g�N���X�Q��
    [SerializeField] private GameObject bombCeiling;                            //���e�v���t�@�u�V��p
    [SerializeField] private LayerMask groundChecklayer;                        //���ڒn����p�̃��C���[
    [SerializeField] private LayerMask ceilingChecklayer;                       //�ǐڒn����p�̃��C���[
    [SerializeField] private float speed;                                       //�v���C���[�̈ړ��X�s�[�h
    [SerializeField] private float bufferTime;                                  //��s���̓W�����v�^�C�}�[�Z�b�g�p
    //private Transform cameraScale;
    private float jumpBufferTimer;                                              //��s���̓W�����v�^�C�}�[
    private bool jumpBuffered;                                                  //��s���̓W�����v�p�t���O
    private bool isGround;                                                      //�n�ʂɐڒn���Ă��邩
    private bool isCeiling;                                                     //�V�䂪��ɂ��邩
    private bool isJump;                                                        //�W�����v�����ǂ���
    private float jumpPow;                                                      //�W�����v�́iAddForce �̗́j
    private float jumpPowCeiling;                                               //�V�䂪���鎞�̃W�����v��
    private float interval;                                                     //���e��u���C���^�[�o��          
    private float setInterval;                                                  //���e�C���^�[�o���Z�b�g�p
    #endregion

    #region public�ϐ�
    public int generate;                                                        //���e���������
    public bool isGenerate;                                                     //�����������̃t���O
    public bool gameOver;                                                       //�Q�[���I�[�o�[�p
    #endregion

    //public static int bombType;

    #region Unity�C�x���g
    // Start is called before the first frame update
    void Start()
    {
        Init();
        PlayerPos();         //�v���C���[�̃X�^�[�g�n�_
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

    #region Start�Ăяo���֐�
    void Init()
    {
        rb = GetComponent<Rigidbody2D>(); //Rigidbody2D�擾
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
        speed = 4f;     //�ړ�
        jumpPow = 650f; //�W�����v��
        jumpPowCeiling = 400f;
        interval = 0;
        setInterval = 1f;
        //generate = 5;
    }

    //�v���C���[�̏������W
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

    #region Update�Ăяo���֐�
    //�v���C���[�ړ�
    void Move()
    {
        var move = new Vector3(moveInput.x * speed * Time.deltaTime, 0, 0);
        // �����̐���F�ړ������ɉ����č��E���]
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);  // �E����
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z); // ������
        }

        // ���ۂ̈ړ�
        transform.Translate(move);

        // �A�j���[�V����
        if (moveInput.x == 0 || isJump == true)
        {
            animator.SetTrigger("Idle"); //�~�܂��Ă���A�j���[�V�����Đ�
        }
        else
        {
            animator.SetTrigger("Run");  //�����A�j���[�V�����Đ�
        }
    }

    void OnMovePerformrd(InputAction.CallbackContext context)
    {
        //���͎��̓���
        moveInput = context.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext context)
    {
        //�������Ƃ��̓���
        moveInput = Vector2.zero;
    }

    //���e�U��
    void Attack(InputAction.CallbackContext context)
    {
        isCeiling = IsCeiling();
        if (isCeiling && generate > 0 && bomSelect.sele == 1 && interval == 0)
        {
            float gridSize = 1.0f; // �^�C���}�b�v��1�}�X�̃T�C�Y�i1���j�b�g = 1�^�C���j

            Vector3 playerPos = transform.position; // �v���C���[�̌��݈ʒu���擾

            //X���W��Y���W���A���ꂼ�ꌻ�݂���}�X�̍����ɃX�i�b�v(�}�X�ڂɃs�^�b�Ƒ�����)
            float snappedX = Mathf.Floor(playerPos.x / gridSize) * gridSize;

            float snappedY = Mathf.Floor(playerPos.y / gridSize) * gridSize;

            //�}�X�̒����ɗ���悤�ɁA���ꂼ�ꔼ�}�X���igridSize / 2�j�����Z
            snappedX += gridSize / 2f;
            snappedY += gridSize / 2f;

            //�X�i�b�v��̈ʒu�x�N�g�����쐬�iZ�͂��̂܂܁j
            Vector3 snappedPosition = new Vector3(snappedX, snappedY, playerPos.z);

            //���e�𐶐��i�X�i�b�v���ꂽ�ʒu�ɔz�u�j
            GameObject bomb = Instantiate(bombCeiling, snappedPosition, Quaternion.identity);
            bomb.GetComponent<BomManager>().myBombType = BomSelect.seles;
            //bombPos = snappedPosition;

            generate -= 1;
            isGenerate = true;
            interval = setInterval;

            //Debug.Log($"���e�����i�X�i�b�v�ʒu�j: {snappedPosition}");
        }
        else if(generate > 0 && bomSelect.sele != 1 && interval == 0)
        {
            float gridSize = 1.0f; // �^�C���}�b�v��1�}�X�̃T�C�Y�i1���j�b�g = 1�^�C���j

            Vector3 playerPos = transform.position; // �v���C���[�̌��݈ʒu���擾

            //X���W��Y���W���A���ꂼ�ꌻ�݂���}�X�̍����ɃX�i�b�v(�}�X�ڂɃs�^�b�Ƒ�����)
            float snappedX = Mathf.Floor(playerPos.x / gridSize) * gridSize;

            float snappedY = Mathf.Floor(playerPos.y / gridSize) * gridSize;

            //�}�X�̒����ɗ���悤�ɁA���ꂼ�ꔼ�}�X���igridSize / 2�j�����Z
            snappedX += gridSize / 2f;
            snappedY += gridSize / 2f;

            //�X�i�b�v��̈ʒu�x�N�g�����쐬�iZ�͂��̂܂܁j
            Vector3 snappedPosition = new Vector3(snappedX, snappedY, playerPos.z);

            //���e�𐶐��i�X�i�b�v���ꂽ�ʒu�ɔz�u�j
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

    //������Ray���΂��Ēn�ʂƂ̐ڒn������s��
    private bool IsGrounded()
    {
        float rayLength = 0.5f; //�����̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //��������Raycast�igroundChecklayer�ɓ���������ڒn�j
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //�n�ʂɓ���������true
    }

    //�V�䂪���邩�Ȃ����`�F�b�N�p
    private bool IsCeiling()
    {
        float rayLength = 3f; //��̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //�������Raycast�iceilingChecklayer�ɓ���������ڒn�j
        RaycastHit2D hitCeiling = Physics2D.Raycast(origin, Vector2.up, rayLength, ceilingChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hitCeiling.collider != null; //�n�ʂɓ���������true
    }

    //�W�����v����
    void Jump(InputAction.CallbackContext context)
    {
        jumpBuffered = true;
        jumpBufferTimer = bufferTime;
        /*isGround = IsGrounded(); //���t���[���A�n�ʂɂ��邩����
        isCeiling = IsCeiling(); //�V��̔���������ōX�V

        //�X�y�[�X�L�[��������A���n�ʂɂ��āA�W�����v���łȂ��ꍇ
        if (isGround && !isJump && !isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPow)); //������ɗ͂�������
            isJump = true;                        //�W�����v���ɂ���
            Debug.Log("�W�����v�I");
        }
        //�X�y�[�X�L�[��������A���n�ʂɂ��āA�W�����v���łȂ��V�䂪��ɂ���ꍇ
        if (isGround && !isJump && isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPowCeiling)); //������ɗ͂�������
            isJump = true;                        //�W�����v���ɂ���
            Debug.Log("�W�����v�I");
        }*/
    }

    void TypeAheadJump()
    {
        isGround = IsGrounded();  //�ڒn����
        isCeiling = IsCeiling();  //�V�䔻��

        //��s���͂���W�����v����
        if (jumpBuffered)//�o�b�t�@����W�����v�{�^���������ꂽ���
        {
            jumpBufferTimer -= Time.deltaTime;

            if (isGround && !isJump)
            {
                float jumpForce = isCeiling ? jumpPowCeiling : jumpPow; //�V�䂪���邩�ǂ����ŃW�����v�͂�ς���
                rb.AddForce(new Vector2(0, jumpForce));
                isJump = true;
                jumpBuffered = false;                                   //�o�b�t�@�������i�W�����v�����j
                audioSource.PlayOneShot(audioClip[JUMP]);

                //Debug.Log("�W�����v���s�I");
            }

            //�o�b�t�@���Ԃ��߂����疳����
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
        //�uPlayer�v�A�N�V�����́uMove�v�A�N�V�����}�b�v��L��
        controls.Player.Move.Enable();
        controls.Player.Jump.Enable();
        controls.Player.Attack.Enable();

        //���͎��ɌĂ΂��C�x���g��o�^
        controls.Player.Move.performed += OnMovePerformrd; //���͎�
        controls.Player.Move.canceled += OnMoveCanceled;   //��������

        //�W�����v
        controls.Player.Jump.performed += Jump;            //���͎�

        //�U��
        controls.Player.Attack.performed += Attack;        //���͎�
    }

    private void OnDisable()
    {
        //���͎��ɌĂ΂��C�x���g���폜
        controls.Player.Move.performed -= OnMovePerformrd; //���͎�
        controls.Player.Move.canceled -= OnMoveCanceled;   //��������

        //�W�����v
        controls.Player.Jump.performed -= Jump;            //���͎�

        //�U��
        controls.Player.Attack.performed -= Attack;        //���͎�

        //�uPlayer�v�A�N�V�����́uMove�v�A�N�V�����}�b�v�𖳌�
        controls.Player.Move.Disable();
        controls.Player.Jump.Disable();
        controls.Player.Attack.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;  //�W�����v�I��
            isGround = true; //�ڒn��
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero; //���x���[���ɂ���
        }
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBat") || collision.gameObject.CompareTag("EnemyDevil") || collision.gameObject.CompareTag("EnemyGhost") || collision.gameObject.CompareTag("GameOver"))
        {
            gameOver = true;
        }
    }
    #endregion
}
