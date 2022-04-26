using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class DownloadandSave : MonoBehaviour
{
    public string folderPath;
    public string FileFolderName;
    public Uri url;
    public string link;
    public TextMeshProUGUI Debugtext;
    public TextMeshProUGUI cutIndex;
    public TMP_InputField linkPreview;
    public TMP_InputField countPreview;
    [TextArea(3, 10)]
    public string htmlSample;
    [TextArea(3, 10)]
    public string textSample;


    public string TextToFind = "src =\"";
    public string searchIndexReference = "<img src=\"https:";
    public string SearchCutContent = "https://";
    public string replace = @"\/";
    public string replaceTo = "/";
    public int cutLastIndex = 1;

    public string imageURL;
    public string debugLink;
    
    public List<string> gallery = new List<string>();


    public bool CoroutineIsRunning;
    void Start()
    {
        folderPath = PlayerPrefs.GetString("defaultFolder");
    }

    public void changeLink(string newLink)
    {
        link = newLink;
    }
    public void startSearch(string searchLink)
    {
        Debug.Log("Iniciando Pesquisa");
        gallery.Clear();
        if(searchLink == "")
        {
            StartCoroutine(Get(link));
        }
        else
        {
            StartCoroutine(Get(searchLink));
        }
        
    }

    public void changeRef(string info)
    {
        searchIndexReference = info;
        testPreview();
    }
    public void changeParseName(string info)
    {
        FileFolderName = info;
    }
    public void changeCut(string info)
    {
        SearchCutContent = info;
        testPreview();
    }
    public void ChangeTextToFind(string info)
    {
        TextToFind = info;
        testPreview();
    }
    public void changeReplace(string info)
    {
        replace = info;
        testPreview();
    }
    public void changeReplaceTo(string info)
    {
        replaceTo = info;
        testPreview();
    }

    public void changecutLastIndex(float info)
    {
        cutLastIndex = (int)info;
        if (cutIndex)
        {
            cutIndex.text = info.ToString();
        }
        testPreview();
    }

    public void testPreview()
    {
        gallery.Clear();
        Debug.Log("Testando link");
        if(htmlSample != "")
        {
            textSample = htmlSample;
        }

        textSample = textSample.Substring(textSample.IndexOf(searchIndexReference) + searchIndexReference.Length);
        textSample = textSample.Substring(textSample.IndexOf(TextToFind) + TextToFind.Length);
        imageURL = SearchCutContent + textSample.Substring(0, textSample.IndexOf("\""));
        string newImageURL = imageURL.Replace(replace, replaceTo);
        newImageURL = newImageURL.Substring(0, newImageURL.Length - cutLastIndex);
        linkPreview.text = newImageURL;
    }
    public void testCountPreview()
    {
        gallery.Clear();
        Debug.Log("Testando link");
        if (htmlSample != "")
        {
            textSample = htmlSample;
        }

        int cycleProtection = 0;
        while (textSample.IndexOf(searchIndexReference) != -1 && cycleProtection < 300)
        {
            cycleProtection++;
            textSample = textSample.Substring(textSample.IndexOf(searchIndexReference) + searchIndexReference.Length);
            textSample = textSample.Substring(textSample.IndexOf(TextToFind) + TextToFind.Length);
            imageURL = SearchCutContent + textSample.Substring(0, textSample.IndexOf("\""));
            string newImageURL = imageURL.Replace(replace, replaceTo);
            newImageURL = newImageURL.Substring(0, newImageURL.Length - cutLastIndex);
            //Uri imageUri = new Uri(newImageURL);
            gallery.Add(newImageURL);
        }
        countPreview.text = gallery.Count.ToString();
    }

    public void savePostParser()
    {
        if (folderPath != PlayerPrefs.GetString("defaultFolder"))
        {
            folderPath = PlayerPrefs.GetString("defaultFolder");
        }
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(folderPath + "/sources/" + FileFolderName)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(folderPath + "/sources/" + FileFolderName));
            Debug.Log("Creating now");
        }
        else
        {
            Debug.Log(folderPath + " does exist");
        }

        try
        {
            PostParse postParser = new PostParse();
            postParser.TextToFind = TextToFind;
            postParser.SearchCutContent = SearchCutContent;
            postParser.searchIndexReference = searchIndexReference;
            postParser.replace = replace;
            postParser.replaceTo = replaceTo;
            postParser.cutLastIndex = cutLastIndex;


            string results = JsonUtility.ToJson(postParser, true);
            Debug.Log(results + postParser + postParser.TextToFind);
            File.WriteAllText(folderPath+"/sources/"+FileFolderName+".json", results);
            Debug.Log("Saved Data to: " + folderPath + "/sources/" + FileFolderName + ".json".Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + folderPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }


    [System.Serializable]
    public class Title
    {
        public string rendered { get; set; }
    }

    public class Content
    {
        public string rendered { get; set; }
        public bool @protected { get; set; }
    }
    public class Root
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public DateTime date_gmt { get; set; }
        public Guid guid { get; set; }
        public DateTime modified { get; set; }
        public DateTime modified_gmt { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public Title title { get; set; }
        public Content content { get; set; }
        public List<int> categories { get; set; }
        public List<object> tags { get; set; }
    }
    [Serializable]
    public class PostParse
    {
        [SerializeField] public string TextToFind;
        [SerializeField] public string searchIndexReference;
        [SerializeField] public string SearchCutContent;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
        [SerializeField] public int cutLastIndex;
    }

    IEnumerator DownloadingImage(Uri url2, string fileName, string fileFormat)
    {
        Debug.Log("Start Downloading Images");
        Debugtext.text = "Baixando imagens";
        CoroutineIsRunning = true;

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url2))
        {
            // uwr2.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                Debugtext.text = "Successo!";
                Debug.Log("Success" + uwr.error);
                Texture myTexture = DownloadHandlerTexture.GetContent(uwr);
                byte[] results = uwr.downloadHandler.data;
                fileName = Path.GetFileName(uwr.downloadHandler.text); 
                if(fileFormat == "")
                {
                    fileFormat = ".png";
                }
                if (fileName == "")
                {
                    fileName = gameObject.name + fileFormat;
                }
                // saveImage(folderPath, results);            // Not a folder path
                saveImage(folderPath + "/" + FileFolderName + "/" + fileName, results);  // give filename 
            }
        }
        CoroutineIsRunning = false;
    }

    void saveImage(string path, byte[] imageBytes)
    {
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Debug.Log("Creating now");
        }
        else
        {
            Debug.Log(path + " does exist");
        }

        try
        {
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public void DownloadGallery()
    {
        Debug.Log("Downloading Gallery");
        for (int i = 0; i < gallery.Count; i++)
        {
            Uri imageURI = new Uri(gallery[i]);
            Debug.Log("Downloading :" + imageURI);
            StartCoroutine(DownloadingImage(imageURI, i.ToString(), ".png"));
        }
    }
    public IEnumerator Get(string urli)
    {
        using (UnityWebRequest html = UnityWebRequest.Get(urli))
        {
            yield return html.SendWebRequest();

            if(html.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + html.error);
            }
            else
            {
                Debug.Log("Received: " + html.error);
                int cycleProtection = 0;
                textSample = html.downloadHandler.text;
                Debugtext.text = textSample;
                htmlSample = textSample;
                while (textSample.IndexOf(searchIndexReference) != -1 && cycleProtection < 300)
                {
                    
                    cycleProtection++;
                    textSample = textSample.Substring(textSample.IndexOf(searchIndexReference) + searchIndexReference.Length);
                    textSample = textSample.Substring(textSample.IndexOf(TextToFind) + TextToFind.Length);
                    imageURL = SearchCutContent+textSample.Substring(0, textSample.IndexOf("\""));
                    string newImageURL = imageURL.Replace(replace, replaceTo);
                    newImageURL = newImageURL.Substring(0, newImageURL.Length - cutLastIndex);
                    //Uri imageUri = new Uri(newImageURL);
                    gallery.Add(newImageURL);
                }
                //Debug.Log(SiteComic.ToString());
            }
        }
    }
}

