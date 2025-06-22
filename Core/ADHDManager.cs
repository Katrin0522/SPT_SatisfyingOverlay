using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using EFT;
using EFT.UI;
using KmyTarkovApi;
using SatisfyingOverlay.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SatisfyingOverlay.Core;

public class ADHDManager: MonoBehaviour
{
    private RawImage rawImage;
    private Vector2 lastPosition;
    private Vector2 lastScale;
    private float lastTransparency;
    
    public ManualLogSource logger;
    public static ADHDManager Instance;
    
    public static ADHDManager Create(ManualLogSource log)
    {
        if (Instance != null)
        {
            return Instance;
        }
        
        return Instance = new ADHDManager(log);
    }
    
    public ADHDManager(ManualLogSource log)
    {
        logger = log;
        WorldStart += OnWorldStart;
    }
    
    public static event Action<GameWorld> WorldStart
    {
        add => EFTHelpers._GameWorldHelper.OnGameStarted.Add(value);
        remove => EFTHelpers._GameWorldHelper.OnGameStarted.Remove(value);
    }

    private EftBattleUIScreen YourEftBattleUIScreen => EFTGlobal.EftBattleUIScreen;

    private Player YourPlayer => EFTGlobal.Player;

    private bool HasPlayer => YourPlayer != null;

    public static void OnWorldStart(GameWorld world)
    {
        if (SettingsModel.Instance.showVideo.Value)
        {
            Instance.logger.LogInfo("World started, we`ll start ADHD video");
            Instance.PlayVideo(SettingsModel.Instance.positionX.Value, SettingsModel.Instance.positionY.Value);
        }
        else
        {
            Instance.logger.LogInfo("World started, but you disabled Satisfying video in settings pWq");
        }
    }
    
    public void PlayVideo(float posX, float posY)
    {
        if (!HasPlayer)
        {
            return;
        }
        
        string videoPath = Path.Combine(BepInEx.Paths.PluginPath, "SatisfyingOverlay", "ADHDVideos", "soap.mp4");
        logger.LogWarning($"Path to video {videoPath}");

        if (!File.Exists(videoPath))
        {
            logger.LogWarning("Video Not Exist: " + videoPath);
        }

        GameObject videoPlayer  = new GameObject("ADHDVideoPlayer");
        VideoPlayer player = videoPlayer.AddComponent<VideoPlayer>();
            
        GameObject imageRender = new GameObject("ADHDVideoImage", typeof(RawImage));
        imageRender.transform.SetParent(YourEftBattleUIScreen.RectTransform.transform, false);
            
        var renderTexture = new RenderTexture(1280, 720, 0);
        renderTexture.Create();
        
        rawImage = imageRender.GetComponent<RawImage>();
        rawImage.texture = renderTexture;
        rawImage.color = new Color(1, 1, 1, 0.85f);
        rawImage.rectTransform.sizeDelta = new Vector2(320, 180);
        rawImage.rectTransform.anchoredPosition = new Vector2(posX, posY);
        lastPosition = rawImage.rectTransform.anchoredPosition;
        lastScale = new Vector2(320, 180);

            
        player.playOnAwake = false;
        player.isLooping = true;
        player.audioOutputMode = VideoAudioOutputMode.None;
        player.targetTexture = renderTexture;
        player.source = VideoSource.Url;
        player.url = "file:///" + videoPath.Replace("\\", "/");
        player.Play();

        logger.LogInfo("Videoplayer created " + videoPath);
    }
    
    public void UpdatePosition(float posX, float posY)
    {
        if (rawImage == null)
            return;

        Vector2 current = new Vector2(posX, posY);
        if (current != lastPosition)
        {
            rawImage.rectTransform.anchoredPosition = current;
            lastPosition = current;
        }
    }
    
    public void UpdateScale(float width, float height)
    {
        if (rawImage == null)
            return;

        Vector2 newScale = new Vector2(width, height);
        if (newScale != lastScale)
        {
            rawImage.rectTransform.sizeDelta = newScale;
            lastScale = newScale;
        }
    }
    
    public void UpdateTransparency(float newTransparency)
    {
        if (rawImage == null)
            return;
        
        if (newTransparency != lastTransparency)
        {
            rawImage.color = new Color(1, 1, 1, newTransparency);
            lastTransparency = newTransparency;
        }
    }

}