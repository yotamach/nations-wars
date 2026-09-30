using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NationsWars.Game
{
    /// <summary>
    /// One place for all player input. Works with the new Input System or the legacy Input Manager,
    /// whichever the project's Active Input Handling is set to.
    /// </summary>
    public static class RtsInput
    {
#if ENABLE_INPUT_SYSTEM
        public static Vector2 MousePosition { get { return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero; } }
        public static bool LeftDown { get { return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame; } }
        public static bool LeftHeld { get { return Mouse.current != null && Mouse.current.leftButton.isPressed; } }
        public static bool LeftUp { get { return Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame; } }
        public static bool RightDown { get { return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame; } }

        public static float Scroll
        {
            get
            {
                if (Mouse.current == null) return 0f;
                float y = Mouse.current.scroll.ReadValue().y;
                return Mathf.Abs(y) >= 10f ? y / 120f : y; // some platforms report notches of 120
            }
        }

        public static bool Shift { get { return Keyboard.current != null && Keyboard.current.shiftKey.isPressed; } }
        public static bool Ctrl { get { return Keyboard.current != null && Keyboard.current.ctrlKey.isPressed; } }

        public static Vector2 MoveAxis
        {
            get
            {
                var k = Keyboard.current;
                if (k == null) return Vector2.zero;
                float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1f : 0f) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1f : 0f);
                float y = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1f : 0f) - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1f : 0f);
                return new Vector2(x, y);
            }
        }

        /// <summary>True on the frame digit key 1..9 goes down.</summary>
        public static bool DigitDown(int digit)
        {
            if (Keyboard.current == null || digit < 1 || digit > 9) return false;
            return Keyboard.current[(Key)((int)Key.Digit1 + digit - 1)].wasPressedThisFrame;
        }
#else
        public static Vector2 MousePosition { get { return UnityEngine.Input.mousePosition; } }
        public static bool LeftDown { get { return UnityEngine.Input.GetMouseButtonDown(0); } }
        public static bool LeftHeld { get { return UnityEngine.Input.GetMouseButton(0); } }
        public static bool LeftUp { get { return UnityEngine.Input.GetMouseButtonUp(0); } }
        public static bool RightDown { get { return UnityEngine.Input.GetMouseButtonDown(1); } }
        public static float Scroll { get { return UnityEngine.Input.mouseScrollDelta.y; } }
        public static bool Shift { get { return UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift); } }
        public static bool Ctrl { get { return UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl); } }

        public static Vector2 MoveAxis
        {
            get
            {
                return new Vector2(
                    (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) -
                    (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f),
                    (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) -
                    (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow) ? 1f : 0f));
            }
        }

        public static bool DigitDown(int digit)
        {
            return digit >= 1 && digit <= 9 && UnityEngine.Input.GetKeyDown(KeyCode.Alpha0 + digit);
        }
#endif
    }
}
