using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScenario1Controller : MonoBehaviour
{
    public GameObject[] _foods; //  "�T���h�E�B�b�`�Z�b�g", "�s�U", "�p���P�[�L", "�n���o�[�K�[�Z�b�g", "�`���R���[�g�P�[�L", "�o�Q�b�g�Z�b�g"�̏��Ԃ�
    public GameObject[] _emptyPlates; // �󂫎M�B�e�[�u��1����6�ɂ����Ċi�[
    public GameObject[] _avatars; // �A�o�^�[�B�e�[�u��1����6�ɂ����Ċi�[
    public GameObject[] _chairs; // �֎q1-6�����ԂɊi�[
    public EnterCube[] _enterCubes;
    public EnterCube _entranceEnterCube; // �G���g�����X��enterCube
    public GameObject _entranceAvatar; // �G���g�����X�̃A�o�^�[

    [SerializeField] CleanPlateJudge _cleanPlageJudge;
    [SerializeField] GameObject _playerEye; // player�̖�

    Speech_Recognition_Manager _speechRecManager;
    Animator[] _animators = new Animator[6]; // animator���A�o�^�[1����6�ɂ����ď��ԂɊi�[
    Animator _entranceAvatarAnimator; // ���X�q��animator
    List<double> _taskFinishedTimeList; // �e�^�X�N�̏I������
    List<string> _taskFinishedList; // �^�X�N�̏I������
    List<int> _dialogueProgressList; // �e�^�X�N�̉�b�̐i�s����i�[�������X�g // 0: 2�ԃe�[�u��, 1: 5�ԃe�[�u��, 2: 6�ԃe�[�u��, 3: 3�ԃe�[�u��, 4: 4�ԃe�[�u�� 5: 1�ԃe�[�u��
    NavMeshAgent _navMeshAgent;
    double _elapsedTime; // �o�ߎ���

    // �V�i���I //
    // 2�ԃe�[�u�������@5�ԃe�[�u���Еt��
    // 6�ԃe�[�u�����X�@3�ԃe�[�u������
    // 4�ԃe�[�u���z�V�@1�ԕЕt��

    void Start()
    {
        BGMActivate();

        // _animators�̏�����
        for (int i = 0; i < _avatars.Length; i++)
        {
            _animators[i] = _avatars[i].GetComponent<Animator>();
        }
        _entranceAvatarAnimator = _entranceAvatar.GetComponent<Animator>();

        // �K�C�h�p�l���̏�����
        UIManager.Instance.guide.text = "";

        // 2�ԃe�[�u���̃A�o�^�[����A5�ԃe�[�u���󂫎M�\��
        _animators[1].SetBool("updown", true);
        _emptyPlates[4].SetActive(true);

        // �q���g�p�l���̏�����
        UIManager.Instance.hint1.text =
            "�ڋq�ł̒��ӓ_\n" +
            "�E�ڋq�����́A���������ł���Ɩ�����s���܂��傤\n" +
            "�E�����̂��q�l����Ăяo���ꂽ�ꍇ�ɁA��ɌĂ΂ꂽ���q�l����Ή����܂��傤\n" +
            "�E�z�V�̓I�[�_�[�\�̏��ɔz�V����悤�ɂ��܂��傤\n" +
            "�E�ς�ł���M�����������琺�������ϋɓI�ɕЕt���܂��傤\n" +
            "�E�f�U�[�g������ꍇ�A�H���ς݂��m�F�����炷���ɔz�V���܂��傤\n";
        UIManager.Instance.hint2.text =
            "�E�������������͂��q����̑O�ɒu���悤�ɂ��܂��傤\n" +
            "�E�ς񂾎M�̓J�E���^�[�֖߂��܂��傤\n" +
            "�E�����Ă��������������q����ɓ`���܂��傤\n" +
            "�E���q�l��҂����Ă��܂��ꍇ�ɂ́A�u���X���҂����������v�Ɛ��������s���܂��傤\n" +
            "�E���������I�����ۂɂ́A�����𕜏����Ă��q�l�Ɋm�F���܂��傤�B\n" +
            "�E�z�V��ɂ͒������ȏ�ł��邩���m�F���܂��傤\n";
        UIManager.Instance.hint3.text =
            "������@\n" +
            "�E�ETouch�̉��̃{�^��(A�{�^��)�F���b��������ɉ������ƂŃV�i���I���i�s\n" +
            "�E�ETouch�̏�̃{�^��(B�{�^��)�F�|�[�Y\n" +
            "�E��Touch�̉��̃{�^��(X�{�^��)�F�K�C�h�p�l���̕\����\���̐؂�ւ�\n" +
            "�E��Touch�̏�̃{�^��(Y�{�^��)�F�q���g�p�l���̕\����\���̐؂�ւ�(�S3�y�[�W)\n" +
            "�E���w�ɂ���{�^��(�O���b�v)�F��𗿗��E�M�̕��֌����Ď���\n" +
            "�E���X�e�B�b�N�F�e���|�[�g�ړ�\n" +
            "�E�E�X�e�B�b�N�F�e���|�[�g�ړ���̃L�����N�^�[�̌�����ύX����";

        // �I�[�_�[�e�[�u���̏�����
        UIManager.Instance.ordertable.text = "";

        // �t���O�}�l�[�W���[�̏�����
        FlagManager.Instance.ResetFlags();

        // �����F���N���X�̋N��
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _speechRecManager = null;
        }
        else
        {
            _speechRecManager = new Speech_Recognition_Manager();
            _speechRecManager.startRecognition();
        }

        // �ڋq�I�����Ԃ��i�[���郊�X�g�̏�����
        _taskFinishedTimeList = new List<double>();

        // �ڋq�I���������i�[���郊�X�g
        _taskFinishedList = new List<string>();

        // ��b�i�s���X�g�̏�����
        InitializeDialogueProgressList();
    }

    void Update()
    {
        // �o�ߎ��Ԃ̍X�V
        _elapsedTime = Time.time;

        // 2�ԃe�[�u������
        if (_enterCubes[1].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[1]))
        {
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[0]) // �^�X�N���������Ă�����AB�{�^���\�����邾��
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // A�{�^���\��
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // B�{�^���\��
                        switch (_dialogueProgressList[0])
                        {
                            case 0:
                                _animators[1].SetBool("updown", false); // ���������

                                // �����u���������ł��傤���v
                                AudioManager.Instance.PlayVoice("order_ok_man");

                                _dialogueProgressList[0] += 1; // ��b�̐i�s

                                break;
                            case 1:
                                AudioManager.Instance.PlayVoice("sandwitch_man"); // �����B�u�T���h�E�B�b�`�v

                                _dialogueProgressList[0] += 1; // ��b�̐i�s

                                break;
                            case 2:
                                // �����u�͂��v
                                AudioManager.Instance.PlayVoice("yes_man");

                                FlagManager.Instance.flags[0] = true;

                                // �I�����Ԃ̊i�[
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // �I�������̊i�[
                                _taskFinishedList.Add("2�ԃe�[�u������");

                                // 5�ԃe�[�u���Еt�����I����Ă����玟�̃^�X�N�J�n
                                if (FlagManager.Instance.flags[1])
                                {
                                    // ���X�q�̕\��
                                    _entranceAvatar.SetActive(true);
                                    _navMeshAgent = _entranceAvatar.GetComponent<NavMeshAgent>();
                                    // 3�ԃe�[�u���ڋq����
                                    _animators[2].SetBool("updown", true);
                                }

                                break;
                            default:
                                break;
                        }
                        // �����F���I�t
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }

        // 5�ԃe�[�u���Еt��
        // �e�[�u������M���ЂÂ���ꂽ���̔���
        if (_cleanPlageJudge.IsPlatePicked("Plate5") && !FlagManager.Instance.flags[1])
        {
            // �Еt������
            FlagManager.Instance.flags[1] = true;
            // �I�����Ԃ̊i�[
            _taskFinishedTimeList.Add(_elapsedTime);
            // �I�������̊i�[
            _taskFinishedList.Add("5�ԃe�[�u���Еt��");
            // 2�ԃe�[�u���������I����Ă����玟�̃^�X�N�J�n
            if (FlagManager.Instance.flags[0])
            {
                // ���X�q�̕\��
                _entranceAvatar.SetActive(true);
                _navMeshAgent = _entranceAvatar.GetComponent<NavMeshAgent>();
                // 3�ԃe�[�u���ڋq����
                _animators[2].SetBool("updown", true);
            }
        }

        // ���X�q�̑Ή��i�s��6�ԃe�[�u���j
        if (_entranceEnterCube.GetEnter() == 1 && IsPlayerLookAtCustomer(_entranceAvatar))
        {
            // �����F�������̃`�F�b�N
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (!FlagManager.Instance.flags[2])
                {
                    ABbuttonUpdate(true); // A�{�^���\��
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // B�{�^���\��
                        switch (_dialogueProgressList[2])
                        {
                            case 0:
                                AudioManager.Instance.PlayVoice("2-4"); // �����u�R���ł��B�ォ���l���܂��v
                                                                                                 
                                _dialogueProgressList[2] += 1;

                                break;
                            case 1:
                                AudioManager.Instance.PlayVoice("yes_man"); // �����u�͂��v
                                FlagManager.Instance.flags[2] = true;

                                // �I�����Ԃ̊i�[
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // �I�������̊i�[
                                _taskFinishedList.Add("6�ԃe�[�u�����X");

                                // 3�ԃe�[�u���������I����Ă����玟�̃^�X�N�J�n
                                if (FlagManager.Instance.flags[3])
                                {
                                    // �I�[�_�[�p�l���̍X�V
                                    UIManager.Instance.ordertable.text += "4�ԁF�s�U\n";
                                    // �u�s�U�v�̕\��
                                    _foods[1].SetActive(true);

                                    // 1�ԃe�[�u���̋󂫎M�\��
                                    _emptyPlates[0].SetActive(true);
                                }

                                break;
                            default:
                                break;
                        }
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }
        // ���X�q�̈ړ�
        if (FlagManager.Instance.flags[2])
        {
            int tableNum = 5;
            _navMeshAgent.SetDestination(_chairs[tableNum].transform.position);
            _entranceAvatarAnimator.SetBool("walk", true);
            _entranceAvatar.GetComponent<LookAtController>().enabled = false;

            if (Vector3.Distance(_chairs[tableNum].transform.position, _entranceAvatar.transform.position) < 1)
            {
                if (tableNum > 1) // table 2, 3, 4, 5�̏ꍇ
                {
                    _entranceAvatar.transform.rotation = Quaternion.Slerp(_entranceAvatar.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 0.2f);
                }
                else // table 0, 1�̏ꍇ
                {
                    _entranceAvatar.transform.rotation = Quaternion.Slerp(_entranceAvatar.transform.rotation, Quaternion.Euler(0f, 90f, 0f), 0.2f);
                }
            }

            if (Vector3.Distance(_chairs[tableNum].transform.position, _entranceAvatar.transform.position) < 0.71)
            {
                _entranceAvatarAnimator.SetBool("sitting", true);
            }
        }

        // 3�ԃe�[�u������
        if (_enterCubes[2].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[2]))
        {
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[3]) // �^�X�N���������Ă�����AB�{�^���\�����邾��
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // A�{�^���\��
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // B�{�^���\��
                        if (!FlagManager.Instance.flags[2])
                        {
                            // ���|��
                            // �����u�͂��v
                            AudioManager.Instance.PlayVoice("yes_woman");
                        }
                        else
                        {
                            switch (_dialogueProgressList[3])
                            {
                                case 0:
                                    _animators[2].SetBool("updown", false); // ���������

                                    // �����u���������ł��傤���v
                                    AudioManager.Instance.PlayVoice("order_ok_woman");

                                    _dialogueProgressList[3] += 1; // ��b�̐i�s

                                    break;
                                case 1:
                                    AudioManager.Instance.PlayVoice("pancake_woman"); // �����B�u�p���P�[�L�v

                                    _dialogueProgressList[3] += 1; // ��b�̐i�s

                                    break;
                                case 2:
                                    // �����u�͂��v
                                    AudioManager.Instance.PlayVoice("yes_woman");

                                    FlagManager.Instance.flags[3] = true;

                                    // �I�����Ԃ̊i�[
                                    _taskFinishedTimeList.Add(_elapsedTime);

                                    // �I�������̊i�[
                                    _taskFinishedList.Add("3�ԃe�[�u������");

                                    // 1�ԃe�[�u�����X���I����Ă����玟�̃^�X�N�J�n
                                    if (FlagManager.Instance.flags[2])
                                    {
                                        // �I�[�_�[�p�l���̍X�V
                                        UIManager.Instance.ordertable.text += "4�ԁF�s�U\n";
                                        // �u�s�U�v�̕\��
                                        _foods[1].SetActive(true);

                                        // 1�ԃe�[�u���̋󂫎M�\��
                                        _emptyPlates[0].SetActive(true);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                        // �����F���I�t
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }

        // 4�ԃe�[�u���z�V
        // �v���C���[���ڋq�����Ă��邩�̔���
        if (_enterCubes[3].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[3]))
        {
            // �����F�������`�F�b�N
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[4]) // �^�X�N���������Ă�����AB�{�^���\�����邾��
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // A�{�^���\��
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // B�{�^���\��
                        switch (_dialogueProgressList[4])
                        {
                            case 0:
                                // �K���ȗ������^�΂ꂽ���̔���
                                if (_enterCubes[3].GetObjectName().Contains("�s�U"))
                                {
                                    // �K���ȗ������^�΂ꂽ�Ƃ�
                                    // �����u���肪�Ƃ��������܂��v
                                    AudioManager.Instance.PlayVoice("thankyou_man");

                                    _dialogueProgressList[4] += 1; // ��b�i�s��̍X�V
                                }
                                else
                                {
                                    //�ԈႦ�������������Ă����ꍇ
                                    // �����u���̗����͗���ł��܂���v
                                    AudioManager.Instance.PlayVoice("not_order_man");
                                }

                                break;
                            case 1:
                                // �����u�͂��v
                                AudioManager.Instance.PlayVoice("yes_man");

                                FlagManager.Instance.flags[4] = true;

                                // �I�����Ԃ̊i�[
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // �I�������̊i�[
                                _taskFinishedList.Add("�S�ԃe�[�u���z�V");

                                // �I�[�_�[�e�[�u������폜
                                UIManager.Instance.Order_Remove("4��:�s�U");

                                break;
                            default:
                                break;
                        }
                        // �����F���I�t
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }

        // 1�ԃe�[�u���Еt��
        if (_cleanPlageJudge.IsPlatePicked("Plate5") && !FlagManager.Instance.flags[1])
        {
            // �Еt������
            FlagManager.Instance.flags[1] = true;
            // �I�����Ԃ̊i�[
            _taskFinishedTimeList.Add(_elapsedTime);
            // �I�������̊i�[
            _taskFinishedList.Add("5�ԃe�[�u���Еt��");
            // 2�ԃe�[�u���������I����Ă����玟�̃^�X�N�J�n
            if (FlagManager.Instance.flags[0])
            {
                // ���X�q�̕\��
                _entranceAvatar.SetActive(true);
                _navMeshAgent = _entranceAvatar.GetComponent<NavMeshAgent>();
                // 3�ԃe�[�u���ڋq����
                _animators[2].SetBool("updown", true);
            }
        }

        // �S�^�X�N�I��
        ScenarioCompletedJudge();

        if (FlagManager.Instance.flags[6])
        {
            // �K�C�h�p�l���̍X�V
            UIManager.Instance.resultPanel.SetActive(true);
            UIManager.Instance.evaluationPanel.SetActive(true);
            UIManager.Instance.recognitionPanel.SetActive(false);
            UIManager.Instance.APanel.SetActive(false);
            UIManager.Instance.BPanel.SetActive(false);
            UIManager.Instance.guidePanel.SetActive(false);

            // �ڋq�I�����Ԃ̕\��
            UIManager.Instance.result.text = "�ڋq�^�C���F\n";
            for (int i = 0; i < _taskFinishedTimeList.Count; i++)
            {
                UIManager.Instance.result.text += (i + 1) + "�ԖڏI���^�C���F" + (int)_taskFinishedTimeList[i] / 60 + "��" + (int)_taskFinishedTimeList[i] % 60 + "�b     ";
                if (i % 2 != 0)
                {
                    UIManager.Instance.result.text += " " + "\n";
                }
            }

            // �ڋq�I�������̕\��
            UIManager.Instance.evaluation.text = "�ڋq�����F\n";
            for (int i = 0; i < _taskFinishedList.Count; i++)
            {
                UIManager.Instance.evaluation.text += (i + 1) + "�ԖځF" + _taskFinishedList[i] + "    ";
                if (i % 2 == 1)
                {
                    UIManager.Instance.evaluation.text += "\n";
                }
            }
        }
    }

    // �S�V�i���I���������Ă��邩�̔���
    void ScenarioCompletedJudge()
    {
        bool _isCompleted = true;
        if (!FlagManager.Instance.flags[6])
        {
            for (int i = 0; i < 6; i++)
            {
                if (!FlagManager.Instance.flags[i])
                {
                    _isCompleted = false;
                }
            }

            // �������Ă���ꍇ�́A�I���t���O�𗧂Ă�
            if (_isCompleted)
            {
                FlagManager.Instance.flags[6] = true;
            }
        }
    }

    // AB�{�^���̍X�V
    void ABbuttonUpdate(bool isA)
    {
        if (isA)
        {
            UIManager.Instance.a.text = "A";
            UIManager.Instance.b.text = "";
        }
        else
        {
            UIManager.Instance.a.text = "";
            UIManager.Instance.b.text = "B";
        }
    }

    // BGM�𗬂�
    void BGMActivate()
    {
        AudioManager.Instance.PlayBGM("Special To Me-Slow Edit");
        AudioManager.Instance.AttachBGMSource.loop = true;
        AudioManager.Instance.ChangeVolume(0.2f, 1f, 1f);
    }

    // �v���C���[���ڋq�����Ă��邩�̔���
    bool IsPlayerLookAtCustomer(GameObject customer)
    {
        // �J�����̕������擾
        Vector3 cameraDirection = _playerEye.transform.forward;

        // �^�[�Q�b�g�܂ł̕������擾
        Vector3 targetDirection = customer.transform.position - _playerEye.transform.position;
        targetDirection.y = targetDirection.y + 1.50f;  //�L�����̊�ւ̕�������

        float angle = Vector3.Angle(cameraDirection, targetDirection);

        return 0 < angle && angle < 30;
    }

    // _dialogueProgressList�̏�����
    void InitializeDialogueProgressList()
    {
        _dialogueProgressList = new List<int>();

        // �V�i���I�̐�����0��ǉ�
        for (int i = 0; i < 6; i++)
        {
            _dialogueProgressList.Add(0);
        }
    }
}
