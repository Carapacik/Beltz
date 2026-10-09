using UnityEngine;
using UnityEngine.UI;

namespace Beltz.Unity
{
    [RequireComponent(typeof(Text))]
    public sealed class ReadableText : MonoBehaviour
    {
        Text label;
        int preferredSize;
        Vector2 lastScreen;
        Matrix4x4 lastTransform, lastProjection, lastView;
        bool dirty = true;

        void Start()
        {
            label = GetComponent<Text>();
            preferredSize = label.fontSize;
        }

        void OnEnable() => dirty = true;
        void OnRectTransformDimensionsChange() => dirty = true;

        void LateUpdate()
        {
            if (label == null || label.canvas == null) return;
            var canvas = label.canvas;
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            var screen = new Vector2(Screen.width, Screen.height);
            var transformMatrix = label.rectTransform.localToWorldMatrix;
            var projection = camera != null ? camera.projectionMatrix : Matrix4x4.identity;
            var view = camera != null ? camera.worldToCameraMatrix : Matrix4x4.identity;
            if (!dirty && screen == lastScreen && transformMatrix == lastTransform &&
                projection == lastProjection && view == lastView) return;

            lastScreen = screen;
            lastTransform = transformMatrix;
            lastProjection = projection;
            lastView = view;
            dirty = false;
            // Font files carry the actual 500/700 weight; avoid synthetic bold.
            int size = Mathf.Max(preferredSize, Mathf.CeilToInt(14 / ScreenScale(label)));
            if (label.fontSize != size) label.fontSize = size;
        }

        public static float ScreenScale(Text text)
        {
            var canvas = text.canvas;
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            var origin = text.rectTransform.TransformPoint(Vector3.zero);
            var top = text.rectTransform.TransformPoint(Vector3.up);
            return Mathf.Max(.01f, Vector2.Distance(RectTransformUtility.WorldToScreenPoint(camera, origin),
                RectTransformUtility.WorldToScreenPoint(camera, top)));
        }
    }
}
