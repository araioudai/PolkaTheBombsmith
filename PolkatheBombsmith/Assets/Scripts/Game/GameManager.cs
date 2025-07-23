using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�萔:���ʂ̏��
    private const int GAME_OVER = 0;
    private const int GAME_CLEAR = 1;
    private const int GAME_CLEAR_ALL = 2;

    //bgm��se�̃I�[�f�B�I�𕪂��ĂȂ炷
    
    private AudioSource audioSource;                                                   //����炷�p�v���[���[BGM��p
    [SerializeField] private AudioSource seSource;                                     //SE��p
    //[SerializeField] private BomSelect select;
    [SerializeField] private PlayerManager player;                                     //PlayerManager�Q��
    [SerializeField] private GameObject virtualPad;                                    //�o�[�`�����p�b�h(�\���E��\��)
    [SerializeField] private GameObject bombType;                                      //�o�[�`�����p�b�hoff�̎����e�̎�ޕ\��
    [SerializeField] private GameObject multiple;                                      //�c�萔(�\���E��\��)
    [SerializeField] private GameObject attackButton;                                  //�A�^�b�N�{�^��(�\���E��\��) / ���e�^�C�v�؂�ւ��{�^��(image�؂�ւ��p)
    [SerializeField] private GameObject gameClearPanel;                                //�Q�[���N���A���p�l��(�\���E��\��)
    [SerializeField] private GameObject gameOverPanel;                                 //�Q�[���I�[�o�[���p�l��(�\���E��\��)
    //[SerializeField] private GameObject buttonUp;                                      //���e�^�C�v�؂�ւ��{�^��(image�؂�ւ��p)
    [SerializeField] private Text enemyRestText;                                       //�c��G��
    [SerializeField] private Text textMultiple;                                        //�c�蔚�e��
    [SerializeField] private Text nowBomb;                                             //���݂̔��e�^�C�v
    [SerializeField] private Text textBombType;                                        //�o�[�`�����p�b�hoff�̎����e�̎�ރe�L�X�g
    [SerializeField] private GameObject currentSelected;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private string[] bomb = { "�ʏ�", "�V��", "�n��", "�n��", "����" }; //�{���̎�ނɔz��
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>();        //�I�[�f�B�I�Z�b�g�p
    [SerializeField] private AudioClip[] resultSE;                                     //�Q�[���N���A�Q�[���I�[�o�[���̃T�E���h�p 
    [SerializeField] private Sprite[] spriteBombType;                                  //���e�{�^���̉摜�؂�ւ�
    private Image imageChange;
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
        ChangeButton();
        if (TitleManager.virtualPad == false)
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
        imageChange = attackButton.GetComponent<Image>();
        Time.timeScale = 1f;
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

    void ChangeButton()
    {
        //���e�^�C�v���V�䂩�n�ʂ̎��ɂ��������e�摜�ɂ���
        if(BomSelect.seles == 1 || BomSelect.seles == 2)
        {
            imageChange.sprite = spriteBombType[1];
        }
        else
        {
            imageChange.sprite = spriteBombType[0];
        }
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
        //�G��0 & �v���C���[�����S & �܂��N���A�o�^���ĂȂ�
        //�Q�[���N���A����
        if (enemyRest == 0 && !player.gameOver && !clearChack)
        {
            StartCoroutine(CheckGameClear());
        }

        //�Q�[���I�[�o�[
        if (!gameClear && player.gameOver)
        {
            //GameOver�����͂����Ŏ��{�i�Q�[���N���A���D��j
            if (TitleManager.virtualPad == false)
            {
                GameObject firstButton = gameOverPanel.transform.GetChild(0).gameObject;
                EventSystem.current.SetSelectedGameObject(firstButton);
                currentSelected = firstButton;
                lastSelected = firstButton;
                currentSelected.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
            }

            audioSource.Stop();
            audioSource = null;
            Time.timeScale = 0f;
            seSource.PlayOneShot(resultSE[GAME_OVER]);
            gameOverPanel.SetActive(true);
        }
    }


    IEnumerator CheckGameClear()
    {
        yield return new WaitForSeconds(0.1f); //�����x�点�Đ�����gameOver�𔽉f

        if (!player.gameOver && !gameClear)
        {
            //�Q�[���N���A�t���O�𗧂ĂāA�ȍ~�̏�����1�񂾂��s��
            gameClear = true;

            audioSource.Stop();
            audioSource = null;
            Time.timeScale = 0f;
            if (TitleManager.stageIndex == 5)
            {
                seSource.PlayOneShot(resultSE[GAME_CLEAR_ALL]);
            }
            else
            {
                seSource.PlayOneShot(resultSE[GAME_CLEAR]);
            }
            gameClearPanel.SetActive(true);

            //�X�e�[�W�N���A����ۑ��i1�x�����s���j
            if (!clearChack && stageClear < TitleManager.stageIndex + 1)
            {
                //���݂̃X�e�[�W���ߋ��ō��Ȃ�A���̃X�e�[�W���A�����b�N
                stageClear = TitleManager.stageIndex + 1;

                //��x�����X�e�[�W�A�����b�N����������悤�Ƀt���O�𗧂Ă�
                clearChack = true;
            }

            if (TitleManager.virtualPad == false)
            {
                EventSystem.current.SetSelectedGameObject(gameClearPanel.transform.GetChild(0).gameObject);
                ResetLastSelected();
            }
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
            if (BomSelect.seles == 3)
            {
                m_rest -= 4;
            }
            player.isGenerate = false;
        }
        //�e�L�X�g�̕\�������ւ���
        textMultiple.text = "�c��:" + player.generate;
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
        Time.timeScale = 1f;
        TitleManager.stageIndex += 1; //���̃X�e�[�W�ԍ��ɐi��
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void ReStart()
    {
        Time.timeScale = 1f;
        enemyRest = 0;
        clearChack = false;
        SceneManager.LoadScene("Game");
    }

    public void Title()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
