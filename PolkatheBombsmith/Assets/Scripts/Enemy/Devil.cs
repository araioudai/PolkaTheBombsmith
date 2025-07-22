using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    #region �ϐ�
    public TurnCheckGnd turnCheckGnd; // �n�ʔ���X�N���v�g

    // �R���|�[�l���g�̎擾
    Rigidbody2D rb;

    // �W�����v�ϐ�
    [SerializeField] float jumpForce = 300f; // �W�����v�p���[
    [SerializeField] float jumpInterval = 2f; // �W�����v����Ԋu
    float jumpTimer; //�@�W�����v�^�C�}�[

    // �ړ��ϐ�
    [SerializeField] float speed = -5f; // �ړ����x
    int direction; // �ړ�����
    Vector3 scale; // �G�l�~�[�̃X�P�[�����擾

    // ���C�L���X�g�ϐ�
    Vector3 origin;       // Ray�̊J�n�ʒu
    Vector3 dirWall;      // Ray�̕���(��)
    Vector3 dirDown;      // Ray�̕���(�^��)
    Vector3 dirLeftDown;  // Ray�̕���(���΂߉�)
    Vector2 dirRightDown; // Ray�̕���(�E�΂߉�)
    float distance; // Ray�̋���
    #endregion

    #region�@�֐�
    /// <summary>
    /// �������֐�
    /// </summary>
    void Init()
    {
        direction = 1; // �X�^�[�g���̕������E�ɐݒ�
        scale = transform.localScale; // Enemy��Scale��ϐ��ɑ��
        distance = 1f; // Ray�̔򋗗�
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ���擾
        jumpTimer = 0; // �^�C�}�[�̏����l��0�ɐݒ�
    }

    /// <summary>
    /// Raycast�֐�
    /// </summary>
    void Raycast()
    {
        // Ray�̊J�n�ʒu�����ݒn�ɐݒ�
        origin = transform.localPosition;
        // Ray�̕������������ɐݒ� * �ړ�����
        dirWall = Vector2.left * direction;
        // Ray�̕������������ɐݒ�
        dirDown = Vector2.down;
        // Ray�̕��������΂߉������ɐݒ�A���K��
        dirLeftDown = (Vector2.down + Vector2.left).normalized;
        // Ray�̕������E�΂߉������ɐݒ�A���K��
        dirRightDown = (Vector2.down + Vector2.right).normalized;
        // Ray�̔򋗗���ݒ�
        distance = 1f;

        // Ray�����s(Wall���C���[��Ray�������邩����)
        RaycastHit2D hitWall = Physics2D.Raycast(origin, dirWall, distance, LayerMask.GetMask("Ground"));
        // ���O������Ray�����s(Ground���C���[��Ray�������邩����)
        RaycastHit2D hitDown = Physics2D.Raycast(origin, dirDown, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitLeftDown = Physics2D.Raycast(origin, dirLeftDown, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRightDown = Physics2D.Raycast(origin, dirRightDown, distance, LayerMask.GetMask("Ground"));

        // �ǂɓ��������ꍇ
        if (hitWall.collider != null)
        {
            // �i�s�����𔽓]
            direction *= -1;
            //Debug.Log("�ǂɓ�������");
        }

        // �n�ʂɐG��Ă����ꍇ
        if (hitDown.collider != null || hitLeftDown.collider != null || hitRightDown.collider != null)
        {
            Jump(); // �W�����v�����s
            GroundCheck(); // ���]����
        }
    }

    /// <summary>
    /// �n�ʔ���֐�
    /// </summary>
    void GroundCheck()
    {
        // �n�ʂ̒[���ǂ�������
        if (turnCheckGnd.turnPointGnd)
        {
            direction *= -1; // �����𔽓]
            turnCheckGnd.turnPointGnd = false; // �t���O�����Z�b�g
        }
    }

    /// <summary>
    /// �W�����v�֐�
    /// </summary>
    void Jump()
    {
        jumpTimer += Time.deltaTime; //jumpTimer�ɖ��t���[���v���X����

        // jumpTimer��jumpInterval�ȏ�ɂȂ�����W�����v����
        if (jumpTimer >= jumpInterval)
        {
            rb.AddForce(Vector2.up * jumpForce); // jumpForce����ɏオ��
            jumpTimer = 0f; // �^�C�}�[�����Z�b�g
            //Debug.Log("�W�����v");
        }
    }

    /// <summary>
    /// �ړ��֘A�֐�
    /// </summary>
    void Move()
    {
        // OnTriggerExit2D����������Enemy�̌�����-1��������
        transform.localScale = new Vector3(scale.x * direction, scale.y, scale.x);
        // OnTriggerExit2D����������Enemy�̈ړ��x�N�g����-1������
        transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);

        // ���]�������ǂ����̃f�o�b�O
        if (scale.x > 0)
        {
            //Debug.Log("���]����");
        }
    }
    #endregion

    #region Unity�C�x���g
    void Start()
    {
        Init(); // �������֐�
    }

    void Update()
    {
        Raycast();     // Raycast�֐�
        Move();        // �ړ��֘A�֐�
    }
    #endregion
}
