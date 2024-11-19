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
    // Start is called before the first frame update
    void Start()
    {

        dbManager = new DatabaseManager();
        dbManager.Connect();

        // DB에서 성별 정보 가져오기
        User user = dbManager.login();
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name.Contains("Male") ? 0 : 1).ToArray();
        // 성별에 따라 캐릭터 선택 (0: 남자, 1: 여자)
        SetCharacterByGender(setGender);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetCharacterByGender(int gender){
        if (characterList.Length < 2){

            if(gender == 0 ){
                characterList[0].SetActive(true);
                characterList[1].SetActive(false);
            }else if(gender == 1){
                characterList[0].SetActive(false);
                characterList[1].SetActive(true);
            }
        }
        else
        {
            Debug.LogError("characterList에 필요한 캐릭터 오브젝트가 부족합니다.");
        }
    }
}
