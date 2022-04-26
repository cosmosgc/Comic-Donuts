using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotoViewer.Scripts.Gallery
{
    public class GalleryTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _name;
        [SerializeField] private Text _date;

        public event Action<int> OnClick;
        public int Number { get; set; }

        public Image Image
        {
            get => _image;
            set => _image = value;
        }

        public Text Name
        {
            get => _name;
            set => _name = value;
        }

        public Text Date
        {
            get => _date;
            set => _date = value;
        }

        public void OnPointerClick(PointerEventData eventData) => 
            OnClick?.Invoke(Number);
    }
}