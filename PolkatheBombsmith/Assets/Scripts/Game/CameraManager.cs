using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;           // �Ǐ]�Ώہi�v���C���[��Transform�j
    public Vector2 deadZone = new Vector2(2f, 1.5f); // �f�b�h�]�[����X�EY�͈́i���S����̋����j
    public float followSpeed = 5f;     // �Ǐ]���x�iLerp�̌W���j

    void LateUpdate()
    {
        if (target == null) return;   // �Ǐ]�Ώۂ��Z�b�g����Ă��Ȃ���Ώ����I��

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;

        // ------ X ���̏��� ------
        float dx = tpos.x - pos.x;
        if (Mathf.Abs(dx) > deadZone.x)
        {
            // �f�b�h�]�[���O�ɏo�Ă���ꍇ�̂�Lerp�Ŋ��炩�ɒǏ]
            pos.x = Mathf.Lerp(pos.x,
                               tpos.x - Mathf.Sign(dx) * deadZone.x,
                               followSpeed * Time.deltaTime);
        }

        // ------ Y ���̏��� ------
        float dy = tpos.y - pos.y;
        if (Mathf.Abs(dy) > deadZone.y)
        {
            pos.y = Mathf.Lerp(pos.y,
                               tpos.y - Mathf.Sign(dy) * deadZone.y,
                               followSpeed * Time.deltaTime);
        }
        // �J�����ʒu���X�V�iZ���͈ێ��j
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}