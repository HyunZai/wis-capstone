using System.Collections;
using System.Collections.Generic;
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
        if (other.gameObject.tag == "Point" && videoPlayer != null) 
        {
            switch (other.gameObject.name)
            {
                case "School":
                    videoPlayer.clip = videoClips[2];
                    break;
                case "FireStation":
                    videoPlayer.clip = videoClips[0];
                    break;
                case "Library":
                    videoPlayer.clip = videoClips[6];
                    break;
                case "Home":
                    videoPlayer.clip = videoClips[4];
                    break;
                case "Mart":
                    videoPlayer.clip = videoClips[7];
                    break;
                case "Police":
                    videoPlayer.clip = videoClips[1];
                    break;
                case "Bank":
                    videoPlayer.clip = videoClips[3];
                    break;
                case "Hospital":
                    videoPlayer.clip = videoClips[5];
                    break;
            }

            videoPlayer.Play();
        }
    }

    void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        SceneManager.LoadScene("InformationScene");
    }
}
