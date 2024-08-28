using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Player : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    public float moveSpeed;  //이동속도
    Animator anim;

    void Start()
    {   
        if (PlayerPrefs.HasKey("playerPosition"))
        {
            float x = float.Parse(PlayerPrefs.GetString("playerPosition").Split("/")[0]); 
            float y = float.Parse(PlayerPrefs.GetString("playerPosition").Split("/")[1]); 
            transform.position = new Vector3(x, y, 0);

            PlayerPrefs.DeleteKey("playerPosition");
        }

        anim = GetComponent<Animator>();  //에니메이션 접근

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
            anim.SetBool("ismove",true);
        else
            anim.SetBool("ismove",false);

        anim.SetFloat("inputx",inputX);
        anim.SetFloat("inputy",inputY);

        transform.Translate(new Vector2(inputX,inputY) * Time.deltaTime * moveSpeed);
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
                    position = (transform.position.x - 0.1).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[0];
                    break;
                case "Library":
                    position = (transform.position.x + 0.1).ToString() + "/" + transform.position.y.ToString();
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
                    position = (transform.position.x + 0.1).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[1];
                    break;
                case "Bank":
                    position = (transform.position.x - 0.1).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[3];
                    break;
                case "Hospital":
                    position = (transform.position.x - 0.1).ToString() + "/" + transform.position.y.ToString();
                    videoPlayer.clip = videoClips[5];
                    break;
                case "Cafe":
                    position = transform.position.x.ToString() + "/" + (transform.position.y - 0.1).ToString();
                    break;
            }

            videoPlayer.Play();
        }

        PlayerPrefs.SetString("playerPosition", position);
    }

    void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        SceneManager.LoadScene("InformationScene");
    }
}
