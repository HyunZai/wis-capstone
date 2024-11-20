using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SetPlayerGender : MonoBehaviour
{
    public int setGender = 0;
    public GameObject [] characterList;
    private DatabaseManager dbManager;
    void Awake()
    {

        dbManager = new DatabaseManager();
        dbManager.Connect();

        // DB에서 성별 정보 가져오기
        User user = dbManager.login();

        if (user != null)
        {
            // DB에서 가져온 성별 정보 설정 (0: 남자, 1: 여자)
            setGender = user.gender == 0 ? 0 : 1;  // 여기서 성별을 setGender에 할당
        }

        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name.Contains("Male") ? 0 : 1).ToArray();
        SetCharacterByGender(setGender);   
    }

    void SetCharacterByGender(int gender){

        if (characterList[0] != null) 
        {
            characterList[0].SetActive(gender == 0); //남자 캐릭터 활성화
        }   

        if (characterList[1] != null)  
        {
            characterList[1].SetActive(gender == 1);  // 여자 캐릭터 활성화
        }
    
    }
}
