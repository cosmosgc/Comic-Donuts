using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PhotoViewer.Scripts.Gallery
{
    public class GalleryView : MonoBehaviour
    {
        [SerializeField] private Text _name = default;
        [SerializeField] private RectTransform _content = default;
        [SerializeField] private GalleryTile _galleryTilePrefab = default;

        public event Action<int> OnImageSelect;

        private Coroutine _applySizeRoutine;

        public void Clear()
        {
            if (_applySizeRoutine != null)
            {
                StopCoroutine(_applySizeRoutine);
                _applySizeRoutine = null;
            }

            foreach (RectTransform child in _content)
                Destroy(child.gameObject);
        }

        public void Init(List<ImageData> imageDatas)
        {
            _name.text = Globals.GalleryViewHeader;

            for (var i = 0; i < imageDatas.Count; i++)
            {
                var go = Instantiate(_galleryTilePrefab, Vector3.zero, Quaternion.identity, _content);
                var taleT = go.GetComponent<RectTransform>();

                go.Image.sprite = imageDatas[i].Sprite;
                var imageT = go.Image.GetComponent<RectTransform>();
                var relative = imageDatas[i].Sprite.rect.width / imageDatas[i].Sprite.rect.height;

                go.Number = i;
                go.OnClick += TileClick;

                if (go.Name)
                    go.Name.text = imageDatas[i].Name;

                if (go.Date)
                    go.Date.text = imageDatas[i].Date;

                _applySizeRoutine = StartCoroutine(ApplySize(imageT, taleT, relative));
            }
        }

        private void TileClick(int number) =>
            OnImageSelect?.Invoke(number);

        private IEnumerator ApplySize(RectTransform imageT, RectTransform taleT, float relative)
        {
            yield return null;

            if (relative > 1)
                imageT.sizeDelta = new Vector2(taleT.rect.height * relative, taleT.rect.height);
            else
                imageT.sizeDelta = new Vector2(taleT.rect.height, taleT.rect.height * (2 - relative));

            _applySizeRoutine = null;
        }
    }
}