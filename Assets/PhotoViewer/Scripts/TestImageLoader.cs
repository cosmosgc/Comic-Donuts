using UnityEngine;

namespace PhotoViewer.Scripts
{
    public class TestImageLoader : MonoBehaviour
    {
        [SerializeField] private PhotoViewer _photoViewer = null;
        [SerializeField] private TestImageLibrary _imageLibrary = null;

        private void Start()
        {
            _photoViewer.AddImageData(_imageLibrary.ImageDatas);

            _photoViewer.Show();
        }
    }
}