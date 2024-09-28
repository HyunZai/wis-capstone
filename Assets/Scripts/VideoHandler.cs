using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("스페이스바 키");
        //     PlayVideo();
        // }

        if (!videoPlayer.isPlaying && videoPlayer.frame > 0)
        {
            VideoEnded();
        }
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    void VideoEnded()
    {
        videoPlayer.Stop();
    }
}
