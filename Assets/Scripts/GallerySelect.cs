using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

namespace PhotoViewer.Scripts
{
    public class GallerySelect : MonoBehaviour
    {
        [SerializeField] private PhotoViewer _photoViewer = null;

        public bool GalleryReady = false;
        public string folder;
        public List<ImageData> image = new List<ImageData>();
        public List<string> links;
        public List<Texture2D> texture;
        private ImageData _img;

        private string[] fileInfo;

        public void Start()
        {
            folder = PlayerPrefs.GetString("defaultFolder");
            //GalleryCreate();
        }

        public void Update()
        {
            if (GalleryReady)
            {
                GalleryFinish();

                GalleryReady = false;
            }
        }

        public void GalleryCreate(string galleryFolder)
        {
            GalleryReady = false;
            string[] fileEntries = Directory.GetFiles(galleryFolder);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                links.Add(fileEntries[i]);
            }
            Debug.Log("Arquivos Pegos");
            CreateImagesData();
        }

        public void CreateImagesData()
        {
            for (int i = 0; i < links.Count; i++)
            {
                StartCoroutine(GetGalleryPhoto(links[i], i));
            }
        }

        public void GalleryFinish()
        {
            _photoViewer.Clear();
            Debug.Log("Texturas Criadas Criada");
            image.Sort((x, y) => string.Compare(x.Name, y.Name)); ;
            _photoViewer.AddImageData(image);
            _photoViewer.Show();
            Debug.Log("Galeria Criada");
        }

        public IEnumerator GetGalleryPhoto(string url, int index)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    _img.Name = Path.GetFileName(links[index]);
                    Texture2D _tex = DownloadHandlerTexture.GetContent(uwr);
                    _tex.Compress(false);
                    _img.Sprite = Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0, 0));
                    texture.Add(DownloadHandlerTexture.GetContent(uwr));
                    image.Add(_img);
                    if (texture.Count == links.Count)
                    {
                        GalleryReady = true;
                    }
                }
            }
        }


    }
}
    
