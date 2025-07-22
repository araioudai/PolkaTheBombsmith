using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koumori : MonoBehaviour
{
    #region �ϐ�
    [SerializeField] float speed = -5f; // �ړ����x
    int direction; // �ړ�����
    Vector3 scale; // �G�l�~�[�̃X�P�[�����擾

    // ���C�L���X�g�ϐ�
    Vector3 origin; // Ray�̊J�n�ʒu
    Vector3 dir;    // Ray�̕���
    float distance; // Ray�̋���
    #endregion

    #region �֐�
    /// <summary>
    /// �������֐�
    /// </summary>
    void Init()
    {
        direction = 1; // �X�^�[�g���̕������E�ɐݒ�
        scale = transform.localScale; // Enemy��Scale��ϐ��ɑ��
        distance = 1f; // Ray�̔򋗗�
    }

    /// <summary>
    /// Raycast�֐�
    /// </summary>
    void Raycast()
    {
        // Ray�̊J�n�ʒu�����ݒn�ɐݒ�
        origin = transform.localPosition;
        // Ray�̕������������ɐݒ� * �ړ�����
        dir = Vector2.left * direction;
        // Ray�̔򋗗���ݒ�
        distance = 0.5f;

        // Ray�����s(Wall���C���[��Ray�������邩����)
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, LayerMask.GetMask("Ground"));
        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, dir * distance, Color.green);
        // �ǂɓ��������ꍇ
        if (hit.collider != null)
        {
            // �i�s�����𔽓]
            direction *= -1;
            //Debug.Log("�ǂɓ�������");
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
