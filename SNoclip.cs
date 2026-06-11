using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime;

[assembly: MelonInfo(typeof(SimpleNoclipMod.SimpleNoclip), "Sealeen's Menu", "1.0.0", "SealWorks")]
[assembly: MelonGame(null, null)]

namespace SimpleNoclipMod
{
    public class SimpleNoclip : MelonMod
    {
        private float _noclipSpeed = 10f;
        private bool _showMenu = false;
        private bool _cursorLocked = true;
        private bool _flyEnabled = false;
        private string _speedInputBuffer = "10";

        private GameObject _localPlayer;
        private MonoBehaviour _fpsController;
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

            // фикс
            _windowRect = GUILayout.Window(999, _windowRect, (GUI.WindowFunction)DrawConsoleWindow, "Sealeen Menu");
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
                    LoggerInstance.Msg($"local speed: {_noclipSpeed}");
                }
                else
                {
                    LoggerInstance.Warning("not valid");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label($"Noclip status: {(_flyEnabled ? "ENABLED" : "DISABLED")}");

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

            LoggerInstance.Msg($"status? : {_flyEnabled}");
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
            // фикс
            var monoBehaviourType = Il2CppType.Of<MonoBehaviour>();
            var objects = UnityEngine.Object.FindObjectsOfType(monoBehaviourType);
            
            foreach (var obj in objects)
            {
                var comp = obj.TryCast<MonoBehaviour>();
                if (comp != null && comp.GetIl2CppType().Name == "FPScontroller")
                {
                    _fpsController = comp;
                    _localPlayer = comp.gameObject;
                    LoggerInstance.Msg("true");
                    break;
                }
            }
        }
    }
}
