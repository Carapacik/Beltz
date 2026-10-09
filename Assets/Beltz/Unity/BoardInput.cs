using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Beltz.Unity
{
    // Converts a short pointer gesture outside UI into a board-space tap.
    internal sealed class BoardInput
    {
        readonly Camera camera;
        readonly Action unlockAudio;
        readonly Action<Vector2> tap;
        readonly List<RaycastResult> hits = new List<RaycastResult>();
        readonly PointerEventData pointer;
        Vector2 pressPosition;
        bool pressing;

        public BoardInput(Camera camera, Action unlockAudio, Action<Vector2> tap)
        {
            this.camera = camera;
            this.unlockAudio = unlockAudio;
            this.tap = tap;
            pointer = new PointerEventData(EventSystem.current);
        }

        public void Cancel() => pressing = false;

        public void Tick(bool allowTap)
        {
            ReadPointer(out bool down, out bool up, out Vector2 position);
            if (down)
            {
                unlockAudio();
                pressing = allowTap && !OverUI(position);
                pressPosition = position;
            }
            if (!up || !pressing) return;
            pressing = false;
            float dragThreshold = Mathf.Max(20, Screen.height * .018f);
            if (allowTap && !OverUI(position) && Vector2.Distance(position, pressPosition) < dragThreshold)
                tap(camera.ScreenToWorldPoint(position));
        }

        bool OverUI(Vector2 position)
        {
            var events = EventSystem.current;
            if (events == null) return false;
            pointer.position = position;
            hits.Clear();
            events.RaycastAll(pointer, hits);
            return hits.Count > 0;
        }

        static void ReadPointer(out bool down, out bool up, out Vector2 position)
        {
            down = up = false;
            position = default;
            var touch = Touchscreen.current?.primaryTouch;
            if (touch != null && (touch.press.isPressed || touch.press.wasReleasedThisFrame))
            {
                down = touch.press.wasPressedThisFrame;
                up = touch.press.wasReleasedThisFrame;
                position = touch.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                down = Mouse.current.leftButton.wasPressedThisFrame;
                up = Mouse.current.leftButton.wasReleasedThisFrame;
                position = Mouse.current.position.ReadValue();
            }
        }
    }
}
