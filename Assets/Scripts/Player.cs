using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Player : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    private float moveSpeed = 4f;  //이동속도
    Animator anim;

    public GameObject popup;
    //Rigidbody2D rb;

    void Start()
    {   
        popup.SetActive(false);

        if (PlayerPrefs.HasKey("playerPosition")) //건물에서 나왔을 때
        {
            float x = float.Parse(PlayerPrefs.GetString("playerPosition").Split("/")[0]);
            float y = float.Parse(PlayerPrefs.GetString("playerPosition").Split("/")[1]);
            transform.position = new Vector3(x, y, 0);

            if (y < -2f) //팝업 위치 변경(캐릭터가 아래쪽 건물에서 나오면 팝업에 가려져서)
            {
                Vector3 popupNewPosition = popup.transform.position;
                popupNewPosition.y = 2.8f;
                popup.transform.position = popupNewPosition;
            }
            
            popup.SetActive(true);

            PlayerPrefs.DeleteKey("playerPosition");
        }

        if (anim == null) anim = GetComponent<Animator>();  //에니메이션 접근
        //rb = GetComponent<Rigidbody2D>();

        //Video Player 컴포넌트 가져오기
        if (videoPlayer == null)
            videoPlayer = FindObjectOfType<VideoPlayer>();

        if (videoPlayer != null)
            videoPlayer.loopPointReached += EndReached;
    }
    
    void Update()
    {
        //키보드 방향키
        float inputX = Input.GetAxisRaw("Horizontal"); 
        float inputY = Input.GetAxisRaw("Vertical");

        //에니메이션
        if (inputX != 0 || inputY != 0)
        {
            anim.SetBool("ismove",true);
        }
        else
        {
            anim.SetBool("ismove",false);
        }

        anim.SetFloat("inputx",inputX);
        anim.SetFloat("inputy",inputY);
        
        transform.Translate(new Vector2(inputX,inputY) * Time.deltaTime * moveSpeed);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (colliders.Length > 0)
        {
            Debug.Log("Collision detected with: " + colliders[0].gameObject.name);
        }
    }

    //캐릭터가 특정건물 앞에 도착했을 때
    private void OnTriggerEnter2D(Collider2D other) {
        string position = "";

        if (other.gameObject.tag == "Point" && videoPlayer != null) 
        {
            switch (other.gameObject.name)
            {
                case "School":
                    position = transform.position.x.ToString() + "/" + (transform.position.y - 0.1).ToString();
                    videoPlayer.clip = videoClips[2];
                    break;
                case "FireStation":
                    position = (transform.position.x - 0.2).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[0];
                    break;
                case "Library":
                    position = (transform.position.x + 0.2).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[6];
                    break;
                case "Home":
                    videoPlayer.clip = videoClips[4];
                    break;
                case "Mart":
                    position = transform.position.x.ToString() + "/" + (transform.position.y - 0.1).ToString();
                    videoPlayer.clip = videoClips[7];
                    break;
                case "Police":
                    position = (transform.position.x + 0.2).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[1];
                    break;
                case "Bank":
                    position = (transform.position.x - 0.2).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[3];
                    break;
                case "Hospital":
                    position = (transform.position.x - 0.2).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[5];
                    break;
                case "Cafe":
                    position = transform.position.x.ToString() + "/" + (transform.position.y - 0.1).ToString();
                    break;
            }

            videoPlayer.Play();
        }
        if (PlayerPrefs.HasKey(other.gameObject.name + "VisitCount"))
            PlayerPrefs.SetInt(other.gameObject.name + "VisitCount", PlayerPrefs.GetInt(other.gameObject.name + "VisitCount") + 1);
        else
            PlayerPrefs.SetInt(other.gameObject.name + "VisitCount", 1);
        
        PlayerPrefs.SetString("playerPosition", position);
    }

    void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        SceneManager.LoadScene("InformationScene");
    }

    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }
}