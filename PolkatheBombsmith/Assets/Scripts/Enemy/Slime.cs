using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    #region �ϐ�
    public TurnCheckGnd turnCheckGnd; // �n�ʔ���X�N���v�g

    // �ړ��ϐ�
    [SerializeField] float speed = -1f; // �ړ����x
    int direction; // �ړ�����
    Vector3 scale; // �G�l�~�[�̃X�P�[�����擾

    // ���C�L���X�g�ϐ�
    Vector3 origin; // Ray�̊J�n�ʒu
    Vector3 dir;    // Ray�̕���
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
        speed = -1f;
    }

    /// <summary>
    /// Raycast�֐�
    /// </summary>
/*    void Raycast()
    {
        // Ray�̊J�n�ʒu�����ݒn�ɐݒ�
        origin = transform.localPosition;
        // Ray�̕������������ɐݒ� * �ړ�����
        dir = Vector2.left * direction;
        // Ray�̔򋗗���ݒ�
        distance = 1f;

        // Ray�����s(Wall���C���[��Ray�������邩����)
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, LayerMask.GetMask("Wall"));

        // �ǂɓ��������ꍇ
        if (hit.collider != null)
        {
            // �i�s�����𔽓]
            direction *= -1;
            //Debug.Log("�ǂɓ�������");
        }
    }*/

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
        //Raycast();     // Raycast�֐�
        GroundCheck(); // �n�ʔ���֐�
        Move();        // �ړ��֘A�֐�
    }
    #endregion
}
