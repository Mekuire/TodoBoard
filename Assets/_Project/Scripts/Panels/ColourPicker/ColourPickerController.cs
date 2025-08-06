using System;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    public class ColourPickerController : MonoBehaviour
    {
        [SerializeField] private RawImage _h;
        [SerializeField] private RawImage _sv;
        [SerializeField] private RawImage _out;
        [SerializeField] private Slider _hueSlider;
        [SerializeField] private SVPickControl _picker;
        [SerializeField] private Button _doneButton;
        
        private Texture2D _hTexture, _svTexture, _outTexture;
        private float _currentH, _currentS, _currentV;

        public event Action<Color> OnColorSelectionDone;
        
        private void Awake()
        {
            CreateHTexture();
            CreateSVTexture();
            CreateOutTexture();
        }

        private void Start()
        {
            SetColour(Color.white);
        }

        private void OnEnable()
        {
            _hueSlider.onValueChanged.AddListener(OnHueChanged);
            _doneButton.onClick.AddListener(OnDone);
            _picker.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _hueSlider.onValueChanged.RemoveListener(OnHueChanged);
            _doneButton.onClick.RemoveListener(OnDone);
            _picker.OnValueChanged -= OnValueChanged;
        }

        public void SetColour(Color color)
        {
            _picker.SetPickerPositionByColor(color);
            Color.RGBToHSV(color, out _currentH, out _currentS, out _currentV);
            UpdateSVTexture();
            UpdateOutTexture();
        }
        
        private void OnDone()
        {
            OnColorSelectionDone?.Invoke(Color.HSVToRGB(_currentH, _currentS, _currentV));
        }

        private void OnValueChanged(float arg1, float arg2)
        {
            _currentS = arg1;
            _currentV = arg2;
            UpdateOutTexture();
        }

        private void OnHueChanged(float value)
        {
            _currentH = value;
            UpdateSVTexture();
            UpdateOutTexture();
        }
        
        private void CreateHTexture()
        {
            _hTexture = new Texture2D(1, 16);
            _hTexture.wrapMode = TextureWrapMode.Clamp;
            _hTexture.name = "HTexture";

            for (int y = 0; y < _hTexture.height; y++)
            {
                _hTexture.SetPixel(1, y, Color.HSVToRGB( (float)y / _hTexture.height, 1f, 1f ));
            }
            
            _hTexture.Apply();
            _h.texture = _hTexture;
        }

        private void CreateSVTexture()
        {
            _svTexture = new Texture2D(16, 16);
            _svTexture.wrapMode = TextureWrapMode.Clamp;
            _svTexture.name = "SVTexture";
            
            _sv.texture = _svTexture;
        }

        private void CreateOutTexture()
        {
            _outTexture = new Texture2D(1, 1);
            _outTexture.wrapMode = TextureWrapMode.Clamp;
            _outTexture.name = "OutTexture";
            
            _out.texture = _outTexture;
        }

        private void UpdateSVTexture()
        {
            for (int i = 0; i < _svTexture.height; i++)
            {
                for (int j = 0; j < _svTexture.width; j++)
                {
                    _svTexture.SetPixel(j, i, Color.HSVToRGB(_currentH, (float)j / _svTexture.width, (float)i / _svTexture.height));
                }
            }
            
            _svTexture.Apply();
        }

        private void UpdateOutTexture()
        {
            _outTexture.SetPixel(1, 1, Color.HSVToRGB(_currentH, _currentS, _currentV));
            _outTexture.Apply();
        }
    }
}
