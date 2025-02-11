using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private void Start()
    {
        string videoPath = Path.Combine(Application.streamingAssetsPath, "CUTSCENE_SOUND.mp4");
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
