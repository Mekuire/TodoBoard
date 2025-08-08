using System;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    public class ColourPickerController : Panel
    {
        [SerializeField] private RawImage _h;
        [SerializeField] private RawImage _sv;
        [SerializeField] private RawImage _out;
        [SerializeField] private Slider _hueSlider;
        [SerializeField] private SVPickControl _picker;
        [SerializeField] private Button _doneButton;
        [SerializeField] private float _minV = 0.2f;
        
        private Texture2D _hTexture, _svTexture, _outTexture;
        private float _currentH, _currentS, _currentV;

        public event Action<float, float, float> OnColorChanged;
        public event Action<HSVColor> OnColorSelectionDone;
        
        public void Initialize()
        {
            CreateHTexture();
            CreateSVTexture();
            CreateOutTexture();
            _picker.Initialize();
        }
        
        private void OnEnable()
        {
            _hueSlider.onValueChanged.AddListener(OnHueChanged);
            _doneButton.onClick.AddListener(OnDone);
            _picker.OnValueChanged += OnSVChanged;
        }

        private void OnDisable()
        {
            _hueSlider.onValueChanged.RemoveListener(OnHueChanged);
            _doneButton.onClick.RemoveListener(OnDone);
            _picker.OnValueChanged -= OnSVChanged;
        }

        public void SetColour(HSVColor color)
        {
            _picker.SetPickerPositionByColor(color);
            _currentH = color.h;
            _currentS = color.s;
            _currentV = color.v;
            _hueSlider.value = _currentH;
            UpdateSVTexture();
            UpdateOutTexture();
        }
        
        private void OnDone()
        {
            OnColorSelectionDone?.Invoke(new HSVColor(_currentH, _currentS, _currentV));
        }

        private void OnSVChanged(float s, float v)
        {
            v = Mathf.Lerp(_minV, 1f, v);
            _currentS = s;
            _currentV = v;
            UpdateOutTexture();
            OnColorChanged?.Invoke(_currentH, _currentS, _currentV);
        }

        private void OnHueChanged(float value)
        {
            _currentH = value;
            UpdateSVTexture();
            UpdateOutTexture();
            OnColorChanged?.Invoke(_currentH, _currentS, _currentV);
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
                float v = Mathf.Lerp(_minV, 1f, (float)i / (_svTexture.height - 1));

                for (int j = 0; j < _svTexture.width; j++)
                {
                    float s = (float)j / (_svTexture.width - 1);
                    _svTexture.SetPixel(j, i, Color.HSVToRGB(_currentH, s, v));
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
