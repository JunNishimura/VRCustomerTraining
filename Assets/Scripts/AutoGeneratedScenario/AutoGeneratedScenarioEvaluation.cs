using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutoGeneratedScenarioEvaluation : MonoBehaviour
{
    List<int> _usedTableList;
    List<int> _usedFoodList;
    int _scenarioCount;
    bool _isEntranceGenerated = false;
    bool _isComplainGenerated = false;
    string[] SCENARIO_ARRAY = new string[] { "�z�V", "����", "���X", "�Еt��", "�N���[������" };
    string[] FOOD_ARRAY = new string[] { "�T���h�E�B�b�`�Z�b�g", "�s�U", "�p���P�[�L", "�n���o�[�K�[�Z�b�g", "�`���R���[�g�P�[�L", "�o�Q�b�g�Z�b�g" };

    [System.Serializable]
    public class ScenarioData
    {
        public string task;
        public int table;
        public int food;
    }

    [System.Serializable]
    public class ScenarioCollection
    {
        public List<ScenarioData> scenarioList { get; set; }
    };
    ScenarioCollection _scenarioCollection;

    void Start()
    {
        ScenarioGenerate();
        ScenarioEvaluate();
    }

    // �V�i���I�̎�������
    void ScenarioGenerate()
    {
        _scenarioCollection = new ScenarioCollection
        {
            scenarioList = new List<ScenarioData>()
        };
        _usedTableList = new List<int>();
        _usedFoodList = new List<int>();

        _scenarioCount = Random.Range(1, 6);
        for (int i = 0; i < _scenarioCount; i++)
        {
            ScenarioData scenarioData = new ScenarioData();

            string task;
            while (true)
            {
                // �Ɩ��̐���
                task = SCENARIO_ARRAY[Random.Range(0, SCENARIO_ARRAY.Length)];

                // ���X�^�X�N�͑�����1��݂̂ɂ���
                if (task == "���X" && !_isEntranceGenerated)
                {
                    scenarioData.task = task;
                    _isEntranceGenerated = true;
                    break;
                }
                else if (task == "�N���[������" && !_isComplainGenerated) // �N���[�����΂͑�����1��݂̂ɂ���
                {
                    scenarioData.task = task;
                    _isComplainGenerated = true;
                    break;
                }
                else if (task != "���X" && task != "�N���[������")
                {
                    scenarioData.task = task;
                    break;
                }
            }

            // �e�[�u���̊��蓖��
            // �g�p����Ă��Ȃ��e�[�u���ɂȂ�܂Ń��[�v
            while (true)
            {
                int tableNumber = Random.Range(0, 6); // table��0 ~ 5�܂�
                bool isTableUsed = false;

                foreach (int usedTableNumber in _usedTableList)
                {
                    if (tableNumber == usedTableNumber)
                    {
                        isTableUsed = true;
                    }
                }

                // ���g�p�e�[�u���Ȃ�ǉ����ă��[�v�I��
                if (!isTableUsed)
                {
                    _usedTableList.Add(tableNumber);
                    scenarioData.table = tableNumber;
                    break;
                }
            }

            // �����z�V�̏ꍇ�́A�d�����Ȃ��悤�ɗ��������߂�
            if (task == "�z�V")
            {
                // �g�p����Ă��Ȃ������ɂȂ�܂Ń��[�v
                while (true)
                {
                    int foodNumber = Random.Range(0, FOOD_ARRAY.Length);
                    bool isFoodUsed = false;

                    foreach (int usedFoodNumber in _usedFoodList)
                    {
                        if (foodNumber == usedFoodNumber)
                        {
                            isFoodUsed = true;
                        }
                    }

                    // ���g�p�t�[�h�Ȃ�ǉ����ă��[�v�I��
                    if (!isFoodUsed)
                    {
                        _usedFoodList.Add(foodNumber);
                        scenarioData.food = foodNumber;
                        break;
                    }
                }
            }
            else if (task == "����") // �����̏ꍇ
            {
                // �d���A���ŗ��������߂�
                int foodNumber = Random.Range(0, FOOD_ARRAY.Length);
                scenarioData.food = foodNumber;
            } 
            else
            {
                scenarioData.food = -1;
            }

            // �������X�g�ɒǉ�
            _scenarioCollection.scenarioList.Add(scenarioData);
        }
    }

    void ScenarioEvaluate()
    {
        var json = JsonUtility.ToJson(_scenarioCollection);
        string savePath = Path.Combine(Application.dataPath, "auto_generated_scenario.json");
        File.WriteAllText(savePath, json);
    }
}