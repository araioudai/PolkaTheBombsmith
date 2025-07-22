/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] private GameManager manager; // �S�[�����B���ɃQ�[���N���A��m�点��}�l�[�W���[�Q��

    Rigidbody2D rb;                                       //Rigidbody2D ���������p
    Animator animator;
    Vector2 lscale;                                       //�����̐���

    [SerializeField] private GameObject bombCeiling;      //���e�v���t�@�u�V��p

    [SerializeField] private LayerMask groundChecklayer;  //�ڒn����p�̃��C���[
    [SerializeField] private LayerMask ceilingChecklayer; //
    private float dir;                                    //�v���C���[�̐i�s����
    private bool isGround;                                //�n�ʂɐڒn���Ă��邩
    private bool isCeiling;
    private bool isJump;                                  //�W�����v�����ǂ���
    private int count;                                    //�X�y�[�X�L�[�������ꂽ�񐔁i�f�o�b�O�p�j
    private float move;

    private float speed;                                  //���E�ړ����x
    private float jumpPow;                                //�W�����v�́iAddForce �̗́j
    private float jumpPowCeiling;                         //�V�䂪���鎞�̃W�����v��

    void Start()
    {
        Init();              //����������
        PlayerPos();         //�v���C���[�̃X�^�[�g�n�_
    }

    void Update()
    {
        PlayerMove();        //�v���C���[�̍��E�ړ�����
        PlayerJump();        //�W�����v����
        PlayerCeilingBomb(); //�V�䔚�e����
    }

    //����������
    void Init()
    {
        rb = GetComponent<Rigidbody2D>(); //Rigidbody2D�擾
        //bombCeiling = (GameObject)Resources.Load("Circle"); //Circle�v���n�u��GameObject�^�Ŏ擾
        animator = GetComponent<Animator>();
        lscale = transform.localScale;

        isGround = false;
        isCeiling = false;
        count = 0;
        speed = 2f;     //�ړ��i�K�v�ɉ����Ē����j
        jumpPow = 470f; //�W�����v��
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

    //�v���C���[�̍��E�ړ�
    void PlayerMove()
    {
        move = 0;
        //�E��� or D �L�[�ŉE�ړ�
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            lscale.x = 1f;
            move = 1f;
            
            animator.SetTrigger("Run");  //�����A�j���[�V�����Đ�
            if (isJump)
            {
                animator.SetTrigger("Idle"); //�~�܂��Ă���A�j���[�V�����Đ�
            }
        }
        //����� or A �L�[�ō��ړ�
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
            lscale.x = -1f;
            move = 1f;
            
            animator.SetTrigger("Run");  //�����A�j���[�V�����Đ�
            if (isJump)
            {
                animator.SetTrigger("Idle"); //�~�܂��Ă���A�j���[�V�����Đ�
            }
        }
        else
        {
            animator.SetTrigger("Idle"); //�~�܂��Ă���A�j���[�V�����Đ�
        }
        if (transform.position.x < -7.7)
        {
            transform.position = new Vector3(-7.7f, transform.position.y, transform.position.z);
        }
        transform.localScale = lscale;
    }

    //������Ray���΂��Ēn�ʂƂ̐ڒn������s��
    public bool IsGrounded()
    {
        float rayLength = 0.7f; //�����̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //��������Raycast�igroundChecklayer�ɓ���������ڒn�j
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.green);

        return hit.collider != null; //�n�ʂɓ���������true
    }

    private bool IsCeiling()
    {
        float rayLength = 3f; //�����̋���
        Vector2 origin = transform.position; //Ray�̎n�_�i�v���C���[�̈ʒu�j

        //��������Raycast�iceilingChecklayer�ɓ���������ڒn�j
        RaycastHit2D hitCeiling = Physics2D.Raycast(origin, Vector2.up, rayLength, ceilingChecklayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, Vector2.up * rayLength, Color.green);

        return hitCeiling.collider != null; //�n�ʂɓ���������true
    }

    //�W�����v����
    void PlayerJump()
    {
        isGround = IsGrounded(); //���t���[���A�n�ʂɂ��邩����

        //�X�y�[�X�L�[��������A���n�ʂɂ��āA�W�����v���łȂ��ꍇ
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJump && !isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPow)); //������ɗ͂�������
            isJump = true;                        //�W�����v���ɂ���
            Debug.Log("�W�����v�I");
        }
        //�X�y�[�X�L�[��������A���n�ʂɂ��āA�W�����v���łȂ��V�䂪��ɂ���ꍇ
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJump && isCeiling)
        {
            rb.AddForce(new Vector2(0, jumpPowCeiling)); //������ɗ͂�������
            isJump = true;                        //�W�����v���ɂ���
            Debug.Log("�W�����v�I");
        }

        //�X�y�[�X�L�[�������ꂽ�񐔂��L�^�i�f�o�b�O�p�j
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
            // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
            //Instantiate(bombCeiling, transform.position, Quaternion.identity);
            *//* Instantiate(bombCeiling, new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z), Quaternion.identity);
             Debug.Log(Mathf.RoundToInt(transform.position.x));*//*
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
            //Vector3 snappedPosition = new Vector3(snappedX, playerPos.y, playerPos.z);

            //���e�𐶐��i�X�i�b�v���ꂽ�ʒu�ɔz�u�j
            Instantiate(bombCeiling, snappedPosition, Quaternion.identity);

            Debug.Log($"���e�����i�X�i�b�v�ʒu�j: {snappedPosition}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;  //�W�����v�I��
            isGround = true; //�ڒn��
            count = 0;       //�J�E���g���Z�b�g�i�C�Ӂj
        }

    *//*    if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero; //���x���[���ɂ���
        }*//*
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }
}
*/