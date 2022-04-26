using UnityEngine;
using UnityEngine.UI;

namespace PhotoViewer.Scripts.Photo
{
    public class PhotoView : AbstractView
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private Sprite _defaultImage = null;

        [SerializeField] private PhotoMap _photoMap = null;

        private RectTransform _transform;

        private RectTransform _imageTransform;

        private Vector2? _defaultImageSize;

        private Vector2 ViewerSize
        {
            get
            {
                var rect = _transform.rect;
                return new Vector2(rect.width, rect.height);
            }
        }

        private Vector2 ImageSize
        {
            get
            {
                var rect = _imageTransform.rect;
                var angle = (int) _imageTransform.rotation.eulerAngles.z;
                Vector2 result;

                if (angle == 0 || angle == 180)
                    result = new Vector2(rect.width, rect.height);
                else
                    result = new Vector2(rect.height, rect.width);

                return result;
            }
        }

        protected override void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _imageTransform = _image.GetComponent<RectTransform>();

            base.Awake();
        }

        protected override void ShowMap(bool show) =>
            _photoMap.Show(show);

        protected override void ShowData(ImageData imageData)
        {
            Clear();

            _zoomSlider.onValueChanged.AddListener(Zoom);
            _btnReset.Show(false);
            
            _image.sprite = imageData.Sprite;

            RescalePhoto(imageData.Sprite);
            _photoMap.SetSize(ImageSize, ViewerSize);
        }

        protected override void Zoom(float value)
        {
            if (_defaultImageSize == null)
                return;

            OnChange?.Invoke();

            _imageTransform.sizeDelta = (Vector2) (_defaultImageSize + _defaultImageSize * value * 10);
            _photoMap.SetSize(ImageSize, ViewerSize);
        }

        public void RotateLeft()
        {
            var euler = _imageTransform.rotation.eulerAngles;
            _imageTransform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z + 90);
            _photoMap.SetSize(ImageSize, ViewerSize);

            OnChange?.Invoke();
        }

        public void RotateRight()
        {
            var euler = _imageTransform.rotation.eulerAngles;
            _imageTransform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z - 90);
            _photoMap.SetSize(ImageSize, ViewerSize);

            OnChange?.Invoke();
        }

        public override void ApplyInput(Vector2 deltaPosition)
        {
            Vector2 newPosition = _imageTransform.localPosition;

            if (ImageSize.x > ViewerSize.x)
            {
                newPosition.x += deltaPosition.x;

                if ((newPosition.x - ImageSize.x / 2) > -ViewerSize.x / 2)
                    newPosition.x = -ViewerSize.x / 2 + ImageSize.x / 2;

                if ((newPosition.x + ImageSize.x / 2) < ViewerSize.x / 2)
                    newPosition.x = ViewerSize.x / 2 - ImageSize.x / 2;
            }
            else
                newPosition.x = 0;

            if (ImageSize.y > ViewerSize.y)
            {
                newPosition.y += deltaPosition.y;

                if ((newPosition.y - ImageSize.y / 2) > -ViewerSize.y / 2)
                    newPosition.y = -ViewerSize.y / 2 + ImageSize.y / 2;

                if ((newPosition.y + ImageSize.y / 2) < ViewerSize.y / 2)
                    newPosition.y = ViewerSize.y / 2 - ImageSize.y / 2;
            }
            else
                newPosition.y = 0;

            _imageTransform.localPosition = (Vector3) newPosition;

            if( float.IsNaN(ImageSize.x) || float.IsNaN(ViewerSize.x))
            {
                _photoMap.SetPosition(newPosition, ImageSize, ViewerSize);
            }

            if (deltaPosition != Vector2.zero)
                OnChange?.Invoke();
        }

        public void Clear()
        {
            var euler = _imageTransform.rotation.eulerAngles;
            _imageTransform.rotation = Quaternion.Euler(euler.x, euler.y, 0);
            _image.sprite = _defaultImage;
            _photoMap.Clear();

            ResetZoom();
        }

        private void RescalePhoto(Sprite sprite)
        {
            var viewerSize = ViewerSize;
            var spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);

            var viewerAspect = viewerSize.x / viewerSize.y;
            var spriteAspect = spriteSize.x / spriteSize.y;

            if (spriteAspect > viewerAspect)
            {
                var relation = viewerSize.x / sprite.texture.width;
                _imageTransform.sizeDelta = new Vector2(viewerSize.x, relation * spriteSize.y);
            }
            else
            {
                var relate = viewerSize.y / sprite.texture.height;
                _imageTransform.sizeDelta = new Vector2(relate * spriteSize.x, viewerSize.y);
            }

            _defaultImageSize = _imageTransform.sizeDelta;
        }
    }
}