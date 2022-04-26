using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotoViewer.Scripts
{
    public abstract class AbstractInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
#if UNITY_IOS || UNITY_ANDROID
        private float _speed = 5;
#else
        private float _speed = 25;
#endif
        
        [SerializeField] protected AbstractView _view = default;

        [SerializeField] protected float _dumping = 3f;

        protected Vector2 _deltaPosition = Vector2.zero;

        protected void Start() => 
            _view.OnShow += Clear;

        protected abstract void OnUpdate();

        private void Update()
        {
            _deltaPosition = Vector2.Lerp(_deltaPosition, Vector2.zero, Time.deltaTime * _dumping);
            
            if (_deltaPosition.magnitude < 0.1f) 
                _deltaPosition = Vector2.zero;

            OnUpdate();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData) => 
            _deltaPosition = eventData.delta * Time.deltaTime * _speed;

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        private void Clear() =>
            _deltaPosition = Vector2.zero;

        protected void OnDestroy() => 
            _view.OnShow += Clear;
    }
}