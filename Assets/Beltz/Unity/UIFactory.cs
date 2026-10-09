using System;
using UnityEngine;
using UnityEngine.UI;
using static Beltz.Unity.UITheme;

namespace Beltz.Unity
{
    // Owns reusable visual elements and generated assets, without game or navigation state.
    internal sealed class UIFactory : IDisposable
    {
        readonly Material glassMaterial;
        readonly Font mediumFont;
        readonly Sprite rounded;
        readonly Texture2D roundedTexture;
        public Font BoldFont { get; }

        public UIFactory(Material glassMaterial)
        {
            this.glassMaterial = glassMaterial;
            mediumFont = Resources.Load<Font>("Fonts/Montserrat-Medium");
            BoldFont = Resources.Load<Font>("Fonts/Montserrat-Bold");
            if (mediumFont == null || BoldFont == null)
                throw new InvalidOperationException("Montserrat 500 and 700 font assets are required.");

            const int size = 128;
            roundedTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "UI rounded mask",
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x - 63.5f) - 1, 0);
                    float dy = Mathf.Max(Mathf.Abs(y - 63.5f) - 1, 0);
                    pixels[y * size + x] = new Color(1, 1, 1, Mathf.Clamp01(63 - Mathf.Sqrt(dx * dx + dy * dy)));
                }
            roundedTexture.SetPixels(pixels);
            roundedTexture.Apply(false, true);
            rounded = Sprite.Create(roundedTexture, new Rect(0, 0, size, size), Vector2.one * .5f,
                100, 0, SpriteMeshType.FullRect, new Vector4(63, 63, 63, 63));
        }

        public RectTransform Rect(string name, Transform parent, float x, float y, float x2, float y2)
        {
            var element = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            var rect = element.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(x, y);
            rect.anchorMax = new Vector2(x2, y2);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            return rect;
        }

        public Image Fill(RectTransform rect, Color color, bool glass = false)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = rounded;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 1.1f;
            image.color = color;
            if (glass && glassMaterial != null)
            {
                image.material = glassMaterial;
                image.color = new Color(color.r, color.g, color.b, 1);
                rect.gameObject.AddComponent<GlassSurface>();
            }
            return image;
        }

        public RectTransform Panel(string name, Transform parent, float x, float y, float x2, float y2, bool glass = true)
        {
            var rect = Rect(name, parent, x, y, x2, y2);
            Fill(rect, new Color(1, 1, 1, .30f), glass);
            return rect;
        }

        public Text Label(Transform parent, string value, float x, float y, float x2, float y2,
            int size, bool bold = false, Color? color = null, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            var rect = Rect(value, parent, x, y, x2, y2);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = bold ? BoldFont : mediumFont;
            text.text = value;
            text.fontSize = size;
            text.color = color ?? Ink;
            text.alignment = anchor;
            text.fontStyle = FontStyle.Normal;
            text.raycastTarget = false;
            text.lineSpacing = 1.12f;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            rect.gameObject.AddComponent<ReadableText>();
            return text;
        }

        public Button Clickable(RectTransform rect, Action action)
        {
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            var colors = button.colors;
            colors.highlightedColor = new Color(.96f, .93f, 1);
            colors.pressedColor = new Color(.78f, .68f, .95f);
            colors.disabledColor = new Color(.8f, .85f, .86f, .4f);
            colors.fadeDuration = .12f;
            button.colors = colors;
            if (button.targetGraphic.color.a > 0 && IsInsideGlass(rect))
            {
                var border = Rect("Button border", rect, 0, 0, 1, 1).gameObject.AddComponent<ButtonBorder>();
                border.Source = (Image)button.targetGraphic;
                border.Button = button;
                border.raycastTarget = false;
            }
            button.onClick.AddListener(() => action());
            return button;
        }

        bool IsInsideGlass(Transform element)
        {
            if (glassMaterial == null) return false;
            for (var parent = element.parent; parent != null; parent = parent.parent)
                if (parent.TryGetComponent<Image>(out var panel) && panel.material == glassMaterial)
                    return true;
            return false;
        }

        public Button TextButton(Transform parent, string text, float x, float y, float x2, float y2,
            Action action, bool primary = false)
        {
            var rect = Rect(text, parent, x, y, x2, y2);
            Fill(rect, primary ? Accent : new Color(1, 1, 1, .20f), primary);
            var button = Clickable(rect, action);
            Label(rect, text, .04f, .03f, .96f, .97f, 29, true, primary ? White : Ink, TextAnchor.MiddleCenter);
            return button;
        }

        public UIIcon Icon(Transform parent, string kind, float x, float y, float x2, float y2)
        {
            var icon = Rect(kind, parent, x, y, x2, y2).gameObject.AddComponent<UIIcon>();
            icon.Kind = kind;
            icon.color = Ink;
            icon.raycastTarget = false;
            return icon;
        }

        public Button IconButton(Transform parent, string kind, float x, float y, float x2, float y2,
            Action action, bool glass = true)
        {
            var rect = Rect(kind, parent, x, y, x2, y2);
            var image = Fill(rect, glass ? new Color(1, 1, 1, .30f) : Color.clear, glass);
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            Icon(rect, kind, .21f, .21f, .79f, .79f);
            return Clickable(rect, action);
        }

        public RectTransform Row(Transform parent, string icon, string text, float y, float y2, Action action)
        {
            var rect = Rect(text, parent, .045f, y, .955f, y2);
            Fill(rect, Color.clear);
            Clickable(rect, action);
            Icon(rect, icon, .015f, .17f, .12f, .83f);
            Label(rect, text, .15f, .1f, .70f, .9f, 31);
            return rect;
        }

        public Image Switch(Transform parent, out RectTransform knob)
        {
            var rect = Rect("Switch", parent, 1, .5f, 1, .5f);
            rect.pivot = new Vector2(1, .5f);
            rect.sizeDelta = new Vector2(144, 72);
            rect.anchoredPosition = new Vector2(-8, 0);
            var track = Fill(rect, Color.clear);
            track.pixelsPerUnitMultiplier = 1.75f;
            track.raycastTarget = false;
            knob = Rect("Thumb", rect, 0, .5f, 0, .5f);
            knob.sizeDelta = new Vector2(82, 60);
            var image = Fill(knob, Color.white);
            image.pixelsPerUnitMultiplier = 2.1f;
            image.raycastTarget = false;
            var shadow = knob.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, .15f);
            shadow.effectDistance = new Vector2(0, -3);
            var rim = knob.gameObject.AddComponent<Outline>();
            rim.effectColor = new Color(1, 1, 1, .7f);
            rim.effectDistance = Vector2.one;
            return track;
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(rounded);
            UnityEngine.Object.Destroy(roundedTexture);
        }
    }
}
