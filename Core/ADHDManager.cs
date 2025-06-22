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
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace SatisfyingOverlay.Core;

public class ADHDManager: MonoBehaviour
{
    private RawImage _rawImage;
    private Vector2 _lastPosition;
    private Vector2 _lastScale;
    private float _lastTransparency;
    
    private GameObject _videoPlayer;
    private GameObject _imageRender;
    private RenderTexture _renderTexture;


    public ManualLogSource logger;
    public static ADHDManager Instance;
    
    public static ADHDManager Create(ManualLogSource log)
    {
        if (Instance != null)
            return Instance;

        GameObject go = new GameObject("ADHDManager");
        Instance = go.AddComponent<ADHDManager>();
        Instance.logger = log;
        DontDestroyOnLoad(go);

        WorldStart += OnWorldStart;
        WorldDispose += OnWorldDispose;

        return Instance;
    }
    
    public static event Action<GameWorld> WorldStart
    {
        add => EFTHelpers._GameWorldHelper.OnGameStarted.Add(value);
        remove => EFTHelpers._GameWorldHelper.OnGameStarted.Remove(value);
    }
    
    public static event Action<GameWorld> WorldDispose
    {
        add => EFTHelpers._GameWorldHelper.Dispose.Add(value);
        remove => EFTHelpers._GameWorldHelper.Dispose.Remove(value);
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
    }
    
    public static void OnWorldDispose(GameWorld world)
    {
        if (Instance == null)
            return;

        Instance.logger.LogInfo("ADHDVideo Disposed");

        if (Instance._rawImage != null)
            Destroy(Instance._rawImage.gameObject);

        if (Instance._videoPlayer != null)
            Destroy(Instance._videoPlayer);

        if (Instance._imageRender != null)
            Destroy(Instance._imageRender);

        if (Instance._renderTexture != null)
        {
            Instance._renderTexture.Release();
            Destroy(Instance._renderTexture);
        }

        Instance._rawImage = null;
        Instance._videoPlayer = null;
        Instance._imageRender = null;
        Instance._renderTexture = null;
    }

    
    public void PlayVideo(float posX, float posY)
    {
        if (!HasPlayer)
        {
            return;
        }
        
        var videoPath = Path.Combine(BepInEx.Paths.PluginPath, "SatisfyingOverlay", "ADHDVideos", "soap.mp4");
        logger.LogInfo($"Path to video {videoPath}");

        if (!File.Exists(videoPath))
        {
            logger.LogWarning("Video Not Exist: " + videoPath);
        }

        _videoPlayer = new GameObject("ADHDVideoPlayer");
        var player = _videoPlayer.AddComponent<VideoPlayer>();
            
        _imageRender = new GameObject("ADHDVideoImage", typeof(RawImage));
        _imageRender.transform.SetParent(YourEftBattleUIScreen.RectTransform.transform, false);
            
        _renderTexture = new RenderTexture(1280, 720, 0);
        _renderTexture.Create();
        
        _rawImage = _imageRender.GetComponent<RawImage>();
        _rawImage.texture = _renderTexture;
        _rawImage.color = new Color(1, 1, 1, SettingsModel.Instance.transparency.Value);
        _rawImage.rectTransform.sizeDelta = new Vector2(320, 180);
        _rawImage.rectTransform.anchoredPosition = new Vector2(posX, posY);
        _lastPosition = _rawImage.rectTransform.anchoredPosition;
        _lastScale = new Vector2(320, 180);

            
        player.playOnAwake = false;
        player.isLooping = true;
        player.audioOutputMode = VideoAudioOutputMode.None;
        player.targetTexture = _renderTexture;
        player.source = VideoSource.Url;
        player.url = "file:///" + videoPath.Replace("\\", "/");
        player.Play();

        logger.LogInfo("Videoplayer created " + videoPath);
    }
    
    public void UpdatePosition(float posX, float posY)
    {
        if (_rawImage == null)
            return;

        Vector2 current = new Vector2(posX, posY);
        if (current != _lastPosition)
        {
            _rawImage.rectTransform.anchoredPosition = current;
            _lastPosition = current;
        }
    }
    
    public void UpdateScale(float width, float height)
    {
        if (_rawImage == null)
            return;

        Vector2 newScale = new Vector2(width, height);
        if (newScale != _lastScale)
        {
            _rawImage.rectTransform.sizeDelta = newScale;
            _lastScale = newScale;
        }
    }
    
    public void UpdateTransparency(float newTransparency)
    {
        if (_rawImage == null)
            return;
        
        if (newTransparency != _lastTransparency)
        {
            _rawImage.color = new Color(1, 1, 1, newTransparency);
            _lastTransparency = newTransparency;
        }
    }

}