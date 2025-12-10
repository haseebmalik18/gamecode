using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    Canvas canvas;
    GameObject mainPanel;
    GameObject instructionsPanel;
    GameObject settingsPanel;
    GameObject highScoresPanel;
    GameObject nameInputPanel;
    
    Slider volumeSlider;
    TextMeshProUGUI volumeValueText;
    Button easyBtn, normalBtn, hardBtn;
    Toggle muteToggle;
    RectTransform muteHandleRect;
    TMP_InputField nameInputField;
    TextMeshProUGUI pendingScoreText;
    int currentDifficulty;
    int pendingScore;
    
    GameObject scoreListContainer;

    Color batmanYellow = new Color(1f, 0.82f, 0.04f);
    Color batmanDarkBlue = new Color(0.05f, 0.05f, 0.15f);
    Color batmanBlue = new Color(0.1f, 0.1f, 0.25f);
    Color batmanGray = new Color(0.2f, 0.2f, 0.3f);

    void Start()
    {
        EnsureManagersExist();
        CreateCanvas();
        CreateMainPanel();
        CreateInstructionsPanel();
        CreateSettingsPanel();
        CreateHighScoresPanel();
        CreateNameInputPanel();
        
        CheckPendingScore();
    }

    void CheckPendingScore()
    {
        pendingScore = PlayerPrefs.GetInt("PendingScore", 0);
        if (pendingScore > 0)
        {
            PlayerPrefs.DeleteKey("PendingScore");
            PlayerPrefs.Save();
            ShowNameInputPanel();
        }
        else
        {
            ShowMainPanel();
        }
    }

    void EnsureManagersExist()
    {
        if (GameData.Instance == null)
        {
            GameObject gameDataObj = new GameObject("GameData");
            gameDataObj.AddComponent<GameData>();
        }
        
        if (HighScoreManager.Instance == null)
        {
            GameObject highScoreObj = new GameObject("HighScoreManager");
            highScoreObj.AddComponent<HighScoreManager>();
        }
    }

    void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("MenuCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CreateMainPanel()
    {
        mainPanel = CreatePanel("MainPanel");
        
        CreateBatLogo(mainPanel.transform, new Vector2(0, 280));
        CreateStyledTitle("WHACK-A-JOKER", mainPanel.transform, new Vector2(0, 150));
        
        CreateHintText("Return to main menu to save your score!", mainPanel.transform, new Vector2(0, 70));
        
        CreateStyledButton("PLAY GAME", mainPanel.transform, new Vector2(0, -10), OnPlayClicked, true);
        CreateStyledButton("HIGH SCORES", mainPanel.transform, new Vector2(0, -100), OnHighScoresClicked, false);
        CreateStyledButton("INSTRUCTIONS", mainPanel.transform, new Vector2(0, -180), OnInstructionsClicked, false);
        CreateStyledButton("SETTINGS", mainPanel.transform, new Vector2(0, -260), OnSettingsClicked, false);
        CreateStyledButton("QUIT", mainPanel.transform, new Vector2(0, -340), OnQuitClicked, false);

        CreateFooterText("© GOTHAM CITY", mainPanel.transform);
    }

    void CreateHintText(string text, Transform parent, Vector2 position)
    {
        GameObject hintObj = new GameObject("HintText");
        hintObj.transform.SetParent(parent, false);
        RectTransform rect = hintObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(600, 30);
        TextMeshProUGUI tmp = hintObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.6f, 0.6f, 0.7f);
        tmp.fontStyle = FontStyles.Italic;
        tmp.raycastTarget = false;
    }

    void CreateInstructionsPanel()
    {
        instructionsPanel = CreatePanel("InstructionsPanel");

        CreateStyledTitle("INSTRUCTIONS", instructionsPanel.transform, new Vector2(0, 320));
        CreateDivider(instructionsPanel.transform, new Vector2(0, 270));

        GameObject contentBox = CreateContentBox(instructionsPanel.transform, new Vector2(0, 20), new Vector2(700, 400));

        string instructions = 
            "<color=#FFD100>OBJECTIVE</color>\n" +
            "Throw batarangs at the Jokers to score points!\n\n" +
            "<color=#FFD100>CONTROLS</color>\n" +
            "WASD / Arrow Keys - Move Batman\n" +
            "Space / Left Click - Throw Batarang\n" +
            "ESC - Pause Game\n\n" +
            "<color=#FFD100>GAMEPLAY</color>\n" +
            "• Jokers grow over time and respawn when too big\n" +
            "• Bigger Jokers = More points\n" +
            "• Police officers block your batarangs\n" +
            "• More police spawn as your score increases";

        CreateRichText(instructions, contentBox.transform, Vector2.zero, 28);
        
        CreateStyledButton("BACK", instructionsPanel.transform, new Vector2(0, -280), OnBackToMainClicked, false);
    }

    void CreateSettingsPanel()
    {
        settingsPanel = CreatePanel("SettingsPanel");

        CreateStyledTitle("SETTINGS", settingsPanel.transform, new Vector2(0, 320));
        CreateDivider(settingsPanel.transform, new Vector2(0, 270));

        CreateSettingsLabel("VOLUME", settingsPanel.transform, new Vector2(-250, 180));
        volumeValueText = CreateSettingsValue(settingsPanel.transform, new Vector2(250, 180));
        volumeSlider = CreateStyledSlider(settingsPanel.transform, new Vector2(0, 120));
        
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        UpdateVolumeText(savedVolume);

        muteToggle = CreateMuteToggle(settingsPanel.transform, new Vector2(0, 40));
        bool isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        muteToggle.SetIsOnWithoutNotify(isMuted);
        UpdateMuteToggleVisual(isMuted);

        CreateSettingsLabel("DIFFICULTY", settingsPanel.transform, new Vector2(0, -60));
        
        easyBtn = CreateDifficultyButton("EASY", settingsPanel.transform, new Vector2(-200, -130), 0);
        normalBtn = CreateDifficultyButton("NORMAL", settingsPanel.transform, new Vector2(0, -130), 1);
        hardBtn = CreateDifficultyButton("HARD", settingsPanel.transform, new Vector2(200, -130), 2);

        currentDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        UpdateDifficultyButtons();

        CreateRichText("<size=22><color=#888888>Easy: Slower enemies  •  Normal: Default  •  Hard: Faster enemies</color></size>", 
            settingsPanel.transform, new Vector2(0, -210), 22);

        CreateStyledButton("BACK", settingsPanel.transform, new Vector2(0, -320), OnBackToMainClicked, false);
    }

    void CreateBatLogo(Transform parent, Vector2 position)
    {
        GameObject logoObj = new GameObject("BatLogo");
        logoObj.transform.SetParent(parent, false);

        RectTransform rect = logoObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 80);

        TextMeshProUGUI tmp = logoObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "🦇";
        tmp.fontSize = 80;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
    }

    void CreateStyledTitle(string text, Transform parent, Vector2 position)
    {
        GameObject shadowObj = new GameObject("TitleShadow");
        shadowObj.transform.SetParent(parent, false);
        RectTransform shadowRect = shadowObj.AddComponent<RectTransform>();
        shadowRect.anchoredPosition = position + new Vector2(4, -4);
        shadowRect.sizeDelta = new Vector2(800, 100);
        TextMeshProUGUI shadowTmp = shadowObj.AddComponent<TextMeshProUGUI>();
        shadowTmp.text = text;
        shadowTmp.fontSize = 72;
        shadowTmp.alignment = TextAlignmentOptions.Center;
        shadowTmp.color = new Color(0, 0, 0, 0.5f);
        shadowTmp.fontStyle = FontStyles.Bold;
        shadowTmp.raycastTarget = false;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        RectTransform rect = titleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(800, 100);
        TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 72;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = batmanYellow;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
    }

    void CreateSubtitle(string text, Transform parent, Vector2 position)
    {
        GameObject subtitleObj = new GameObject("Subtitle");
        subtitleObj.transform.SetParent(parent, false);
        RectTransform rect = subtitleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 40);
        TextMeshProUGUI tmp = subtitleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.6f, 0.6f, 0.7f);
        tmp.characterSpacing = 8;
        tmp.raycastTarget = false;
    }

    void CreateFooterText(string text, Transform parent)
    {
        GameObject footerObj = new GameObject("Footer");
        footerObj.transform.SetParent(parent, false);
        RectTransform rect = footerObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, 40);
        rect.sizeDelta = new Vector2(400, 30);
        TextMeshProUGUI tmp = footerObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.4f, 0.4f, 0.5f);
        tmp.characterSpacing = 4;
        tmp.raycastTarget = false;
    }

    void CreateDivider(Transform parent, Vector2 position)
    {
        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(parent, false);
        RectTransform rect = divider.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(500, 2);
        Image img = divider.AddComponent<Image>();
        img.color = batmanYellow;
        img.raycastTarget = false;
    }

    GameObject CreateContentBox(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject box = new GameObject("ContentBox");
        box.transform.SetParent(parent, false);
        RectTransform rect = box.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Image img = box.AddComponent<Image>();
        img.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);
        img.raycastTarget = false;

        GameObject border = new GameObject("Border");
        border.transform.SetParent(box.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        Outline outline = border.AddComponent<Outline>();

        return box;
    }

    void CreateRichText(string text, Transform parent, Vector2 position, int fontSize)
    {
        GameObject textObj = new GameObject("RichText");
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(650, 380);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.85f, 0.85f, 0.9f);
        tmp.lineSpacing = 5;
        tmp.richText = true;
        tmp.raycastTarget = false;
    }

    void CreateSettingsLabel(string text, Transform parent, Vector2 position)
    {
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(parent, false);
        RectTransform rect = labelObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 50);
        TextMeshProUGUI tmp = labelObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = batmanYellow;
        tmp.fontStyle = FontStyles.Bold;
        tmp.characterSpacing = 2;
        tmp.raycastTarget = false;
    }

    TextMeshProUGUI CreateSettingsValue(Transform parent, Vector2 position)
    {
        GameObject textObj = new GameObject("ValueText");
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(120, 50);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
        return tmp;
    }

    Slider CreateStyledSlider(Transform parent, Vector2 position)
    {
        GameObject sliderObj = new GameObject("VolumeSlider");
        sliderObj.transform.SetParent(parent, false);
        RectTransform rect = sliderObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(500, 30);

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = batmanGray;

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = batmanYellow;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(15, 0);
        handleAreaRect.offsetMax = new Vector2(-15, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24, 40);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.onValueChanged.AddListener(OnVolumeChanged);

        return slider;
    }

    void CreateStyledButton(string text, Transform parent, Vector2 position, UnityEngine.Events.UnityAction onClick, bool isPrimary)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = isPrimary ? new Vector2(320, 70) : new Vector2(280, 55);

        Image img = buttonObj.AddComponent<Image>();
        img.color = isPrimary ? batmanYellow : batmanGray;
        img.raycastTarget = true;

        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = true;
        btn.onClick.AddListener(onClick);

        ColorBlock colors = btn.colors;
        if (isPrimary)
        {
            colors.normalColor = batmanYellow;
            colors.highlightedColor = new Color(1f, 0.9f, 0.3f);
            colors.pressedColor = new Color(0.8f, 0.65f, 0f);
        }
        else
        {
            colors.normalColor = batmanGray;
            colors.highlightedColor = new Color(0.35f, 0.35f, 0.5f);
            colors.pressedColor = new Color(0.15f, 0.15f, 0.25f);
        }
        btn.colors = colors;

        GameObject textChild = new GameObject("Text");
        textChild.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textChild.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textChild.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = isPrimary ? 32 : 26;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = isPrimary ? batmanDarkBlue : Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.characterSpacing = 2;
        tmp.raycastTarget = false;
    }

    Button CreateDifficultyButton(string text, Transform parent, Vector2 position, int difficulty)
    {
        GameObject buttonObj = new GameObject(text + "DiffBtn");
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(160, 55);

        Image img = buttonObj.AddComponent<Image>();
        img.color = batmanGray;
        img.raycastTarget = true;

        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = true;
        btn.onClick.AddListener(() => OnDifficultySelected(difficulty));

        GameObject textChild = new GameObject("Text");
        textChild.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textChild.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textChild.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;

        return btn;
    }

    GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = batmanDarkBlue;

        GameObject gradient = new GameObject("Gradient");
        gradient.transform.SetParent(panel.transform, false);
        RectTransform gradRect = gradient.AddComponent<RectTransform>();
        gradRect.anchorMin = Vector2.zero;
        gradRect.anchorMax = Vector2.one;
        gradRect.offsetMin = Vector2.zero;
        gradRect.offsetMax = Vector2.zero;
        Image gradImg = gradient.AddComponent<Image>();
        gradImg.color = new Color(0.02f, 0.02f, 0.08f, 0.7f);
        gradImg.raycastTarget = false;

        return panel;
    }

    void OnDifficultySelected(int difficulty)
    {
        currentDifficulty = difficulty;
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();
        UpdateDifficultyButtons();
    }

    void UpdateDifficultyButtons()
    {
        UpdateDifficultyButtonVisual(easyBtn, currentDifficulty == 0);
        UpdateDifficultyButtonVisual(normalBtn, currentDifficulty == 1);
        UpdateDifficultyButtonVisual(hardBtn, currentDifficulty == 2);
    }

    void UpdateDifficultyButtonVisual(Button btn, bool selected)
    {
        Image img = btn.GetComponent<Image>();
        TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        
        if (selected)
        {
            img.color = batmanYellow;
            txt.color = batmanDarkBlue;
        }
        else
        {
            img.color = batmanGray;
            txt.color = Color.white;
        }
    }

    void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeValueText.text = percent + "%";
    }

    Toggle CreateMuteToggle(Transform parent, Vector2 position)
    {
        GameObject toggleObj = new GameObject("MuteToggle");
        toggleObj.transform.SetParent(parent, false);
        RectTransform rect = toggleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 50);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        GameObject label = new GameObject("Label");
        label.transform.SetParent(toggleObj.transform, false);
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.anchoredPosition = new Vector2(80, 0);
        labelRect.sizeDelta = new Vector2(150, 50);
        TextMeshProUGUI labelTmp = label.AddComponent<TextMeshProUGUI>();
        labelTmp.text = "MUTE";
        labelTmp.fontSize = 28;
        labelTmp.alignment = TextAlignmentOptions.Left;
        labelTmp.color = Color.white;
        labelTmp.fontStyle = FontStyles.Bold;
        labelTmp.raycastTarget = false;

        GameObject switchBg = new GameObject("SwitchBackground");
        switchBg.transform.SetParent(toggleObj.transform, false);
        RectTransform switchBgRect = switchBg.AddComponent<RectTransform>();
        switchBgRect.anchorMin = new Vector2(1, 0.5f);
        switchBgRect.anchorMax = new Vector2(1, 0.5f);
        switchBgRect.anchoredPosition = new Vector2(-60, 0);
        switchBgRect.sizeDelta = new Vector2(80, 36);
        Image switchBgImg = switchBg.AddComponent<Image>();
        switchBgImg.color = batmanGray;

        GameObject switchOn = new GameObject("SwitchOn");
        switchOn.transform.SetParent(switchBg.transform, false);
        RectTransform switchOnRect = switchOn.AddComponent<RectTransform>();
        switchOnRect.anchorMin = Vector2.zero;
        switchOnRect.anchorMax = Vector2.one;
        switchOnRect.offsetMin = new Vector2(2, 2);
        switchOnRect.offsetMax = new Vector2(-2, -2);
        Image switchOnImg = switchOn.AddComponent<Image>();
        switchOnImg.color = batmanYellow;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(switchBg.transform, false);
        muteHandleRect = handle.AddComponent<RectTransform>();
        muteHandleRect.anchorMin = new Vector2(1, 0.5f);
        muteHandleRect.anchorMax = new Vector2(1, 0.5f);
        muteHandleRect.anchoredPosition = new Vector2(-6, 0);
        muteHandleRect.sizeDelta = new Vector2(28, 28);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;

        GameObject offLabel = new GameObject("OffLabel");
        offLabel.transform.SetParent(switchBg.transform, false);
        RectTransform offRect = offLabel.AddComponent<RectTransform>();
        offRect.anchorMin = new Vector2(0, 0);
        offRect.anchorMax = new Vector2(0.5f, 1);
        offRect.offsetMin = Vector2.zero;
        offRect.offsetMax = Vector2.zero;
        TextMeshProUGUI offTmp = offLabel.AddComponent<TextMeshProUGUI>();
        offTmp.text = "OFF";
        offTmp.fontSize = 14;
        offTmp.alignment = TextAlignmentOptions.Center;
        offTmp.color = new Color(0.3f, 0.3f, 0.3f);
        offTmp.fontStyle = FontStyles.Bold;
        offTmp.raycastTarget = false;

        GameObject onLabel = new GameObject("OnLabel");
        onLabel.transform.SetParent(switchBg.transform, false);
        RectTransform onRect = onLabel.AddComponent<RectTransform>();
        onRect.anchorMin = new Vector2(0.5f, 0);
        onRect.anchorMax = new Vector2(1, 1);
        onRect.offsetMin = Vector2.zero;
        onRect.offsetMax = Vector2.zero;
        TextMeshProUGUI onTmp = onLabel.AddComponent<TextMeshProUGUI>();
        onTmp.text = "ON";
        onTmp.fontSize = 14;
        onTmp.alignment = TextAlignmentOptions.Center;
        onTmp.color = batmanDarkBlue;
        onTmp.fontStyle = FontStyles.Bold;
        onTmp.raycastTarget = false;

        toggle.targetGraphic = switchBgImg;
        toggle.graphic = switchOnImg;
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(OnMuteToggleChanged);

        return toggle;
    }

    void OnMuteToggleChanged(bool isOn)
    {
        UpdateMuteToggleVisual(isOn);
        OnMuteToggled(isOn);
    }

    void UpdateMuteToggleVisual(bool isOn)
    {
        if (muteHandleRect != null)
            muteHandleRect.anchoredPosition = new Vector2(isOn ? -6 : -68, 0);
    }

    void OnMuteToggled(bool isMuted)
    {
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        if (MusicManager.Instance != null)
        {
            if (isMuted != MusicManager.Instance.IsMuted())
                MusicManager.Instance.ToggleMute();
        }
        
        AudioListener.volume = isMuted ? 0f : PlayerPrefs.GetFloat("Volume", 1f);
    }

    void CreateHighScoresPanel()
    {
        highScoresPanel = CreatePanel("HighScoresPanel");

        CreateStyledTitle("HIGH SCORES", highScoresPanel.transform, new Vector2(0, 320));
        CreateDivider(highScoresPanel.transform, new Vector2(0, 270));

        GameObject scoreBox = CreateContentBox(highScoresPanel.transform, new Vector2(0, 20), new Vector2(500, 350));
        
        scoreListContainer = new GameObject("ScoreList");
        scoreListContainer.transform.SetParent(scoreBox.transform, false);
        RectTransform listRect = scoreListContainer.AddComponent<RectTransform>();
        listRect.anchorMin = Vector2.zero;
        listRect.anchorMax = Vector2.one;
        listRect.offsetMin = new Vector2(20, 20);
        listRect.offsetMax = new Vector2(-20, -20);

        CreateStyledButton("BACK", highScoresPanel.transform, new Vector2(0, -280), OnBackToMainClicked, false);
    }

    void RefreshHighScoresList()
    {
        foreach (Transform child in scoreListContainer.transform)
            Destroy(child.gameObject);

        var scores = HighScoreManager.Instance?.HighScores;
        if (scores == null || scores.Count == 0)
        {
            CreateEmptyScoresMessage(scoreListContainer.transform);
            return;
        }

        for (int i = 0; i < scores.Count && i < 5; i++)
        {
            CreateScoreEntry(scoreListContainer.transform, i, scores[i].playerName, scores[i].score);
        }
    }

    void CreateEmptyScoresMessage(Transform parent)
    {
        GameObject msgObj = new GameObject("EmptyMessage");
        msgObj.transform.SetParent(parent, false);
        RectTransform rect = msgObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = msgObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "NO SCORES YET\n\nPlay a game and return to\nmain menu to save your score!";
        tmp.fontSize = 26;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.5f, 0.5f, 0.6f);
        tmp.raycastTarget = false;
    }

    void CreateScoreEntry(Transform parent, int rank, string playerName, int score)
    {
        GameObject entry = new GameObject("ScoreEntry" + rank);
        entry.transform.SetParent(parent, false);
        RectTransform rect = entry.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -rank * 55);
        rect.sizeDelta = new Vector2(0, 50);

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(entry.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = rank == 0 ? new Color(1f, 0.82f, 0.04f, 0.3f) : new Color(0.15f, 0.15f, 0.2f, 0.5f);
        bgImg.raycastTarget = false;

        GameObject rankObj = new GameObject("Rank");
        rankObj.transform.SetParent(entry.transform, false);
        RectTransform rankRect = rankObj.AddComponent<RectTransform>();
        rankRect.anchorMin = new Vector2(0, 0);
        rankRect.anchorMax = new Vector2(0, 1);
        rankRect.anchoredPosition = new Vector2(40, 0);
        rankRect.sizeDelta = new Vector2(60, 0);
        TextMeshProUGUI rankTmp = rankObj.AddComponent<TextMeshProUGUI>();
        rankTmp.text = "#" + (rank + 1);
        rankTmp.fontSize = 28;
        rankTmp.alignment = TextAlignmentOptions.Left;
        rankTmp.color = rank == 0 ? batmanYellow : Color.white;
        rankTmp.fontStyle = FontStyles.Bold;
        rankTmp.raycastTarget = false;

        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(entry.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.anchoredPosition = new Vector2(0, 0);
        nameRect.offsetMin = new Vector2(100, 0);
        nameRect.offsetMax = new Vector2(-100, 0);
        TextMeshProUGUI nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
        nameTmp.text = playerName;
        nameTmp.fontSize = 26;
        nameTmp.alignment = TextAlignmentOptions.Left;
        nameTmp.color = rank == 0 ? batmanYellow : new Color(0.8f, 0.8f, 0.85f);
        nameTmp.raycastTarget = false;

        GameObject scoreObj = new GameObject("Score");
        scoreObj.transform.SetParent(entry.transform, false);
        RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(1, 0);
        scoreRect.anchorMax = new Vector2(1, 1);
        scoreRect.anchoredPosition = new Vector2(-50, 0);
        scoreRect.sizeDelta = new Vector2(100, 0);
        TextMeshProUGUI scoreTmp = scoreObj.AddComponent<TextMeshProUGUI>();
        scoreTmp.text = score.ToString();
        scoreTmp.fontSize = 28;
        scoreTmp.alignment = TextAlignmentOptions.Right;
        scoreTmp.color = rank == 0 ? batmanYellow : Color.white;
        scoreTmp.fontStyle = FontStyles.Bold;
        scoreTmp.raycastTarget = false;
    }

    void CreateNameInputPanel()
    {
        nameInputPanel = CreatePanel("NameInputPanel");

        CreateStyledTitle("GAME OVER", nameInputPanel.transform, new Vector2(0, 280));
        CreateDivider(nameInputPanel.transform, new Vector2(0, 230));

        pendingScoreText = CreateCenteredText("YOUR SCORE: 0", nameInputPanel.transform, new Vector2(0, 160), 36);
        pendingScoreText.color = batmanYellow;

        CreateCenteredText("ENTER YOUR NAME", nameInputPanel.transform, new Vector2(0, 80), 28);

        nameInputField = CreateInputField(nameInputPanel.transform, new Vector2(0, 10));

        CreateStyledButton("SAVE SCORE", nameInputPanel.transform, new Vector2(0, -100), OnSaveScoreClicked, true);
        CreateStyledButton("SKIP", nameInputPanel.transform, new Vector2(0, -180), OnSkipClicked, false);
    }

    TMP_InputField CreateInputField(Transform parent, Vector2 position)
    {
        GameObject inputObj = new GameObject("NameInput");
        inputObj.transform.SetParent(parent, false);
        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 60);

        Image bg = inputObj.AddComponent<Image>();
        bg.color = batmanGray;

        TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
        input.characterLimit = 12;

        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform taRect = textArea.AddComponent<RectTransform>();
        taRect.anchorMin = Vector2.zero;
        taRect.anchorMax = Vector2.one;
        taRect.offsetMin = new Vector2(10, 5);
        taRect.offsetMax = new Vector2(-10, -5);
        textArea.AddComponent<RectMask2D>();

        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform, false);
        RectTransform phRect = placeholder.AddComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = Vector2.zero;
        phRect.offsetMax = Vector2.zero;
        TextMeshProUGUI phTmp = placeholder.AddComponent<TextMeshProUGUI>();
        phTmp.text = "BATMAN";
        phTmp.fontSize = 28;
        phTmp.alignment = TextAlignmentOptions.Center;
        phTmp.color = new Color(0.5f, 0.5f, 0.6f);
        phTmp.fontStyle = FontStyles.Italic;

        GameObject text = new GameObject("Text");
        text.transform.SetParent(textArea.transform, false);
        RectTransform txtRect = text.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
        TextMeshProUGUI txtTmp = text.AddComponent<TextMeshProUGUI>();
        txtTmp.fontSize = 28;
        txtTmp.alignment = TextAlignmentOptions.Center;
        txtTmp.color = Color.white;
        txtTmp.fontStyle = FontStyles.Bold;

        input.textViewport = taRect;
        input.textComponent = txtTmp;
        input.placeholder = phTmp;

        return input;
    }

    TextMeshProUGUI CreateCenteredText(string text, Transform parent, Vector2 position, int fontSize)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(600, 50);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    void ShowNameInputPanel()
    {
        mainPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        highScoresPanel.SetActive(false);
        nameInputPanel.SetActive(true);
        
        pendingScoreText.text = "YOUR SCORE: " + pendingScore;
        nameInputField.text = "";
    }

    void OnSaveScoreClicked()
    {
        string playerName = nameInputField.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(playerName))
            playerName = "BATMAN";
        
        HighScoreManager.Instance?.TryAddScore(playerName, pendingScore);
        pendingScore = 0;
        ShowMainPanel();
    }

    void OnSkipClicked()
    {
        pendingScore = 0;
        ShowMainPanel();
    }

    void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        highScoresPanel.SetActive(false);
        nameInputPanel.SetActive(false);
    }

    void OnPlayClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OnInstructionsClicked()
    {
        mainPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    void OnHighScoresClicked()
    {
        mainPanel.SetActive(false);
        highScoresPanel.SetActive(true);
        RefreshHighScoresList();
    }

    void OnSettingsClicked()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);
        
        bool isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        muteToggle.SetIsOnWithoutNotify(isMuted);
        UpdateMuteToggleVisual(isMuted);
        
        currentDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        UpdateDifficultyButtons();
    }

    void OnBackToMainClicked()
    {
        ShowMainPanel();
    }

    void OnQuitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        UpdateVolumeText(value);
        
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(value);
    }
}
