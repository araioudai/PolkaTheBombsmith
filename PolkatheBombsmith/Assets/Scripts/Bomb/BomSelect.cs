using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BomSelect : MonoBehaviour
{
    PlayerAction controls;                                          //InputSystem�p
    public int sele;//�z��̒����w�肷�邽�߂̕ϐ�
    public string[] Boms = { "�ʏ�", "�V��", "��", "�n��" ,"����" };//�{���̎�ނɔz��
    public static int seles;
    //public GameObject text;//�e�L�X�g�\���p

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Select();
    }

    void Init()
    {
        sele = 0;
        seles = sele;
    }

    void Select()
    {
        float wh = Input.GetAxis("Mouse ScrollWheel");//�}�E�X�z�C�[�����������o���̂Ő�����int�^�ɕϊ�����p
        sele += (int)(wh * 10);
        //�F�̕ύX
        if (sele >= Boms.Length)
        {
            sele = 0;
        }
        if (sele < 0)
        {
            sele = Boms.Length - 1;
        }
        //Debug.Log(sele);
        if (sele >= 0 && sele < Boms.Length)
        {
            //Debug.Log(Boms[sele]); // �R���\�[���ɕ\��
            //text.GetComponent<Text>().text = Boms[sele];
            // �܂��́AUI�e�L�X�g�ɕ\��
            // GetComponent<TextMesh>().text = words[indexToShow];
        }
    }

    void SelectUp (InputAction.CallbackContext context)
    {
        sele += 1;
        seles = sele;
    }

    void SelectDown(InputAction.CallbackContext context)
    {
        sele -= 1;
        seles = sele;
    }

    private void Awake()
    {
        controls = new PlayerAction();
    }

    private void OnEnable()
    {
        //�uBomd�v�A�N�V�����́uUp.Down�v�A�N�V�����}�b�v��L��
        controls.Bomb.Up.Enable();
        controls.Bomb.Down.Enable();

        //���e��ސ؂�ւ�
        controls.Bomb.Up.performed += SelectUp;            //���͎�
        controls.Bomb.Down.performed += SelectDown;        //���͎�
    }

    private void OnDisable()
    {
        //���e��ސ؂�ւ�
        controls.Bomb.Up.performed -= SelectUp;            //���͎�
        controls.Bomb.Down.performed -= SelectDown;        //���͎�

        //�uBomd�v�A�N�V�����́uUp.Down�v�A�N�V�����}�b�v�𖳌�
        controls.Bomb.Up.Disable();
        controls.Bomb.Down.Disable();
    }
}
