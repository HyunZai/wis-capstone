using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SetPlayerGender : MonoBehaviour
{
    public int setGender = 0;
    public GameObject[] characterList;
    private DatabaseManager dbManager;

    // Start is called before the first frame update
    void Start()
    {
        dbManager = new DatabaseManager();
        dbManager.Connect();

        // DB에서 성별 정보 가져오기
        User user = dbManager.login();

        // Player 태그가 있는 활성화된 GameObject만 찾기
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name.Contains("Male") ? 0 : 1).ToArray();

        // 성별에 맞는 캐릭터 활성화 및 스크립트 비활성화 처리
        SetCharacterByGender(setGender);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetCharacterByGender(int gender)
    {
        Debug.Log(characterList[0].name + " , " + characterList[1].name);
        for (int i = 0; i < characterList.Length; i++)
        {
            bool isActive = (i == gender); // 현재 선택된 캐릭터만 활성화
            characterList[i].SetActive(isActive); // 오브젝트 활성화/비활성화
            MonoBehaviour[] scripts = characterList[i].GetComponents<MonoBehaviour>(); // 연결된 모든 스크립트 가져오기

            foreach (var script in scripts)
            {
                script.enabled = isActive; // 활성화된 캐릭터의 스크립트만 활성화
            }
        }
    }
}