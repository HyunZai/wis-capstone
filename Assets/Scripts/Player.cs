using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;  //이동속도
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();  //에니메이션 접근
    }
    
    void Update()
    {
        //키보드 방향키
        float inputX = Input.GetAxisRaw("Horizontal"); 
        float inputY = Input.GetAxisRaw("Vertical");

        //에니메이션
        if (inputX != 0 || inputY != 0)
            anim.SetBool("ismove",true);
        else
            anim.SetBool("ismove",false);

        anim.SetFloat("inputx",inputX);
        anim.SetFloat("inputy",inputY);

        transform.Translate(new Vector2(inputX,inputY) * Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Point") {
            Debug.Log(other.gameObject.name);
            SceneManager.LoadScene("Information");
        }
    }
}
