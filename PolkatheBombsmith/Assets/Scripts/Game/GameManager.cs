using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private AudioSource audioSource;                                                   //����炷�p�v���[���[
    //[SerializeField] private BomSelect select;
    [SerializeField] private PlayerManager player;                                     //PlayerManager�Q��
    [SerializeField] private GameObject virtualPad;                                    //�o�[�`�����p�b�h(�\���E��\��)
    [SerializeField] private GameObject bombType;                                      //�o�[�`�����p�b�hoff�̎����e�̎�ޕ\��
    [SerializeField] private GameObject multiple;                                      //�c�萔(�\���E��\��)
    [SerializeField] private GameObject attackButton;                                  //�A�^�b�N�{�^��(�\���E��\��)
    [SerializeField] private GameObject gameClearPanel;                                //�Q�[���N���A���p�l��(�\���E��\��)
    [SerializeField] private GameObject gameOverPanel;                                 //�Q�[���I�[�o�[���p�l��(�\���E��\��)
    [SerializeField] private Text enemyRestText;                                       //�c��G��
    [SerializeField] private Text textMultiple;                                        //�c�蔚�e��
    [SerializeField] private Text nowBomb;                                             //���݂̔��e�^�C�v
    [SerializeField] private Text textBombType;                                        //�o�[�`�����p�b�hoff�̎����e�̎�ރe�L�X�g
    [SerializeField] private GameObject currentSelected;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private string[] bomb = { "�ʏ�", "�V��", "�n��", "�n��", "����" }; //�{���̎�ނɔz��
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>();        //�I�[�f�B�I�Z�b�g�p
    //[SerializeField] private AudioClip bombSound;
    private float m_time;     //�c�莞�ԁF�^�C���A�^�b�N�p
    private bool startTimer;  //true�̎��^�C���A�^�b�N=�Q�[���I�[�o�[�ɂȂ�i���̃��[�h��0�ŃQ�[���I�[�o�[�ɂȂ�Ȃ��p�j
    private bool displayTime; //�e�L�X�g�`��^�C��
    private int m_rest;       //�c�蔚�e���F���e�����X�e�[�W�p
    private bool startRest;   //true�̎����e��������=�Q�[���I�[�o�[�ɂȂ�i���̃��[�h��0�ŃQ�[���I�[�o�[�ɂȂ�Ȃ��p�j
    private bool displayRest; //�e�L�X�g�`�攚�e��������
    private bool gameClear;   //�Q�[���N���A�p
    private bool clearChack;  //1�񂾂��X�e�[�W���
    private int reduce;       //�c�蔚�e���炷�p
    
    public static int enemyRest;    //�G�̐�
    public static int stageClear;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        StageBgmStart();
    }

    // Update is called once per frame
    void Update()
    {
        TimeAttack();
        if(displayTime) DisplayTimer();
        if(displayRest) DisplayRest();
        DisplayEnemy();
        AttackText();
        GameState();
        if(TitleManager.virtualPad == false)
        {
            UpdateSelectUI();
        }
        //AttackEnd();
        //Debug.Log(BomSelect.seles);
    }

    void Init()
    {
        //to do�����ŃG�l�~�[�̃}�b�N�X��錾���� or BomManager�ł��
        switch (TitleManager.stageIndex) {
            case 0:
                displayRest = true;
                SetRest(false, 10);
                break;
            case 1:
                displayRest = true;
                SetRest(false, 50);
                break;
            case 2:
                displayRest = true;
                SetRest(false, 40);
                break;
            case 3:
                textMultiple.fontSize = 100;
                displayTime = true;
                SetTimer(true, 60);
                SetRest(false, 1000);
                break;
            case 4:
                displayRest = true;
                SetRest(false, 30);
                break;
            case 5:
                displayRest = true;
                SetRest(true, 25);
                break;
        }
        enemyRest = 0;
        reduce = 1;
        clearChack = false;
        DisplayPad();
        //�I�u�W�F�N�g����Text�R���|�[�l���g���擾
        textMultiple = multiple.GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    void DisplayPad()
    {
        if (TitleManager.virtualPad == true)
        {
            virtualPad.SetActive(true);
            bombType.SetActive(false);
        }
        else
        {
            virtualPad.SetActive(false);
            //�e�L�X�g�ō��̔��e�̎�ނ�\����ǉ�����
            bombType.SetActive(true);
        }
    }

    void StageBgmStart()
    {
        audioSource.PlayOneShot(audioClip[TitleManager.stageIndex]); //�X�e�[�W���Ɨpbgm
    }

    void AttackText()
    {
        if (BomSelect.seles >= bomb.Length)
        {
            BomSelect.seles = 0;
        }
        if (BomSelect.seles < 0)
        {
            BomSelect.seles = bomb.Length - 1;
        }
        nowBomb.text = "" + bomb[BomSelect.seles];
        textBombType.text = "" + bomb[BomSelect.seles];
    }

    void ResetLastSelected()
    {
        if (lastSelected != null)
        {
            lastSelected.transform.localScale = Vector3.one;
            lastSelected = null;
        }
    }

    //�Q�[���N���A�A�Q�[���I�[�o�[
    void GameState()
    {
        //Debug.Log($"enemyRest={enemyRest}, gameOver={player.gameOver}");
        if (enemyRest == 0 && !player.gameOver && !clearChack) //�A�C�e�����������i�Ԃɍ����΁j
        {
            StartCoroutine(CheckGameClear());
        }
        if (!gameClear && player.gameOver)
        {
            if (TitleManager.virtualPad == false)
            {
                GameObject firstButton = gameOverPanel.transform.GetChild(0).gameObject;
                EventSystem.current.SetSelectedGameObject(firstButton);

                //������ current/lastSelected �𖾎��I�ɐݒ肷��
                currentSelected = firstButton;
                lastSelected = firstButton;

                //�I�������\��
                currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
            }

            gameOverPanel.SetActive(true);
        }
        if (gameClear) {
            if(TitleManager.virtualPad == false)
            {
                EventSystem.current.SetSelectedGameObject(gameClearPanel.transform.GetChild(0).gameObject);
                ResetLastSelected(); //�����I���{�^���̏�����
            }
            gameClearPanel.SetActive(true);
        }
    }

    IEnumerator CheckGameClear()
    {
        yield return new WaitForSeconds(0.1f); //�����x�点�Đ�����gameOver�𔽉f

        if (!player.gameOver)
        {
            gameClear = true;

            if (!clearChack)
            {
                if (stageClear < TitleManager.stageIndex + 1)
                {
                    stageClear = TitleManager.stageIndex + 1;
                    Debug.Log("�X�e�[�W���: " + stageClear);
                }
                clearChack = true;
            }

            gameClearPanel.SetActive(true);
        }
    }

    void UpdateSelectUI()
    {
        //���ݑI�΂�Ă���{�^�����擾
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return; // null �Ȃ�X�L�b�v

        currentSelected = selected;

        //�O��ƈႤ�{�^�����I�΂ꂽ�ꍇ�����������s��
        if (currentSelected != lastSelected)
        {
            //�O�ɑI�΂�Ă����{�^���̃T�C�Y�����ɖ߂�
            if (lastSelected != null)
            {
                lastSelected.transform.localScale = Vector3.one;
            }
            //�I�΂�Ă���{�^���������傫������
            currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);

            //����I�񂾃{�^�����u�O��I�΂ꂽ���́v�Ƃ��ċL�^
            lastSelected = currentSelected;
        }
    }

    /*  void AttackEnd()
      {
          if (attackEnd)
          {
              audioSource.Play();
          }
      }*/

    void TimeAttack()
    {
        //Debug.Log(m_time);
       
        if (m_time > 0)
        {
            m_time -= Time.deltaTime;
        }
        if (m_time < 0)
        {
            m_time = 0;
        }
        if ((m_time == 0 && startTimer) || (m_rest == 0 && startRest))//||�G�Ɠ�������Q�[���I�[�o�[
        {
            //�Q�[���I�[�o�[
            player.gameOver = true;
        }
    }

    void DisplayTimer()
    {
        //�e�L�X�g�̕\�������ւ���
        textMultiple.text = "00:" + (int)m_time;
        if (m_time < 10)
        {
            textMultiple.text = "00:0" + (int)m_time;
        }
    }

    //�J�E���g���K�v�ȖʂŎg��(�^�C���A�^�b�N)
    void SetTimer(bool isTime, float time)
    {
        m_time = time;
        if (isTime)
        {
            startTimer = true;
            multiple.SetActive(true);
        }
    }


    void DisplayRest()
    {
        if (player.isGenerate)
        {
            m_rest -= reduce;
            player.isGenerate = false;
        }
        //�e�L�X�g�̕\�������ւ���
        textMultiple.text = "�c��:" + m_rest;
    }

    void SetRest(bool isRest, int rest)
    {
        m_rest = rest;
        player.generate = rest;
        if (isRest)
        {
            startRest = true;
            multiple.SetActive(true);
        }
        if(!isRest && !startTimer)
        {
            multiple.SetActive(true);
        }
    }

    void DisplayEnemy()
    {
        enemyRestText.text = "�G: " + enemyRest;
    }

    public void NextStage()
    {
        TitleManager.stageIndex += 1; //���̃X�e�[�W�ԍ��ɐi��
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void ReStart()
    {
        enemyRest = 0;
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }
}
