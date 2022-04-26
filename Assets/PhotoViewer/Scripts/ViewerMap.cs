using UnityEngine;

namespace PhotoViewer.Scripts
{
    [RequireComponent(typeof(Routines))]
    public abstract class ViewerMap : MonoBehaviour
    {
        [SerializeField] private Routines _routines = default;
        [SerializeField] private CanvasGroup _canvasGroup = default;
        [SerializeField] private float _fadeTime = 0.5f;
        [SerializeField] private float _showAlpha = 1f;
        [SerializeField] private float _hideAlpha = 0f;
        [SerializeField] private float _showTime = 3f;

        private bool _currentShow = true;

        public void Show(bool show)
        {
            if (show == _currentShow)
                return;

            _routines.StopAllRoutines();

            _currentShow = show;

            if (show)
            {
                _routines.LerpFloat(_canvasGroup.alpha, _showAlpha, _fadeTime, (a) => { _canvasGroup.alpha = a; });
                _routines.LerpFloat(0, 0, _fadeTime + _showTime, null, () => Show(false));
            }
            else
                _routines.LerpFloat(_canvasGroup.alpha, _hideAlpha, _fadeTime, (a) => { _canvasGroup.alpha = a; });
        }
    }
}