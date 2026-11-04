using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        GameObject cameraObject = new GameObject("Menu Camera", typeof(Camera), typeof(AudioListener));
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.025f, 0.05f, 0.1f);
        GameObject canvasObject = new GameObject("Level Select", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        Transform canvas = canvasObject.transform;
        ArcadeMenuStyle.Backdrop(canvas);
        ArcadeMenuStyle.Heading(Text(canvas, "COIN RUSH", new Vector2(0, 278), new Vector2(1100, 75), 58), ArcadeMenuStyle.Gold);
        ArcadeMenuStyle.Heading(Text(canvas, "CHOOSE YOUR ARENA", new Vector2(0, 216), new Vector2(1100, 45), 24), ArcadeMenuStyle.Lilac);
        Button first = null;
        for (int i = 0; i < LevelProgress.Count; i++)
        {
            int level = i;
            float x = (i - 1) * 385;
            float best = LevelProgress.BestTime(i);
            string record = best < 0 ? "No wins yet" :
                "BEST " + LevelProgress.TimeLabel(best) + "  |  " + LevelProgress.Stars(i, best) + " / 3 STARS\n" +
                "LAST " + LevelProgress.TimeLabel(LevelProgress.LastTime(i)) + "  |  " + LevelProgress.Wins(i) + " WINS";
            Color accent = ArcadeMenuStyle.Accent(i);
            string details = "<b><color=#" + ColorUtility.ToHtmlStringRGB(accent) + ">LEVEL " +
                (i + 1) + "\n" + LevelProgress.Names[i] + "</color></b>\n\n" +
                LevelProgress.Targets[i] + " POINTS  /  " + LevelProgress.Enemies[i] + " ENEMIES\n\n" +
                record + "\n\n3 stars  <= " + LevelProgress.ThreeStarTimes[i] + "s\n2 stars  <= " +
                LevelProgress.TwoStarTimes[i] + "s\n1 star  Complete the level";
            GameObject card = new GameObject("Level " + (i + 1), typeof(RectTransform), typeof(Image));
            card.transform.SetParent(canvas, false);
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(360, 380);
            rect.anchoredPosition = new Vector2(x, -10);
            card.GetComponent<Image>().color = ArcadeMenuStyle.Panel;
            ArcadeMenuStyle.Block(card.transform, "Level accent", new Vector2(0, 187), new Vector2(360, 6), accent);
            Text(card.transform, details, new Vector2(0, 25), new Vector2(340, 310), 22);
            Button button = AddButton(card.transform, "PLAY LEVEL " + (i + 1), new Vector2(0, -147),
                () => SceneManager.LoadScene(LevelProgress.Scenes[level]));
            ArcadeMenuStyle.Button(button, accent);
            if (first == null) first = button;
        }
        Text(canvas, "WASD move   /   SPACE jump   /   ESC pause     |     Pause time is excluded", new Vector2(0, -246), new Vector2(1180, 45), 20);
        Button back = AddButton(canvas, "MAIN MENU", new Vector2(0, -307), () => SceneManager.LoadScene("MainMenu"));
        ArcadeMenuStyle.Button(back, ArcadeMenuStyle.Lilac);
        EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    public static TMP_Text Text(Transform parent, string text, Vector2 position, Vector2 size, float fontSize)
    {
        GameObject item = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        item.transform.SetParent(parent, false);
        TMP_Text label = item.GetComponent<TMP_Text>();
        label.rectTransform.sizeDelta = size;
        label.rectTransform.anchoredPosition = position;
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    public static Button AddButton(Transform parent, string title, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject item = new GameObject(title, typeof(RectTransform), typeof(Image), typeof(Button));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 54);
        rect.anchoredPosition = position;
        item.GetComponent<Image>().color = new Color(0.02f, 0.4f, 0.55f);
        Button button = item.GetComponent<Button>();
        button.targetGraphic = item.GetComponent<Image>();
        button.onClick.AddListener(action);
        Text(item.transform, title, Vector2.zero, rect.sizeDelta, 23);
        return button;
    }
}
