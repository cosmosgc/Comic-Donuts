using PhotoViewer.Scripts.Photo;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PhotoViewer.Scripts
{
    public class MouseWheelZoom : MonoBehaviour
    {
        [SerializeField] private AbstractView _view = null;
        [SerializeField] private float _speed = 25f;
        [SerializeField] private float _dumping = 4f;

        private float _delta;

        private void Start() => 
            _view.OnShow += Clear;

        private void Update()
        {
            
            var delta = -Mouse.current.scroll.ReadValue() * Time.deltaTime * _speed;
            _delta = Mathf.Lerp(_delta, delta.y, Time.deltaTime * _dumping);
            SetZoomSlider(_delta);
        }

        private void SetZoomSlider(float value)
        {
            if (_view.GetType() == typeof(PhotoView))
                _view.ZoomSlider.value -= value;
            else
                _view.ZoomSlider.value += value;
        }

        private void Clear() =>
            _delta = 0;

        private void OnDestroy() =>
            _view.OnShow -= Clear;
    }
}