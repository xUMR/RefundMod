using UnityEngine;

namespace RefundMod
{
    public class RefundModUI : MonoBehaviour
    {
        private const uint Size = 22;
        private const uint Width = 320;

        private RefundModData _data;

        private Rect _window = _windowShownRect;

        private static Rect _windowShrunkRect = new Rect(60, 20, Size * 6, Size * 2);
        private static Rect _windowShownRect = new Rect(60, 20, Width, Size * 6);

        private static Rect _windowHorizontalRect = new Rect(_windowShownRect.x, _windowShownRect.y,
                                                             _windowShownRect.width, _windowShrunkRect.height);

        private readonly Rect _hideButtonRect = new Rect(0, 0, Size, Size);

        private readonly Rect _labelBulldozeRect = new Rect(Size / 2, Size * 2, Width / 2, Size);
        private readonly Rect _labelRelocateRect = new Rect(Width / 2, Size * 2, Width / 2, Size);

        private readonly Rect _sliderBulldozeRect = new Rect(Size / 2, Size * 3, Width / 2 - Size, Size);
        private readonly Rect _sliderRelocateRect = new Rect(Width / 2, Size * 3, Width / 2 - Size, Size);

        private readonly Rect _togglePauseRect = new Rect(Size / 2, Size * 4, Size * 6, Size);
        private readonly Rect _labelTimeLimitRect = new Rect(Width / 2, Size * 4, Size * 6, Size);

        private readonly string[] _shrinkButtonTextOptions = new[] { "ʌ", "v" };

        private string _shrinkButtonText;

        private string BulldozeText
        {
            get
            {
                return _data.RefundModifier < 0
                           ? "Bulldoze Cost: " + _data.RefundModifier * -100 + "%"
                           : "Bulldoze Refund: " + _data.RefundModifier * 100 + "%";
            }
        }

        private string RelocateText { get { return "Relocation Cost: " + _data.RelocateModifier * 100 + "%"; } }

        private bool _clicked;
        private bool _visible;
        private bool _ready;

        private float _t0;

        private int _state;
        private const int AnimSpeed = 4;

        private GUIStyle _windowStyle;
        private CameraController _cameraController;

        void Update()
        {
            _visible = !_cameraController.m_freeCamera;
        }

        void OnGUI()
        {
            if (!_visible) return;

            if (_windowStyle == null)
            {
                if (!_ready)
                    Init(new RefundModData());

                _windowStyle = GUI.skin.window;
                _windowStyle.alignment = TextAnchor.UpperRight;
            }

            _window = GUI.Window(185586073, _window, WindowFunction, "Refund Panel", _windowStyle);
            _data.X = _window.x;
            _data.Y = _window.y;
        }

        void WindowFunction(int id)
        {
            Hide(ref _clicked);

            if (GUI.Button(_hideButtonRect, _shrinkButtonText))
            {
                _data.Shrunk = !_data.Shrunk;
                _clicked = true;
                _t0 = Time.time;
                _state = 0;

                _windowShownRect.x = _window.x;
                _windowShownRect.y = _window.y;
                _windowHorizontalRect.x = _window.x;
                _windowHorizontalRect.y = _window.y;
                _windowShrunkRect.x = _window.x;
                _windowShrunkRect.y = _window.y;
            }

            _data.OnlyWhenPaused = GUI.Toggle(_togglePauseRect, _data.OnlyWhenPaused, "Only When Paused");

            if (_data.RemoveTimeLimit)
                GUI.Label(_labelTimeLimitRect, "No Time Limit");

            GUI.Label(_labelBulldozeRect, BulldozeText);
            GUI.Label(_labelRelocateRect, RelocateText);

            _data.RefundModifier = GUI.HorizontalSlider(_sliderBulldozeRect, _data.RefundModifier, -1, 1);
            _data.RelocateModifier = GUI.HorizontalSlider(_sliderRelocateRect, _data.RelocateModifier, 0, 1);

            GUI.DragWindow();
        }

        void Hide(ref bool click)
        {
            if (!click) return;

            if (_data.Shrunk)
            {
                _shrinkButtonText = "v";

                if (_window == _windowHorizontalRect) _state = 1;
                else if (_window == _windowShrunkRect) _state = 2;

                if (_state == 0)
                    _window = Animate(_clicked, _windowShownRect, _windowHorizontalRect, _t0);
                else if (_state == 1)
                    _window = Animate(_clicked, _windowHorizontalRect, _windowShrunkRect, _t0 + 1.0f / AnimSpeed);
                else if (_state == 2)
                    click = false;
            }
            else
            {
                _shrinkButtonText = "ʌ";

                if (_window == _windowHorizontalRect) _state = 1;
                else if (_window == _windowShownRect) _state = 2;

                if (_state == 0)
                    _window = Animate(_clicked, _windowShrunkRect, _windowHorizontalRect, _t0);
                else if (_state == 1)
                    _window = Animate(_clicked, _windowHorizontalRect, _windowShownRect, _t0 + 1.0f / AnimSpeed);
                else if (_state == 2)
                    click = false;
            }
        }

        static Rect Animate(bool input, Rect start, Rect end, float startTime)
        {
            if (!input) return start;

            var time = Time.time;
            var t = (time - startTime) * AnimSpeed;

            return new Rect(start.x, start.y, Mathf.Lerp(start.width, end.width, t),
                            Mathf.Lerp(start.height, end.height, t));
        }

        public void Init(RefundModData data)
        {
            _data = data;

            _windowShrunkRect = new Rect(_data.X, _data.Y, Size * 6, Size * 2);
            _windowShownRect = new Rect(_data.X, _data.Y, Width, Size * 6);

            _shrinkButtonText = _data.Shrunk ? _shrinkButtonTextOptions[1] : _shrinkButtonTextOptions[0];
            _window = _data.Shrunk ? _windowShrunkRect : _windowShownRect;
            _windowHorizontalRect = new Rect(_windowShownRect.x, _windowShownRect.y, _windowShownRect.width,
                                             _windowShrunkRect.height);

            var camera = GameObject.FindGameObjectWithTag("MainCamera");
            if (camera != null)
            {
                _cameraController = camera.GetComponent<CameraController>();
            }

            _ready = true;
        }
    }
}
