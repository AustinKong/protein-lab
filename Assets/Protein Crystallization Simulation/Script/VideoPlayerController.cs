using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public RawImage videoDisplay;
    public VideoPlayer videoPlayer;
    public GameObject videoPanel;

    void Start()
    {
        videoPanel.SetActive(false);

        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayVideo()
    {
        // ʹ�� Unity ���� SceneManager ��ȡ������
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // ������Ƶ·��
        string videoFileName = $"{sceneName}.mp4";
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
