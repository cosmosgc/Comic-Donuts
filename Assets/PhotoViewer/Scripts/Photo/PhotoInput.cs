using UnityEngine;

namespace PhotoViewer.Scripts.Photo
{
    [RequireComponent(typeof(AbstractView))]
    public class PhotoInput : AbstractInput
    {
        public float speed = 2;
        protected override void OnUpdate()
        {
            _view.ApplyInput(_deltaPosition * speed);
        }
    }
}