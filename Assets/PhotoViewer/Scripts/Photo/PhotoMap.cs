using UnityEngine;

namespace PhotoViewer.Scripts.Photo
{
    public class PhotoMap : ViewerMap
    {
        [SerializeField] private RectTransform _map = default;
        [SerializeField] private RectTransform _picture = default;
        private RectTransform _transform;

        private Vector2 _photoMapSize
        {
            get
            {
                Rect rect = _transform.rect;
                return new Vector2(rect.width, rect.height);
            }
        }

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            var a = _transform.rect;
        }

        public void Clear()
        {
        }

        public void SetPosition(Vector2 position, Vector2 imgSize, Vector2 viewSize)
        {
            var newPosition = Vector2.zero;
            var imgBigSize = (imgSize.x > imgSize.y) ? imgSize.x : imgSize.y;
            newPosition.x = -position.x * (_photoMapSize.x / imgBigSize) + _photoMapSize.x / 2;
            newPosition.y = -position.y * (_photoMapSize.y / imgBigSize) + _photoMapSize.y / 2;

            _map.localPosition = newPosition;
        }

        public void SetSize(Vector2 imageSize, Vector2 viewSize)
        {
            var imageMagn = imageSize.magnitude;
            var viewMagn = viewSize.magnitude;

            var bigSize = (imageMagn > viewMagn) ? imageSize : viewSize;
            var smallSize = (imageMagn > viewMagn) ? viewSize : imageSize;

            var bigRect = (imageMagn > viewMagn) ? _picture : _map;

            var relation = bigSize.x / bigSize.y;

            if (bigSize.x > bigSize.y)
                bigRect.sizeDelta = new Vector2(_photoMapSize.x, _photoMapSize.y / relation);
            else
                bigRect.sizeDelta = new Vector2(_photoMapSize.x * relation, _photoMapSize.y);

            InnerPart(bigSize, smallSize, imageMagn > viewMagn);
        }

        private void InnerPart(Vector2 bigSize, Vector2 smallSize, bool imageBigger)
        {
            var relation = Vector2.zero;
            relation.x = smallSize.x / bigSize.x;
            relation.y = smallSize.y / bigSize.y;

            if (imageBigger)
                _map.sizeDelta = new Vector2(_picture.sizeDelta.x * relation.x, _picture.sizeDelta.y * relation.y);
            else
                _picture.sizeDelta = new Vector2(_map.sizeDelta.x * relation.x, _map.sizeDelta.y * relation.y);
        }
    }
}