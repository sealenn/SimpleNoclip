using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(SealeenMenu.SealeenMenuMod), "boolclip", "1.0.0", "Sealeen")]
[assembly: MelonGame]

namespace SealeenMenu
{
    public class SealeenMenuMod : MelonMod
    {
        private bool _menuOpen = false;
        private bool _cursorUnlocked = false;

        private bool _noclipEnabled = false;
        private float _noclipSpeed = 10f;
        private string _speedInput = "10";

        private GameObject _localPlayer = null;
        private CharacterController _charController = null;

        private Rect _windowRect = new Rect(50, 50, 300, 160);
        private GUIStyle _windowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _textFieldStyle;
        private bool _stylesInitialized = false;

        public override void OnUpdate()
        {
        // change these keybinds for your needs
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _menuOpen = !_menuOpen;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                _cursorUnlocked = !_cursorUnlocked;
                if (_cursorUnlocked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                _noclipEnabled = !_noclipEnabled;

                if (_localPlayer == null)
                {
                    _localPlayer = GameObject.FindWithTag("Player");
                    if (_localPlayer != null)
                    // yo add ur own player here im just inserted my sample
                        _charController = _localPlayer.GetComponent<CharacterController>();
                }

                if (_charController != null)
                {
                    // controller change if you want ofc
                    _charController.enabled = !_noclipEnabled;
                }

                MelonLogger.Msg(_noclipEnabled ? "boolclip : Noclip ON" : "boolclip : Noclip OFF");
            }

            if (_noclipEnabled && _localPlayer != null)
            {
                if (_charController != null && _charController.enabled)
                    _charController.enabled = false;

                // camera getter change too if needed 
                Camera cam = Camera.main;
                Transform moveRef = (cam != null) ? cam.transform : _localPlayer.transform;

                Vector3 moveDir = Vector3.zero;

                if (Input.GetKey(KeyCode.W)) moveDir += moveRef.forward;
                if (Input.GetKey(KeyCode.S)) moveDir -= moveRef.forward;
                if (Input.GetKey(KeyCode.A)) moveDir -= moveRef.right;
                if (Input.GetKey(KeyCode.D)) moveDir += moveRef.right;
                if (Input.GetKey(KeyCode.Space)) moveDir += Vector3.up;
                if (Input.GetKey(KeyCode.LeftControl)) moveDir -= Vector3.up;

                _localPlayer.transform.position += moveDir.normalized * _noclipSpeed * Time.deltaTime;
            }
        }

// simple gui for ya
        private void InitStyles()
        {
            if (_stylesInitialized) return;
            _stylesInitialized = true;

            Texture2D windowBg = MakeTexture(2, 2, new Color(0.08f, 0.08f, 0.08f, 0.92f));
            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = windowBg;
            _windowStyle.onNormal.background = windowBg;
            _windowStyle.normal.textColor = new Color(0.4f, 0.85f, 1f);
            _windowStyle.fontStyle = FontStyle.Bold;
            _windowStyle.fontSize = 14;
            _windowStyle.alignment = TextAnchor.UpperCenter;

            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontSize = 12;

            Texture2D btnBg = MakeTexture(2, 2, new Color(0.2f, 0.55f, 0.9f, 1f));
            Texture2D btnHover = MakeTexture(2, 2, new Color(0.3f, 0.65f, 1f, 1f));
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.normal.background = btnBg;
            _buttonStyle.hover.background = btnHover;
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.fontSize = 12;

            Texture2D fieldBg = MakeTexture(2, 2, new Color(0.18f, 0.18f, 0.18f, 1f));
            _textFieldStyle = new GUIStyle(GUI.skin.textField);
            _textFieldStyle.normal.background = fieldBg;
            _textFieldStyle.normal.textColor = Color.white;
            _textFieldStyle.fontSize = 12;
        }

        public override void OnGUI()
        {
            if (!_menuOpen) return;

            InitStyles();

            _windowRect = GUI.Window(9001, _windowRect, DrawWindow, "boolclip settings", _windowStyle);
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 20));

            GUILayout.Space(6);

            string noclipStatus = _noclipEnabled
                ? "<color=#44ff88>● Status: ON</color>"
                : "<color=#ff5555>● Status: OFF</color>";

            GUIStyle richLabel = new GUIStyle(_labelStyle);
            richLabel.richText = true;
            GUILayout.Label(noclipStatus, richLabel);

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.Label("boolclip Speed:", _labelStyle, GUILayout.Width(100));
            _speedInput = GUILayout.TextField(_speedInput, _textFieldStyle, GUILayout.Width(80));

            if (GUILayout.Button("Apply", _buttonStyle, GUILayout.Width(65)))
            {
                if (float.TryParse(_speedInput, out float parsed))
                {
                    _noclipSpeed = parsed;
                    MelonLogger.Msg($"Boolclip : Noclip speed set to {_noclipSpeed}");
                }
                else
                {
                    MelonLogger.Warning("Boolclip : Invalid speed value");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            GUIStyle hintStyle = new GUIStyle(_labelStyle);
            hintStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            hintStyle.fontSize = 10;
            GUILayout.Label("v is noclip", hintStyle);
        }

        private Texture2D MakeTexture(int w, int h, Color col)
        {
            Color[] pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = col;
            Texture2D tex = new Texture2D(w, h);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
