using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ArcadeMenuStyle
{
    public static readonly Color Ink = new Color32(16, 11, 41, 255);
    public static readonly Color Panel = new Color32(39, 26, 77, 255);
    public static readonly Color Mint = new Color32(69, 247, 210, 255);
    public static readonly Color Pink = new Color32(255, 114, 201, 255);
    public static readonly Color Gold = new Color32(255, 215, 87, 255);
    public static readonly Color Lilac = new Color32(219, 206, 255, 255);

    public static void Backdrop(Transform parent)
    {
        Image background = Block(parent, "Arcade backdrop", Vector2.zero, Vector2.zero, Ink);
        RectTransform rect = background.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.SetAsFirstSibling();
        for (int i = 0; i < 4; i++)
        {
            float y = 120 + i * 96;
            Image left = Block(rect, "Neon stripe", new Vector2(-600, -y), new Vector2(1100, 30), Panel);
            Image right = Block(rect, "Neon stripe", new Vector2(600, y), new Vector2(1100, 30), Panel);
            left.rectTransform.localRotation = Quaternion.Euler(0, 0, 26);
            right.rectTransform.localRotation = left.rectTransform.localRotation;
        }
        for (int i = 0; i < 3; i++)
        {
            Color color = Accent(i);
            Image left = Block(rect, "Pixel spark", new Vector2(-550 + i * 30, 225 - i * 42), new Vector2(14, 14), color);
            Image right = Block(rect, "Pixel spark", new Vector2(550 - i * 30, -225 + i * 42), new Vector2(14, 14), color);
            left.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
            right.rectTransform.localRotation = left.rectTransform.localRotation;
        }
    }

    public static Color Accent(int index) => index == 0 ? Mint : index == 1 ? Pink : Gold;

    public static Image Block(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject item = new GameObject(name, typeof(RectTransform), typeof(Image));
        item.transform.SetParent(parent, false);
        Image image = item.GetComponent<Image>();
        image.rectTransform.anchoredPosition = position;
        image.rectTransform.sizeDelta = size;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    public static void Heading(TMP_Text label, Color accent)
    {
        label.color = accent;
        label.fontStyle |= FontStyles.Bold;
        label.characterSpacing = 2;
    }

    public static void Button(Button button, Color accent)
    {
        RectTransform rect = (RectTransform)button.transform;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null && label.transform == rect)
        {
            string title = label.text;
            label.enabled = false;
            label = LevelSelectMenu.Text(rect, title, Vector2.zero, rect.sizeDelta, 30);
        }

        Image surface = button.targetGraphic as Image;
        if (surface == null)
        {
            surface = Block(rect, "Button surface", Vector2.zero, Vector2.zero, Color.white);
            surface.rectTransform.anchorMin = Vector2.zero;
            surface.rectTransform.anchorMax = Vector2.one;
            surface.transform.SetAsFirstSibling();
        }
        surface.color = Color.white;
        surface.raycastTarget = true;
        button.targetGraphic = surface;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = accent;
        colors.highlightedColor = Color.Lerp(accent, Color.white, 0.45f);
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = Color.Lerp(accent, Ink, 0.28f);
        button.colors = colors;
        if (label != null)
        {
            label.color = Ink;
            label.fontStyle |= FontStyles.Bold;
            label.raycastTarget = false;
        }
    }

    public static void Overlay(GameObject panel, Color accent, Vector2 size)
    {
        Image scrim = panel.GetComponent<Image>();
        if (scrim != null) scrim.color = new Color(Ink.r, Ink.g, Ink.b, 0.9f);
        Transform card = Block(panel.transform, "Arcade frame", Vector2.zero, size, Panel).transform;
        card.SetAsFirstSibling();
        Block(card, "Top accent", new Vector2(0, size.y / 2 - 4), new Vector2(size.x, 8), accent);
        Block(card, "Bottom accent", new Vector2(0, -size.y / 2 + 2), new Vector2(size.x, 4), accent);
        foreach (TMP_Text label in panel.GetComponentsInChildren<TMP_Text>(true))
        {
            label.color = Lilac;
            if (label.name.EndsWith("Title") || label.name == "PauseText")
                Heading(label, accent);
        }
        foreach (Button button in panel.GetComponentsInChildren<Button>(true))
        {
            RectTransform rect = (RectTransform)button.transform;
            rect.sizeDelta = new Vector2(300, 60);
            Button(button, accent);
        }
    }

    public static void TitleScreen(Canvas canvas, GameObject options, GameObject credits, Slider volume, Toggle fullscreen)
    {
        Transform root = canvas.transform;
        Image existing = root.Find("Background")?.GetComponent<Image>();
        if (existing != null) existing.enabled = false;
        Backdrop(root);
        TMP_Text title = root.Find("GameTitle").GetComponent<TMP_Text>();
        title.text = "COIN RUSH";
        title.fontSize = 112;
        title.rectTransform.sizeDelta = new Vector2(1100, 150);
        title.rectTransform.anchoredPosition = new Vector2(0, 200);
        Heading(title, Gold);
        Heading(LevelSelectMenu.Text(root, "ARCADE  /  01", new Vector2(0, 328), new Vector2(540, 48), 27), Pink);
        LevelSelectMenu.Text(root, "GRAB COINS. DODGE CHASERS. BEAT YOUR TIME.", new Vector2(0, 102), new Vector2(1050, 52), 29).color = Lilac;
        for (int i = 0; i < 3; i++)
        {
            string name = i == 0 ? "StartButton" : i == 1 ? "OptionsButton" : "CreditsButton";
            Transform item = root.Find(name);
            RectTransform rect = (RectTransform)item;
            rect.anchoredPosition = new Vector2(0, -8 - i * 93);
            rect.sizeDelta = new Vector2(420, 68);
            Button(item.GetComponent<Button>(), Accent(i));
        }
        Overlay(options, Mint, new Vector2(850, 830));
        Overlay(credits, Pink, new Vector2(980, 900));
        credits.transform.Find("CreditsGameTitle").GetComponent<TMP_Text>().text = "COIN RUSH";
        if (volume.fillRect != null) volume.fillRect.GetComponent<Image>().color = Mint;
        if (volume.handleRect != null) volume.handleRect.GetComponent<Image>().color = Gold;
        if (fullscreen.graphic != null) fullscreen.graphic.color = Mint;
        options.transform.SetAsLastSibling();
        credits.transform.SetAsLastSibling();
    }
}
