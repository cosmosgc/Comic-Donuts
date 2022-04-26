using UnityEngine;

namespace PhotoViewer.Scripts.Panorama
{
    [RequireComponent(typeof(AbstractView))]
    public class PanoramaInput : AbstractInput
    {
        protected override void OnUpdate() =>
            _view.ApplyInput(_deltaPosition);
    }
}