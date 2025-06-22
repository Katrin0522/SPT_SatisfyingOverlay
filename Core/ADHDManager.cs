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
    
    public EftBattleUIScreen YourEftBattleUIScreen => EFTGlobal.EftBattleUIScreen;

    public Player YourPlayer => EFTGlobal.Player;

    public bool HasPlayer => YourPlayer != null;

    public static void OnWorldStart(GameWorld world)
    {
        ADHDManager.Instance.logger.LogWarning("World started, we`ll start ADHD video");
        ADHDManager.Instance.PlayADHDVideo(SettingsModel.Instance.positionX.Value, SettingsModel.Instance.positionY.Value);
    }
    
    public void PlayADHDVideo(float posX, float posY)
    {
        if (!HasPlayer)
        {
            return;
        }
        
        string videoPath = Path.Combine(BepInEx.Paths.PluginPath, "ADHDVideo", "ADHDVideos", "soap.mp4");
        SatisfyingOverlayPlugin.LogSource.LogWarning($"Path to video {videoPath}");

        if (!File.Exists(videoPath))
        {
            logger.LogWarning("Video Not Exist: " + videoPath);
        }

        GameObject videoGO  = new GameObject("ADHDVideoPlayer");
        VideoPlayer player = videoGO.AddComponent<VideoPlayer>();
            
        GameObject imageGO = new GameObject("ADHDVideoImage", typeof(RawImage));
        imageGO.transform.SetParent(YourEftBattleUIScreen.RectTransform.transform, false);
            
        var renderTexture = new RenderTexture(1280, 720, 0);
        renderTexture.Create();
        
        rawImage = imageGO.GetComponent<RawImage>();
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

        logger.LogWarning("Videoplayer created " + videoPath);
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

}