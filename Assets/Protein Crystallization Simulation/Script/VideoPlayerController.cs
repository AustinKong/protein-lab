using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public RawImage videoDisplay;
    public VideoPlayer videoPlayer;
    public GameObject videoPanel;

    public StepManager stepManager; // 拖入 StepManager 或通过 Find 获取

    void Start()
    {
        videoPanel.SetActive(false);

        if (stepManager == null)
        {
            stepManager = FindObjectOfType<StepManager>();
        }

        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayVideo()
    {


        string videoFileName = $"{stepManager.currentStepIndex}.mp4";
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);


        videoPanel.SetActive(true);
        videoPlayer.url = videoPath;
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            videoDisplay.texture = videoPlayer.texture;
            videoPlayer.Play();
        };
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        CloseVideo();
    }

    private void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError("Video Error: " + message);
        CloseVideo();
    }
}
