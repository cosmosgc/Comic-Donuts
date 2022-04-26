using UnityEngine;

namespace PhotoViewer.Scripts.Panorama
{
    public class PanoramaRotator : MonoBehaviour
    {
        public float _clamp=60f;

        public float SetClamp(float value) =>
            _clamp = value;

        public void OnRotate(Vector2 deltaValue)
        {
            var euler = transform.rotation.eulerAngles;
            euler = new Vector3(euler.x + deltaValue.y, euler.y - deltaValue.x, euler.z);

            if (euler.x > 180)
                euler.x -= 360;

            euler.x = Mathf.Clamp(euler.x, -_clamp, _clamp);

            transform.rotation = Quaternion.Euler(euler);
        }
    }
}