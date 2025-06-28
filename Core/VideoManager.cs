using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.UI;
using KmyTarkovApi;
using SatisfyingOverlay.Models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace SatisfyingOverlay.Core;

public class VideoManager: MonoBehaviour
{
    public Dictionary<VideoConfigSlot, GameObject> SlotActivePlayers = new();
    public Dictionary<VideoConfigSlot, RawImage> SlotImages = new();
    public Dictionary<VideoConfigSlot, VideoPlayer> SlotPlayers = new();
    public Dictionary<VideoConfigSlot, RenderTexture> SlotTextures = new();

    public ManualLogSource Logger;
    private bool InGame = false;
    public static VideoManager Instance;
    
    public static VideoManager Create(ManualLogSource log)
    {
        if (Instance != null)
            return Instance;

        GameObject go = new GameObject("VideoManager");
        Instance = go.AddComponent<VideoManager>();
        Instance.Logger = log;
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
        Instance.UpdateInGameStatus(true);
        if (SettingsModel.Instance.GlobalEnable.Value)
        {
            Instance.Logger.LogInfo("World started, we`ll start all video");
            foreach (var cfg in SettingsModel.Instance.Slots)
            {
                if (!cfg.Enabled.Value) continue;

                var go = Instance.PlayVideo(cfg);

                if (go != null)
                {
                    Instance.SlotActivePlayers[cfg] = go;
                    Instance.UpdateMute(cfg);
                }
            }
            
            Instance.Logger.LogInfo("We started all enabled videos");
            Singleton<BetterAudio>.Instance.FadeMixerVolume(Singleton<BetterAudio>.Instance.AudioMixerData.MusicVolumeMixer, Singleton<SharedGameSettingsClass>.Instance.Sound.Settings.MusicVolumeValue, 0f, false);
        }
    }
    
    public static void OnWorldDispose(GameWorld world)
    {
        Instance.UpdateInGameStatus(false);
        Instance.DisposeAllVideos();
    }

    public void DisposeAllVideos()
    {
        Logger.LogInfo("Disposing all video objects");

        foreach (var tex in SlotTextures)
        {
            if (tex.Value == null) continue;
            
            tex.Value.Release();
            Destroy(tex.Value);
        }
        
        foreach (var player in SlotPlayers)
        {
            if (player.Value == null) continue;
            
            Destroy(player.Value);
        }
        
        foreach (var image in SlotImages)
        {
            if (image.Value == null) continue;
            
            Destroy(image.Value);
        }
        
        foreach (var playerObj in SlotActivePlayers)
        {
            if (playerObj.Value == null) continue;
            
            Destroy(playerObj.Value);
        }

        SlotActivePlayers.Clear();
        SlotTextures.Clear();
        SlotPlayers.Clear();
        SlotImages.Clear();
    }
    
    public GameObject PlayVideo(VideoConfigSlot cfg)
    {
        if (!HasPlayer)
        {
            return null;
        }
        
        var videoPath = Path.Combine(BepInEx.Paths.PluginPath, "SatisfyingOverlay", "Videos", cfg.FileName.Value);
        Logger.LogInfo($"[Slot {cfg.NameVideoSlot}] Path to video {videoPath}");

        if (!File.Exists(videoPath))
        {
            Logger.LogWarning($"[Slot {cfg.NameVideoSlot}] Video Not Exist: " + videoPath);
            return null;
        }

        var renderTexture = new RenderTexture(1280, 720, 0);
        renderTexture.Create();
        SlotTextures[cfg] = renderTexture;
        
        var videoPlayerGO = new GameObject($"[SO]VideoPlayer_[Slot{cfg.NameVideoSlot}]");
        var videoPlayer = videoPlayerGO.AddComponent<VideoPlayer>();
        SlotPlayers[cfg] = videoPlayer;
            
        var imageGO = new GameObject($"[SO]RawImage_[Slot{cfg.NameVideoSlot}]", typeof(RawImage));
        imageGO.transform.SetParent(YourEftBattleUIScreen.RectTransform.transform, false);
        
        var rawImage = imageGO.GetComponent<RawImage>();
        rawImage.texture = renderTexture;
        rawImage.color = new Color(1, 1, 1, cfg.Transparency.Value);
        rawImage.rectTransform.sizeDelta = new Vector2(cfg.Width.Value, cfg.Height.Value);
        rawImage.rectTransform.anchoredPosition = new Vector2(cfg.PositionX.Value, cfg.PositionY.Value);

        SlotImages[cfg] = rawImage;
            
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        
        var mixer = Singleton<GUISounds>.Instance.MasterMixer;
        
        AudioMixerGroup musicGroup = mixer.FindMatchingGroups("Music").First<AudioMixerGroup>();
            
        var audioSource = videoPlayerGO.gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;
        audioSource.mute = !cfg.AudioEnable.Value;
        audioSource.outputAudioMixerGroup = musicGroup;
        
        videoPlayer.SetTargetAudioSource(0, audioSource);
        
        
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file:///" + videoPath.Replace("\\", "/");
        videoPlayer.prepareCompleted += _ =>
        {
            Logger.LogInfo($"[Slot{cfg.NameVideoSlot}]Prepared, now playing...");
            videoPlayer.Play();
        };
        videoPlayer.Prepare();

        Logger.LogInfo("Videoplayer created " + videoPath);
        return videoPlayerGO;
    }
    
    public void UpdatePosition(VideoConfigSlot slot)
    {
        if (!SlotImages.TryGetValue(slot, out var image) || image == null)
            return;

        var pos = new Vector2(slot.PositionX.Value, slot.PositionY.Value);
        image.rectTransform.anchoredPosition = pos;
    }
    
    public void UpdateScale(VideoConfigSlot slot)
    {
        if (!SlotImages.TryGetValue(slot, out var image) || image == null)
            return;

        var newScale = new Vector2(slot.Width.Value, slot.Height.Value);
        image.rectTransform.sizeDelta = newScale;
    }
    
    public void UpdateTransparency(VideoConfigSlot slot)
    {
        if (!SlotImages.TryGetValue(slot, out var image) || image == null)
            return;
        
        image.color = new Color(1, 1, 1, slot.Transparency.Value);
    }

    public void UpdateVideoSlot(VideoConfigSlot slot)
    {
        Logger.LogInfo($"Updating slot: {slot.NameVideoSlot}");
        
        if (SlotImages.TryGetValue(slot, out var oldImage))
        {
            if (oldImage != null)
            {
                Destroy(oldImage.gameObject);
                SlotImages.Remove(slot);
            }
        }

        if (SlotPlayers.TryGetValue(slot, out var oldPlayer))
        {
            if (oldPlayer != null)
            {
                Destroy(oldPlayer.gameObject);
                SlotPlayers.Remove(slot);
            }
        }
        
        if (SlotTextures.TryGetValue(slot, out var oldTexture))
        {
            if (oldTexture != null)
            {
                oldTexture.Release();
                Destroy(oldTexture);
            }
        }
        
        if (SlotActivePlayers.TryGetValue(slot, out var oldActivePlayer))
        {
            if (oldActivePlayer != null)
            {
                Destroy(oldActivePlayer);
                SlotActivePlayers.Remove(slot);
            }
        }
        
        if (!slot.Enabled.Value || !InGame || !SettingsModel.Instance.GlobalEnable.Value)
        {
            return;
        }
        
        var go = Instance.PlayVideo(slot);

        if (go != null)
        {
            Instance.SlotActivePlayers[slot] = go;
            Logger.LogInfo($"Video updated for slot with {slot.FileName.Value}");
        }
    }

    public void UpdateSlotStatus()
    {
        Logger.LogWarning("Not realized method. UpdateSlotStatus");
    }

    public void UpdateInGameStatus(bool status)
    {
        InGame = status;
    }
    
    public void UpdateMute(VideoConfigSlot slot)
    {
        if (!SlotPlayers.TryGetValue(slot, out var videoPlayer))
            return;

        var existingSource = videoPlayer.GetTargetAudioSource(0);
        existingSource.mute = !slot.AudioEnable.Value;

        Logger.LogInfo($"Mute updated for slot: {slot.FileName.Value} = {slot.AudioEnable.Value}");
    }
}