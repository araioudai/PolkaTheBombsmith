using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    PlayerAction controls;
    [SerializeField] private Transform target;                         //�Ǐ]�Ώہi�v���C���[��Transform�j
    [SerializeField] private Vector2 deadZone = new Vector2(2f, 1.5f); //�f�b�h�]�[����X�EY�͈́i���S����̋����j
    [SerializeField] private float followSpeed = 5f;                   //�Ǐ]���x�iLerp�̌W���j
    private Camera maincam;                                            //�J�����g��k���p

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

    void Start()
    {
        Init();
    }
    void Init()
    {
        maincam = GetComponent<Camera>();
    }

    void CameraExpansion(InputAction.CallbackContext context)
    {
        if (maincam.orthographicSize > 5f)
        {
            maincam.orthographicSize -= 5f;
            /*float target = maincam.orthographicSize - 5f;
            while (target < maincam.orthographicSize)
            {
                maincam.orthographicSize -= Time.deltaTime;
            }*/
        }
        //cameraScale.transform.localScale = Vector3.one / 2;
        //Debug.Log("�k��");
    }

    void CameraReduction(InputAction.CallbackContext context)
    {
        if (maincam.orthographicSize < 20f)
        {
            maincam.orthographicSize += 5f;
            /* float target = maincam.orthographicSize + 5f;
             while(target > maincam.orthographicSize)
             {
                 maincam.orthographicSize += Time.deltaTime;
             }*/
        }
        //cameraScale.transform.localScale = Vector3.one * 2;
        //Debug.Log("�g��");
    }

    private void Awake()
    {
        controls = new PlayerAction();
    }

    private void OnEnable()
    {
        //�uCamera�v�A�N�V�����̃A�N�V�����}�b�v��L��
        controls.Camera.CameraExpansion.Enable();
        controls.Camera.CameraReduction.Enable();

        //�J�����g��
        controls.Camera.CameraExpansion.performed += CameraExpansion;

        //�J�����k��
        controls.Camera.CameraReduction.performed += CameraReduction;
    }
    private void OnDisable()
    {
        //�J�����g��
        controls.Camera.CameraExpansion.performed -= CameraExpansion;

        //�J�����k��
        controls.Camera.CameraReduction.performed -= CameraReduction;

        //�uCamera�v�A�N�V�����̃A�N�V�����}�b�v�𖳌�
        controls.Camera.CameraExpansion.Disable();
        controls.Camera.CameraReduction.Disable();
    }
}