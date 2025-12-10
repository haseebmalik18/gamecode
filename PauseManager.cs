using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    Canvas canvas;
    GameObject pausePanel;
    GameObject pauseButton;
    GameObject muteButton;
    GameObject muteIcon;
    GameObject unmuteIcon;
    bool isPaused = false;

    Color batmanYellow = new Color(1f, 0.82f, 0.04f);
    Color batmanDarkBlue = new Color(0.05f, 0.05f, 0.15f);
    Color batmanGray = new Color(0.2f, 0.2f, 0.3f);

    void Start()
    {
        CreatePauseUI();
        pausePanel.SetActive(false);
        UpdateMuteIcon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void CreatePauseUI()
    {
        GameObject canvasObj = new GameObject("PauseCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        CreatePauseButton();
        CreateMuteButton();

        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvas.transform, false);

        RectTransform rect = pausePanel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = pausePanel.AddComponent<Image>();
        bg.color = new Color(0.02f, 0.02f, 0.08f, 0.95f);

        CreatePauseContent();
    }

    void CreatePauseContent()
    {
        GameObject box = new GameObject("PauseBox");
        box.transform.SetParent(pausePanel.transform, false);
        RectTransform boxRect = box.AddComponent<RectTransform>();
        boxRect.anchoredPosition = Vector2.zero;
        boxRect.sizeDelta = new Vector2(400, 450);
        Image boxImg = box.AddComponent<Image>();
        boxImg.color = batmanDarkBlue;

        GameObject border = new GameObject("Border");
        border.transform.SetParent(box.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        Image borderImg = border.AddComponent<Image>();
        borderImg.color = batmanYellow;
        borderImg.raycastTarget = false;
        
        GameObject innerBg = new GameObject("InnerBg");
        innerBg.transform.SetParent(box.transform, false);
        RectTransform innerRect = innerBg.AddComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(3, 3);
        innerRect.offsetMax = new Vector2(-3, -3);
        Image innerImg = innerBg.AddComponent<Image>();
        innerImg.color = batmanDarkBlue;
        innerImg.raycastTarget = false;

        CreatePauseTitle("PAUSED", box.transform, new Vector2(0, 160));
        CreateDivider(box.transform, new Vector2(0, 110));
        CreateStyledButton("RESUME", box.transform, new Vector2(0, 40), Resume);
        CreateStyledButton("MAIN MENU", box.transform, new Vector2(0, -50), GoToMainMenu);
        CreateStyledButton("QUIT GAME", box.transform, new Vector2(0, -140), QuitGame);
    }

    void CreatePauseButton()
    {
        pauseButton = new GameObject("PauseButton");
        pauseButton.transform.SetParent(canvas.transform, false);

        RectTransform rect = pauseButton.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-20, -20);
        rect.sizeDelta = new Vector2(50, 50);

        Image img = pauseButton.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.6f);

        Button btn = pauseButton.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(Pause);

        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        btn.colors = colors;

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(pauseButton.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = new Vector2(14, 10);
        iconRect.offsetMax = new Vector2(-14, -10);

        GameObject bar1 = new GameObject("Bar1");
        bar1.transform.SetParent(iconObj.transform, false);
        RectTransform bar1Rect = bar1.AddComponent<RectTransform>();
        bar1Rect.anchorMin = new Vector2(0, 0);
        bar1Rect.anchorMax = new Vector2(0.35f, 1);
        bar1Rect.offsetMin = Vector2.zero;
        bar1Rect.offsetMax = Vector2.zero;
        Image bar1Img = bar1.AddComponent<Image>();
        bar1Img.color = batmanYellow;
        bar1Img.raycastTarget = false;

        GameObject bar2 = new GameObject("Bar2");
        bar2.transform.SetParent(iconObj.transform, false);
        RectTransform bar2Rect = bar2.AddComponent<RectTransform>();
        bar2Rect.anchorMin = new Vector2(0.65f, 0);
        bar2Rect.anchorMax = new Vector2(1, 1);
        bar2Rect.offsetMin = Vector2.zero;
        bar2Rect.offsetMax = Vector2.zero;
        Image bar2Img = bar2.AddComponent<Image>();
        bar2Img.color = batmanYellow;
        bar2Img.raycastTarget = false;
    }

    void CreateMuteButton()
    {
        muteButton = new GameObject("MuteButton");
        muteButton.transform.SetParent(canvas.transform, false);

        RectTransform rect = muteButton.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-80, -20);
        rect.sizeDelta = new Vector2(50, 50);

        Image img = muteButton.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.6f);

        Button btn = muteButton.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(ToggleMute);

        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        btn.colors = colors;

        unmuteIcon = new GameObject("UnmuteIcon");
        unmuteIcon.transform.SetParent(muteButton.transform, false);
        RectTransform unmuteRect = unmuteIcon.AddComponent<RectTransform>();
        unmuteRect.anchorMin = Vector2.zero;
        unmuteRect.anchorMax = Vector2.one;
        unmuteRect.offsetMin = new Vector2(8, 8);
        unmuteRect.offsetMax = new Vector2(-8, -8);
        
        GameObject bar1 = new GameObject("Bar1");
        bar1.transform.SetParent(unmuteIcon.transform, false);
        RectTransform b1Rect = bar1.AddComponent<RectTransform>();
        b1Rect.anchorMin = new Vector2(0.1f, 0.3f);
        b1Rect.anchorMax = new Vector2(0.35f, 0.7f);
        b1Rect.offsetMin = Vector2.zero;
        b1Rect.offsetMax = Vector2.zero;
        Image b1Img = bar1.AddComponent<Image>();
        b1Img.color = batmanYellow;
        b1Img.raycastTarget = false;
        
        GameObject bar2 = new GameObject("Bar2");
        bar2.transform.SetParent(unmuteIcon.transform, false);
        RectTransform b2Rect = bar2.AddComponent<RectTransform>();
        b2Rect.anchorMin = new Vector2(0.35f, 0.15f);
        b2Rect.anchorMax = new Vector2(0.6f, 0.85f);
        b2Rect.offsetMin = Vector2.zero;
        b2Rect.offsetMax = Vector2.zero;
        Image b2Img = bar2.AddComponent<Image>();
        b2Img.color = batmanYellow;
        b2Img.raycastTarget = false;
        
        GameObject bar3 = new GameObject("Bar3");
        bar3.transform.SetParent(unmuteIcon.transform, false);
        RectTransform b3Rect = bar3.AddComponent<RectTransform>();
        b3Rect.anchorMin = new Vector2(0.6f, 0f);
        b3Rect.anchorMax = new Vector2(0.9f, 1f);
        b3Rect.offsetMin = Vector2.zero;
        b3Rect.offsetMax = Vector2.zero;
        Image b3Img = bar3.AddComponent<Image>();
        b3Img.color = batmanYellow;
        b3Img.raycastTarget = false;

        muteIcon = new GameObject("MuteIcon");
        muteIcon.transform.SetParent(muteButton.transform, false);
        RectTransform muteRect = muteIcon.AddComponent<RectTransform>();
        muteRect.anchorMin = Vector2.zero;
        muteRect.anchorMax = Vector2.one;
        muteRect.offsetMin = new Vector2(8, 8);
        muteRect.offsetMax = new Vector2(-8, -8);
        
        GameObject mutedBar1 = new GameObject("Bar1");
        mutedBar1.transform.SetParent(muteIcon.transform, false);
        RectTransform mb1Rect = mutedBar1.AddComponent<RectTransform>();
        mb1Rect.anchorMin = new Vector2(0.1f, 0.3f);
        mb1Rect.anchorMax = new Vector2(0.35f, 0.7f);
        mb1Rect.offsetMin = Vector2.zero;
        mb1Rect.offsetMax = Vector2.zero;
        Image mb1Img = mutedBar1.AddComponent<Image>();
        mb1Img.color = batmanGray;
        mb1Img.raycastTarget = false;
        
        GameObject mutedBar2 = new GameObject("Bar2");
        mutedBar2.transform.SetParent(muteIcon.transform, false);
        RectTransform mb2Rect = mutedBar2.AddComponent<RectTransform>();
        mb2Rect.anchorMin = new Vector2(0.35f, 0.15f);
        mb2Rect.anchorMax = new Vector2(0.6f, 0.85f);
        mb2Rect.offsetMin = Vector2.zero;
        mb2Rect.offsetMax = Vector2.zero;
        Image mb2Img = mutedBar2.AddComponent<Image>();
        mb2Img.color = batmanGray;
        mb2Img.raycastTarget = false;
        
        GameObject mutedBar3 = new GameObject("Bar3");
        mutedBar3.transform.SetParent(muteIcon.transform, false);
        RectTransform mb3Rect = mutedBar3.AddComponent<RectTransform>();
        mb3Rect.anchorMin = new Vector2(0.6f, 0f);
        mb3Rect.anchorMax = new Vector2(0.9f, 1f);
        mb3Rect.offsetMin = Vector2.zero;
        mb3Rect.offsetMax = Vector2.zero;
        Image mb3Img = mutedBar3.AddComponent<Image>();
        mb3Img.color = batmanGray;
        mb3Img.raycastTarget = false;
        
        GameObject slash = new GameObject("Slash");
        slash.transform.SetParent(muteIcon.transform, false);
        RectTransform slashRect = slash.AddComponent<RectTransform>();
        slashRect.anchorMin = Vector2.zero;
        slashRect.anchorMax = Vector2.one;
        slashRect.offsetMin = Vector2.zero;
        slashRect.offsetMax = Vector2.zero;
        slashRect.localRotation = Quaternion.Euler(0, 0, -45);
        Image slashImg = slash.AddComponent<Image>();
        slashImg.color = Color.red;
        slashImg.raycastTarget = false;
        
        GameObject slashLine = new GameObject("SlashLine");
        slashLine.transform.SetParent(slash.transform, false);
        RectTransform lineRect = slashLine.AddComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.4f, 0);
        lineRect.anchorMax = new Vector2(0.6f, 1);
        lineRect.offsetMin = Vector2.zero;
        lineRect.offsetMax = Vector2.zero;
        Image lineImg = slashLine.AddComponent<Image>();
        lineImg.color = Color.red;
        lineImg.raycastTarget = false;
        
        Destroy(slashImg);
    }

    void ToggleMute()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMute();
            UpdateMuteIcon();
        }
    }

    void UpdateMuteIcon()
    {
        bool muted = MusicManager.Instance != null && MusicManager.Instance.IsMuted();
        muteIcon.SetActive(muted);
        unmuteIcon.SetActive(!muted);
    }

    void CreatePauseTitle(string text, Transform parent, Vector2 position)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        RectTransform rect = titleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(350, 70);
        TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 52;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = batmanYellow;
        tmp.fontStyle = FontStyles.Bold;
        tmp.characterSpacing = 6;
        tmp.raycastTarget = false;
    }

    void CreateDivider(Transform parent, Vector2 position)
    {
        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(parent, false);
        RectTransform rect = divider.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 2);
        Image img = divider.AddComponent<Image>();
        img.color = batmanYellow;
        img.raycastTarget = false;
    }

    void CreateStyledButton(string text, Transform parent, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280, 55);

        Image img = buttonObj.AddComponent<Image>();
        img.color = batmanGray;
        img.raycastTarget = true;

        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        ColorBlock colors = btn.colors;
        colors.normalColor = batmanGray;
        colors.highlightedColor = batmanYellow;
        colors.pressedColor = new Color(0.8f, 0.65f, 0f);
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
        tmp.fontSize = 26;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
    }

    void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
        muteButton.SetActive(false);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        muteButton.SetActive(true);
        Time.timeScale = 1f;
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        
        if (GameData.Instance != null && GameData.Instance.CurrentScore > 0)
        {
            PlayerPrefs.SetInt("PendingScore", GameData.Instance.CurrentScore);
            PlayerPrefs.Save();
        }
        
        SceneManager.LoadScene("MainMenu");
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
