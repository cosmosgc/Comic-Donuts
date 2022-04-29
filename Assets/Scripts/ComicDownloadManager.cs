using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26;
using TMPro;
using System.Text;
using SimpleFileBrowser;
using UnityEngine.UI;
using Madhur.InfoPopup;

public class ComicDownloadManager : MonoBehaviour
{

    public int pageNumber = 1;
    public int perPage = 10;
    public SourceClass.source source = new SourceClass.source();
    [SerializeField] private PhotoViewer.Scripts.PhotoViewer _photoViewer = null;
    public string folder;
    public FileSystemEntry comicPath = new FileSystemEntry();
    public FileSystemEntry sourcePath = new FileSystemEntry();

    public string sourceSelected;
    public GameObject comicItemPrefab;
    public GameObject ComicItemContainer;

    public GameObject SourceItemContainer;
    public GameObject SourceItemPrefab;


    public GameObject comicScreen;
    public GameObject comicSelectScreen;

    public TextMeshProUGUI pageText;

    [TextArea(3, 10)]
    public string postsText;
    [TextArea(3, 10)]
    public string textSample;


    public List<string> postData;

    private CoroutineQueue queue;
    // Start is called before the first frame update
    void Start()
    {
        folder = PlayerPrefs.GetString("defaultFolder");
        PopulateSourcesList();
        if (queue == null)
        {
            queue = new CoroutineQueue(1, StartCoroutine);
        }
        if(sourceSelected == "")
        {
            sourceSelected = PlayerPrefs.GetString("defaultSource");
        }
    }

    public void getFoldersPath()
    {
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(PlayerPrefs.GetString("defaultFolder"), false);
        for (int i = 0; i < _folders.Length; i++)
        {
            Debug.Log(_folders[i].Name);
            if (_folders[i].Name == "sources")
            {
                sourcePath = _folders[i];
            }
            if (_folders[i].Name == "Comics")
            {
                comicPath = _folders[i];
            }
        }
    }
    public void SourcePostsShowCreate()
    {
        if (sourcePath.Path == null)
        {
            getFoldersPath();
        }
        
        FileSystemEntry[] fileEntries = FileBrowserHelpers.GetEntriesInDirectory(sourcePath.Path, false);
        foreach (Transform child in ComicItemContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < fileEntries.Length; i++)
        {
            //Debug.Log("Comparando " + fileEntries[i].Name + " com " + sourceSelected + ".json");
            if (fileEntries[i].Name == sourceSelected)
            {
                Debug.Log("Achou Source");
                string JsonFile = FileBrowserHelpers.ReadTextFromFile(fileEntries[i].Path);

                source = JsonUtility.FromJson<SourceClass.source>(JsonFile);
            }
        }
        string sourceSyntax = source.PostsURL + source.PageSyntax + (pageNumber * perPage).ToString() + source.PerPageSyntax + perPage.ToString() + source.SuffixSyntax + source.Options;
        if(postsText != "")
        {
            InstantiateWebPosts();
            return;
        }
        Debug.Log("Source Pego " + sourceSyntax);
        Uri url = new Uri(sourceSyntax);
        StartCoroutine(DownloadPostsPages(url));
    }
    public void LocalPostsShowCreate()
    {
        if (comicPath.Path == null)
        {
            getFoldersPath();
        }
        FileSystemEntry[] folderEntries = FileBrowserHelpers.GetEntriesInDirectory(comicPath.Path, false);
        
        foreach (Transform child in ComicItemContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < folderEntries.Length; i++)
        {
            queue.Run(LocalInstancingComic(folderEntries[i]));
        }
    }

    public IEnumerator LocalInstancingComic(FileSystemEntry comicFolder)
    {
        Debug.Log("Instanciando: " + comicFolder.Path);
        GameObject ComicPostInstance = Instantiate(comicItemPrefab, ComicItemContainer.transform);
        ComicPostInstance.name = comicFolder.Name;
        ComicPostInstance.GetComponent<ComicObject>().comicName = comicFolder.Name;
        ComicPostInstance.GetComponent<ComicObject>().url = comicFolder.Path;
        ComicPostInstance.GetComponent<ComicObject>().offline = true;
        ComicPostInstance.GetComponent<ComicObject>().comicScreen = comicScreen;
        ComicPostInstance.GetComponent<ComicObject>().comicSelectScreen = comicSelectScreen;
        ComicPostInstance.GetComponent<ComicObject>().photoViewer = _photoViewer;
        ComicPostInstance.GetComponent<ComicObject>().UpdatePostInfo();
        yield return null;
    }

    public void InstantiateWebPosts()
    {
        int cycleProtection = 0;
        textSample = postsText;
        postData.Clear();
        while (textSample.IndexOf(source.postParser.searchIndexReference) != -1 && cycleProtection < 300)
        {
            cycleProtection++;
            if (source.postParser.replace != "" || source.postParser.replaceTo != "")
            {
                textSample = textSample.Replace(source.postParser.replace, source.postParser.replaceTo);
            }
            textSample = textSample.Substring(textSample.IndexOf(source.postParser.searchIndexReference) + source.postParser.searchIndexReference.Length);
            //textSample = textSample.Substring(textSample.IndexOf(source.postParser.TextToFind) + source.postParser.TextToFind.Length);
            string toSend = textSample.Substring(0, textSample.IndexOf(source.postParser.TextToFindFinish));
            //toSend = toSend.Substring(0, toSend.Length - source.postParser.TextToFindFinish.Length);
            //toSend = source.postParser.startSyntax + toSend;
            
            toSend = source.postParser.PrefixMissing + toSend + source.postParser.SuffixMissing;
            Debug.Log(toSend);
            postData.Add(toSend);
        }
        CreatePostData();
    }

    public void CreatePostData()
    {
        foreach (Transform child in ComicItemContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < postData.Count; i++)
        {
            string _postData = postData[i];
            List<string> _postInfo = new List<string>();

            Debug.Log(source.ToString());
            for (int x = 0; x < source.searchs.Count; x++)
            {
                _postData = _postData.Substring(_postData.IndexOf(source.searchs[x].start) + source.searchs[x].start.Length);
                string toSend = _postData.Substring(0, _postData.IndexOf(source.searchs[x].finish));
                if(source.searchs[x].replace !=""|| source.searchs[x].replaceTo != "")
                {
                    toSend = toSend.Replace(source.searchs[x].replace, source.searchs[x].replaceTo);
                }
                
                _postInfo.Add(toSend);
            }
            GameObject ComicPostInstance = Instantiate(comicItemPrefab, ComicItemContainer.transform);
            ComicPostInstance.name = _postInfo[0];
            ComicPostInstance.GetComponent<ComicObject>().comicName = _postInfo[0];
            ComicPostInstance.GetComponent<ComicObject>().url = _postInfo[1];
            ComicPostInstance.GetComponent<ComicObject>().rawPostData = postData[i];
            ComicPostInstance.GetComponent<ComicObject>().comicScreen = comicScreen;
            ComicPostInstance.GetComponent<ComicObject>().comicSelectScreen = comicSelectScreen;
            ComicPostInstance.GetComponent<ComicObject>().photoViewer = _photoViewer;
            ComicPostInstance.GetComponent<ComicObject>().sourceSelected = sourceSelected;
            ComicPostInstance.GetComponent<ComicObject>().UpdatePostInfo();
        }
    }

    public void Page(int page)
    {
        pageNumber += page;
        if (pageText)
        {
            pageText.text = pageNumber.ToString();
        }
        SourcePostsShowCreate();
    }
    IEnumerator DownloadPostsPages(Uri url2)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url2))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                //byte[] bytes = Encoding.Default.GetBytes(uwr.downloadHandler.text);
                //byte[] bConvert = UnicodeEncoding.Convert(Encoding.UTF8, Encoding.Unicode, uwr.downloadHandler.data);
                string Text = Encoding.Unicode.GetString(UnicodeEncoding.Convert(Encoding.UTF8, Encoding.Unicode, uwr.downloadHandler.data)); ;

                postsText = Text;
            }
        }
        InstantiateWebPosts();
    }
    public void clearMemory()
    {
        Resources.UnloadUnusedAssets();
    }

    public void PopulateSourcesList()
    {
        foreach (Transform child in SourceItemContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (sourcePath.Path == null || sourcePath.Path == "")
        {
            getFoldersPath();
        }
        FileSystemEntry[] fileEntries = FileBrowserHelpers.GetEntriesInDirectory(sourcePath.Path, false);
        for (int i = 0; i < fileEntries.Length; i++)
        {
            GameObject sourceInstance = Instantiate(SourceItemPrefab, SourceItemContainer.transform);
            sourceInstance.name = fileEntries[i].Name;
            sourceInstance.GetComponent<SourceObject>().sourceName = fileEntries[i].Name;
            sourceInstance.GetComponent<Button>().onClick.AddListener(() => selectSource(sourceInstance.GetComponent<SourceObject>().sourceName));
            sourceInstance.GetComponent<SourceObject>().UpdateInfo();
        }
    }

    public void OpenComicFolder()
    {
        if(comicPath.Path == null)
        {
            getFoldersPath();
        }
        Application.OpenURL(comicPath.Path);
    }
    public void selectSource(string info)
    {
        sourceSelected = info;
        PlayerPrefs.SetString("defaultSource", info);
        InfoPopupUtil.ShowInformation("Selecionou [" + sourceSelected + "]!");
    }

}
