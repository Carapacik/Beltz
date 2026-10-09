using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beltz.Unity
{
    // Supplies continuous rectangle coordinates even when the sprite is nine-sliced.
    [RequireComponent(typeof(Image))]
    public sealed class GlassSurface : BaseMeshEffect, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public override void ModifyMesh(VertexHelper mesh)
        {
            if (!IsActive()) return;
            var image = (Image)graphic;
            var rect = image.rectTransform.rect;
            float radius = Mathf.Min(rect.width, rect.height) * .5f;
            if (image.type == Image.Type.Simple && image.preserveAspect)
                rect = new Rect(rect.center - Vector2.one * radius, Vector2.one * radius * 2);
            else if (image.type == Image.Type.Sliced)
                radius = Mathf.Min(radius, 63 / (image.pixelsPerUnit * image.pixelsPerUnitMultiplier));
            UIVertex vertex = default;
            for (int i = 0; i < mesh.currentVertCount; i++)
            {
                mesh.PopulateUIVertex(ref vertex, i);
                vertex.uv1 = new Vector4(vertex.position.x - rect.center.x, vertex.position.y - rect.center.y, rect.width * .5f, rect.height * .5f);
                vertex.uv2 = new Vector4(radius, 0, 0, 0);
                mesh.SetUIVertex(vertex, i);
            }
        }

        bool pressed;
        Button button;
        protected override void Start() { base.Start(); button = GetComponent<Button>(); }
        void Update()
        {
            if (button == null) return;
            float target = pressed && button.IsInteractable() ? .965f : 1;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * target, 1 - Mathf.Exp(-22 * Time.unscaledDeltaTime));
        }
        public void OnPointerDown(PointerEventData eventData) { if (eventData.button == PointerEventData.InputButton.Left) pressed = true; }
        public void OnPointerUp(PointerEventData eventData) { pressed = false; }
        public void OnPointerExit(PointerEventData eventData) { pressed = false; }
        protected override void OnDisable()
        {
            pressed = false;
            if (button != null) transform.localScale = Vector3.one;
            base.OnDisable();
        }
    }
}
