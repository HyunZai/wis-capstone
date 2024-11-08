using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] materials;  

    private float[] arrPosX = {-8f, -6f, -4f, -2f, 0f, 2f, 4f, 6f};
    
    [SerializeField]
    private float spawnInterval = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartMaterialRoutine();
    }

    void StartMaterialRoutine() {
        StartCoroutine("MaterialRoutine");
    }

    IEnumerator MaterialRoutine() {
        // 3초 기다린 후 무한 반복
        yield return new WaitForSeconds(2f);

        while(true) {
            //반복문
            foreach (float posX in arrPosX){    
                int index = Random.Range(0, materials.Length);
                SpawnMaterial(posX, index);
            }    
            yield return new WaitForSeconds(spawnInterval);  // 1.5초 기다렸다가 다시 무한 반복문
        }
    }

    void SpawnMaterial(float posX, int index){
        Vector3 spawPos = new Vector3(posX, transform.position.y, transform.position.z);

        //Instantiate(materials[index], spawPos, Quaternion.identity);
        GameObject material = Instantiate(materials[index], spawPos, Quaternion.identity);

        // 속도를 랜덤하게 설정
        float randomSpeed = Random.Range(0.3f, 1f);
        material.GetComponent<CafeMaterial>().SetMoveSpeed(randomSpeed);

    }
}
