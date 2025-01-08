using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    public Canvas settingsCanvas;
    public Canvas mainMenuCanvas;

    public GameObject videoSettings;
    public GameObject audioSettings;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public TMP_Dropdown fullscreenDropdown;
    public TMP_Dropdown fpsDropdown;
    public TMP_Dropdown vSyncDropdown;

    void Awake()
    {
        LoadSettings();
    }

    void Start()
    {
        InitializeResolutionOptions();
        InitializeGraphicsOptions();
        InitializeFullscreenOptions();
        InitializeFPSOptions();
        InitializeVSyncOptions();
    }

    public void DisableSettingsCanvas()
    {
        settingsCanvas.enabled = false;
    }

    public void DisableMainMenuCanvas()
    {
        mainMenuCanvas.enabled = false;
    }

    public void EnableSettingsCanvas()
    {
        settingsCanvas.enabled = true;
    }

    public void EnableMainMenuCanvas()
    {
        mainMenuCanvas.enabled = true;
    }

    public void DisableAudioSettingsObject()
    {
        audioSettings.SetActive(false);
    }
    public void EnableAudioSettingsObject()
    {
        audioSettings.SetActive(true);
    }

    public void EnableVideoSettingsObject()
    {
        videoSettings.SetActive(true);
    }

    public void DisableVideoSettingsObject()
    {
        videoSettings.SetActive(false);
    }

    private void InitializeResolutionOptions()
    {
        resolutionDropdown.ClearOptions();
        var resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
        }
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(index =>
        {
            ChangeResolution(index);
            PlayerPrefs.SetInt("ResolutionIndex", index);
        });
    }

    private void InitializeGraphicsOptions()
    {
        graphicsDropdown.ClearOptions();
        string[] qualityLevels = QualitySettings.names;
        foreach (string quality in qualityLevels)
        {
            graphicsDropdown.options.Add(new TMP_Dropdown.OptionData(quality));
        }
        int savedQualityLevel = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        graphicsDropdown.value = savedQualityLevel;
        graphicsDropdown.onValueChanged.AddListener(index =>
        {
            ChangeGraphicsQuality(index);
            PlayerPrefs.SetInt("GraphicsQuality", index);
        });
    }

    private void InitializeFullscreenOptions()
    {
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
        fullscreenDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 0 : 1);
        fullscreenDropdown.value = savedFullscreen;
        fullscreenDropdown.onValueChanged.AddListener(index =>
        {
            ChangeFullscreenMode(index);
            PlayerPrefs.SetInt("Fullscreen", index);
        });
    }

    private void InitializeFPSOptions()
    {
        fpsDropdown.ClearOptions();
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("30 FPS"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("60 FPS"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("Unlimited"));
        int savedFPS = PlayerPrefs.GetInt("FPSLimit", 1); // Default to 60 FPS
        fpsDropdown.value = savedFPS;
        fpsDropdown.onValueChanged.AddListener(index =>
        {
            ChangeFPSLimit(index);
            PlayerPrefs.SetInt("FPSLimit", index);
        });
    }

    private void InitializeVSyncOptions()
    {
        vSyncDropdown.ClearOptions();
        vSyncDropdown.options.Add(new TMP_Dropdown.OptionData("Off"));
        vSyncDropdown.options.Add(new TMP_Dropdown.OptionData("On"));
        int savedVSync = PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount > 0 ? 1 : 0);
        vSyncDropdown.value = savedVSync;
        vSyncDropdown.onValueChanged.AddListener(index =>
        {
            ChangeVSync(index);
            PlayerPrefs.SetInt("VSync", index);
        });
    }

    private void ChangeResolution(int index)
    {
        Resolution resolution = Screen.resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void ChangeGraphicsQuality(int index)
    {
        Debug.Log("Index changed to: " + index);
        QualitySettings.SetQualityLevel(index);
    }

    private void ChangeFullscreenMode(int index)
    {
        if (index == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (index == 1)
        {
            int halfWidth = Screen.currentResolution.width / 2;
            int halfHeight = Screen.currentResolution.height / 2;

            Screen.SetResolution(halfWidth, halfHeight, false);

            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void ChangeFPSLimit(int index)
    {
        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = -1; break;
        }
    }

    private void ChangeVSync(int index)
    {
        QualitySettings.vSyncCount = index;
    }

    private void LoadSettings()
    {
        // Load resolution
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", Screen.resolutions.Length - 1);
        ChangeResolution(resolutionIndex);

        // Load graphics quality
        int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        ChangeGraphicsQuality(graphicsQuality);

        // Load fullscreen mode
        int fullscreenMode = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 0 : 1);
        ChangeFullscreenMode(fullscreenMode);

        // Load FPS limit
        int fpsLimit = PlayerPrefs.GetInt("FPSLimit", 1);
        ChangeFPSLimit(fpsLimit);

        // Load VSync
        int vSync = PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount > 0 ? 1 : 0);
        ChangeVSync(vSync);
    }
}
