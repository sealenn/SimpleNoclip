using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(SealeenDevMenuMod.SealeenMod), "Sealeen Menu", "1.0.0", "SealeenWorks")]
[assembly: MelonGame(null, null)]

namespace SealeenDevMenuMod
{
    public class SealeenMod : MelonMod
    {
        private float _noclipSpeed = 10f;
        private bool _showMenu = false;
        private bool _cursorLocked = true;
        private bool _flyEnabled = false;
        private string _speedInputBuffer = "10";

        private GameObject _localPlayer;
        private MonoBehaviour _fpsController;
        
        // для окна 
        private Rect _windowRect = new Rect(20, 20, 250, 150);

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _showMenu = !_showMenu;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                _cursorLocked = !_cursorLocked;
                SetCursorState(_cursorLocked);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                ToggleNoclip();
            }

            if (_flyEnabled)
            {
                MoveInNoclip();
            }
        }

        public override void OnGUI()
        {
            if (!_showMenu) return;

            // виндоу рект 
            _windowRect = GUILayout.Window(999, _windowRect, DrawConsoleWindow, "Sealeen Menu");
        }

        private void DrawConsoleWindow(int windowID)
        {
            GUILayout.Label("Noclip Speed:");
            _speedInputBuffer = GUILayout.TextField(_speedInputBuffer, 10);

            if (GUILayout.Button("Apply"))
            {
                if (float.TryParse(_speedInputBuffer, out float parsedSpeed))
                {
                    _noclipSpeed = parsedSpeed;
                    LoggerInstance.Msg($"local speed {_noclipSpeed}");
                }
                else
                {
                    LoggerInstance.Warning("invalid format");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label($"Noclip status: {(_flyEnabled ? "ENABLED" : "DISABLED")}");

            // драггабельность я хз правда кому она нужна
            GUI.DragWindow();
        }

        private void ToggleNoclip()
        {
            if (_localPlayer == null || _fpsController == null)
            {
                FindPlayer();
            }

            if (_localPlayer == null)
            {
                LoggerInstance.Warning("Player with FPScontroller not found!");
                return;
            }

            _flyEnabled = !_flyEnabled;

            if (_fpsController != null)
            {
                _fpsController.enabled = !_flyEnabled;
            }

            LoggerInstance.Msg($"Noclip status changed: {_flyEnabled}");
        }

        private void MoveInNoclip()
        {
            if (_localPlayer == null) return;

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Vector3 moveDirection = mainCam.transform.right * moveX + mainCam.transform.forward * moveZ;
            _localPlayer.transform.position += moveDirection * _noclipSpeed * Time.deltaTime;
        }

        private void SetCursorState(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void FindPlayer()
        {
            Object[] controllers = Object.FindObjectsOfType(typeof(MonoBehaviour));
            
            foreach (var current in controllers)
            {
                if (current.GetType().Name == "FPScontroller")
                {
                    _fpsController = (MonoBehaviour)current;
                    _localPlayer = _fpsController.gameObject;
                    LoggerInstance.Msg("true");
                    break;
                }
            }
        }
    }
}
