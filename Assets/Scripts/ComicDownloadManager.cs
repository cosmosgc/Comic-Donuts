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

public class ComicDownloadManager : MonoBehaviour
{
    [Serializable]
    public class Source
    {
        [SerializeField] public string PostsURL;
        [SerializeField] public string PageSyntax;
        [SerializeField] public string PerPageSyntax;
        [SerializeField] public int PageNumber = 0;
        [SerializeField] public int PerPage = 10;
        [SerializeField] public string SuffixSyntax;
        [SerializeField] public string Options;
    }
    [Serializable]
    public class SearchIndex
    {
        [SerializeField] public string start;
        [SerializeField] public string finish;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
    }

    public int pageNumber = 1;
    public int perPage = 10;
    public Source source = new Source();
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

    public string searchIndexReference = "\"id\"";
    public string TextToFind = "\"curies\"";
    public string SearchCutContent;
    public string finalIndex;
    public string replace;
    public string replaceTo;
    public int cutLastIndex = 0;
    public string startSyntax;
    public string lastSyntax;

    public List<SearchIndex> searchIndex = new List<SearchIndex>();

    public List<string> postData;
    // Start is called before the first frame update
    void Start()
    {
        folder = PlayerPrefs.GetString("defaultFolder");
        PopulateSourcesList();
        //SourcePostsShowCreate();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (fileEntries[i].Name == sourceSelected+".json")
            {
                Debug.Log("Achou Source");
                string JsonFile = FileBrowserHelpers.ReadTextFromFile(fileEntries[i].Path);

                source = JsonUtility.FromJson<Source>(JsonFile);
            }
        }
        string sourceSyntax = source.PostsURL + source.PerPageSyntax + perPage.ToString() + source.PageSyntax + (pageNumber * perPage).ToString() + source.SuffixSyntax + source.Options;
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
            
            GameObject ComicPostInstance = Instantiate(comicItemPrefab, ComicItemContainer.transform);
            ComicPostInstance.name = folderEntries[i].Name;
            ComicPostInstance.GetComponent<ComicObject>().comicName = folderEntries[i].Name;
            ComicPostInstance.GetComponent<ComicObject>().url = folderEntries[i].Path;
            ComicPostInstance.GetComponent<ComicObject>().offline = true;
            ComicPostInstance.GetComponent<ComicObject>().comicScreen = comicScreen;
            ComicPostInstance.GetComponent<ComicObject>().comicSelectScreen = comicSelectScreen;
            ComicPostInstance.GetComponent<ComicObject>().photoViewer = _photoViewer;
            ComicPostInstance.GetComponent<ComicObject>().UpdatePostInfo();
        }
    }

    public void InstantiateWebPosts()
    {
        int cycleProtection = 0;
        textSample = postsText;
        postData.Clear();
        while (textSample.IndexOf(searchIndexReference) != -1 && cycleProtection < 300)
        {
            cycleProtection++;
            textSample = textSample.Substring(textSample.IndexOf(searchIndexReference) + searchIndexReference.Length);
            textSample = textSample.Substring(textSample.IndexOf(TextToFind) + TextToFind.Length);
            string toSend = SearchCutContent + textSample.Substring(0, textSample.IndexOf(finalIndex));
            toSend = toSend.Substring(0, toSend.Length - finalIndex.Length);
            toSend = startSyntax + toSend;
            toSend = toSend + lastSyntax;
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

            /*
            searchIndex.Add(new SearchIndex() { start = "\"id\": ", finish = ",", replace = "", replaceTo = "" });
            searchIndex.Add(new SearchIndex() { start = "\"\"title\":{\"rendered\":\"", finish = "\"", replace = "", replaceTo = "" });
            searchIndex.Add(new SearchIndex() { start = "\"self\":[{ \"href\":\"", finish = "\"", replace = "", replaceTo = "" });
            
            searchIndex.Add(_searchIndex);
            */
            for (int x = 0; x < searchIndex.Count; x++)
            {
                _postData = _postData.Substring(_postData.IndexOf(searchIndex[x].start) + searchIndex[x].start.Length);
                string toSend = _postData.Substring(0, _postData.IndexOf(searchIndex[x].finish));
                if(searchIndex[x].replace !=""|| searchIndex[x].replaceTo != "")
                {
                    toSend = toSend.Replace(searchIndex[x].replace, searchIndex[x].replaceTo);
                }
                
                _postInfo.Add(toSend);
            }
            GameObject ComicPostInstance = Instantiate(comicItemPrefab, ComicItemContainer.transform);
            ComicPostInstance.name = _postInfo[1];
            ComicPostInstance.GetComponent<ComicObject>().comicName = _postInfo[1];
            ComicPostInstance.GetComponent<ComicObject>().url = _postInfo[2];
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
                byte[] bConvert = UnicodeEncoding.Convert(Encoding.UTF8, Encoding.Unicode, uwr.downloadHandler.data);
                string Text = Encoding.Unicode.GetString(bConvert); ;

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

    public void selectSource(string info)
    {
        sourceSelected = info;
    }




    public class PostContent
    {
        public int id { get; set; }
        public int postName { get; set; }
        public int postPagesCount { get; set; }
        public int postLink { get; set; }
        public int postContent { get; set; }
        public int data { get; set; }
    }

    public class WordpressAPI
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
        public int author { get; set; }
        public int featured_media { get; set; }
        public string comment_status { get; set; }
        public string ping_status { get; set; }
        public bool sticky { get; set; }
        public string template { get; set; }
        public string format { get; set; }
        public List<int> categories { get; set; }
        public List<int> tags { get; set; }
    }
    public class WordpressList
    {
        public WordpressAPI[] wordpressList { get; set; }
    }
    public class Title
    {
        public string rendered { get; set; }
    }
    public class Content
    {
        public string rendered { get; set; }
        public bool @protected { get; set; }
    }
}
