using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // 씬 변경을 위한 네임스페이스 추가

public class CoffeePlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    // 먹을 수 있는 재료 목록을 따로 저장
    private List<string> allowedMaterials = new List<string> { "Material1", "Material5", "Material3" };

    // UI 아이콘 리스트
    private Dictionary<string, Image> uiIcons = new Dictionary<string, Image>();

    void Start()
    {
        // UI 아이콘을 미리 찾아서 저장
        uiIcons.Add("coffeebeans", GameObject.Find("coffeebeans").GetComponent<Image>());
        uiIcons.Add("water", GameObject.Find("water").GetComponent<Image>());
        uiIcons.Add("ice", GameObject.Find("ice").GetComponent<Image>());
    }

    void Update()
    {
        // 키보드로 움직임
        float horizontalInput = Input.GetAxisRaw("Horizontal"); 
        Vector3 moveTo = new Vector3(horizontalInput, 0f, 0f);
        transform.position += moveTo * moveSpeed * Time.deltaTime; 
    }

    // 재료와의 충돌 처리
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Material")) 
        {
            string materialName = other.gameObject.name;

            // 먹을 수 있는 재료인지 체크
            if (IsAllowedMaterial(materialName))
            {
                // 재료에 따라 UI 업데이트
                if (materialName.Contains("Material1")) 
                {
                    UpdateUI("coffeebeans");
                } 
                else if (materialName.Contains("Material5")) 
                {
                    UpdateUI("water");
                } 
                else if (materialName.Contains("Material3")) 
                {
                    UpdateUI("ice");
                }

                Destroy(other.gameObject);  // 재료 파괴
            }
        }
    }

    // 먹을 수 있는 재료인지 확인하는 함수
    bool IsAllowedMaterial(string materialName)
    {
        foreach (string allowedMaterial in allowedMaterials)
        {
            if (materialName.Contains(allowedMaterial))
            {
                return true;  // 먹을 수 있는 재료면 true 반환
            }
        }
        return false;  // 먹을 수 없는 재료면 false 반환
    }

    // UI 업데이트 메서드
    void UpdateUI(string iconName) 
    {
        if (uiIcons.ContainsKey(iconName))
        {
            Image image = uiIcons[iconName];
            if (image != null)
            {
                image.color = Color.black;  // UI 아이콘을 검은색으로 변경
                CheckAllIconsGreen();       // 모든 아이콘이 검은색인지 확인
            }
        }
    }

    // 모든 아이콘이 초록색인지 확인하는 메서드
    void CheckAllIconsGreen()
    {
        foreach (Image icon in uiIcons.Values)
        {
            if (icon.color != Color.black)
            {
                return;  // 검은색이 아닌 아이콘이 있으면 종료
            }
        }

        // 모든 아이콘이 초록색이라면 씬 전환
        SceneManager.LoadScene("NextScene");  // "NextScene" 씬으로 전환
    }
}