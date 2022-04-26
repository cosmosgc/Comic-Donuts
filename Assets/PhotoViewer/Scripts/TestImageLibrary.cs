using System.Collections.Generic;
using UnityEngine;

namespace PhotoViewer.Scripts
{
    [CreateAssetMenu(fileName = "ImageLibrary", menuName = "ImageLibrary", order = 0)]
    public class TestImageLibrary : ScriptableObject
    {
        public List<ImageData> ImageDatas = new List<ImageData>();
    }
}