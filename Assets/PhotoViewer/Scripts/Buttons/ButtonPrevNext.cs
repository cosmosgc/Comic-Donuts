using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotoViewer.Scripts.Buttons
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Routines))]
    public class ButtonPrevNext : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent OnClick = null;

        [SerializeField] private float _defaultAlpha = 0;
        [SerializeField] private float _hoverAlpha = 0.1f;
        [SerializeField] private float _clickAlpha = 0.2f;

        [SerializeField] private float _hoverTime = 0.3f;
        [SerializeField] private float _clickTime = 0.1f;

        private Routines _routines;
        private Image _image;
        private Color _color;

        private void Awake()
        {
            _routines = GetComponent<Routines>();
            _image = GetComponent<Image>();
            _color = _image.color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _routines.LerpFloat(_image.color.a, _clickAlpha, _clickTime, SetAlpha,
                () => { _routines.LerpFloat(_image.color.a, _hoverAlpha, _clickTime, SetAlpha); });

            OnClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _routines.LerpFloat(_image.color.a, _hoverAlpha, _hoverTime, SetAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _routines.LerpFloat(_image.color.a, _defaultAlpha, _hoverTime, SetAlpha);
        }

        private void SetAlpha(float x)
        {
            _color.a = x;
            _image.color = _color;
        }
    }
}