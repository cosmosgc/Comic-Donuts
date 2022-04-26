using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using SimpleFileBrowser;
using UnityEngine.Networking;

public class SourceManager : MonoBehaviour
{

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

    [Serializable]
    public class Source
    {
        [SerializeField] public string PostsURL;
        [SerializeField] public string PageSyntax;
        [SerializeField] public string PerPageSyntax;
        [SerializeField] public int PageNumber;
        [SerializeField] public int PerPage;
        [SerializeField] public string SuffixSyntax;
        [SerializeField] public string Options;
    }
    [Serializable]
    public class SearchIndex
    {
        [SerializeField] public string name;
        [SerializeField] public string start;
        [SerializeField] public string finish;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
    }
    [Serializable]
    public class imageSearch
    {
        [SerializeField] public string start;
        [SerializeField] public string finish;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
        [SerializeField] public string PrefixMissing;
        [SerializeField] public string SuffixMissing;
    }
    [Serializable]
    public class newSource
    {
        [SerializeField] public string PostsURL;
        [SerializeField] public string PageSyntax;
        [SerializeField] public string PerPageSyntax;
        [SerializeField] public int PageNumber;
        [SerializeField] public int PerPage;
        [SerializeField] public string SuffixSyntax;
        [SerializeField] public string Options;
        [SerializeField] public string replaceTo;

        [SerializeField] public List<SearchIndex> searchs;
        [SerializeField] public imageSearch imageSearch;
    }

    public FileSystemEntry comicPath = new FileSystemEntry();
    public FileSystemEntry sourcePath = new FileSystemEntry();
    public newSource source = new newSource();
    public List<SearchIndex> searchIndex = new List<SearchIndex>();
    public imageSearch image = new imageSearch();

    public string folder;
    public string FileFolderName;
    public TextMeshProUGUI DebugText;
    public TMP_InputField linkPreview;
    public TMP_InputField SearchPreview;
    public TMP_InputField Imagepreview;
    public TMP_InputField jsonPreview;

    public string HTMLContent;
    public string PostHTMLContent;

    public string jsonToSave;



    // Start is called before the first frame update
    void Start()
    {
        folder = PlayerPrefs.GetString("defaultFolder");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeFileFolderName(string info)
    {
        FileFolderName = info;
    }
    public void changePostURL(string info)
    {
        source.PostsURL = info;
        PreviewInfo();
    }
    public void changePageSyntax(string info)
    {
        source.PageSyntax = info;
        PreviewInfo();
    }
    public void changePerPageSyntax(string info)
    {
        source.PerPageSyntax = info;
        PreviewInfo();
    }
    public void changeSuffixSyntax(string info)
    {
        source.SuffixSyntax = info;
        PreviewInfo();
    }
    public void changeOptions(string info)
    {
        source.Options = info;
        PreviewInfo();
    }

    //Post Search stuff
    //nome
    public void changeSearchNameStart(string info)
    {
        searchIndex[0].start = info;
        PreviewInfoSearch();
    }
    public void changeSearchNameFinish(string info)
    {
        searchIndex[0].finish = info;
        PreviewInfoSearch();
    }
    public void changeSearchNameReplace(string info)
    {
        searchIndex[0].replace = info;
        PreviewInfoSearch();
    }
    public void changeSearchNameReplaceTo(string info)
    {
        searchIndex[0].replaceTo = info;
        PreviewInfoSearch();
    }

    //Link do post
    public void changeSearchLinkStart(string info)
    {
        searchIndex[1].start = info;
        PreviewInfoSearch();
    }
    public void changeSearchLinkFinish(string info)
    {
        searchIndex[1].finish = info;
        PreviewInfoSearch();
    }
    public void changeSearchLinkReplace(string info)
    {
        searchIndex[1].replace = info;
        PreviewInfoSearch();
    }
    public void changeSearchLinkReplaceTo(string info)
    {
        searchIndex[1].replaceTo = info;
        PreviewInfoSearch();
    }

    //Thumbnail
    public void changeSearchThumbStart(string info)
    {
        searchIndex[2].start = info;
        //PreviewInfoSearch();
    }
    public void changeSearchThumbFinish(string info)
    {
        searchIndex[2].finish = info;
        //PreviewInfoSearch();
    }
    public void changeSearchThumbReplace(string info)
    {
        searchIndex[2].replace = info;
        //PreviewInfoSearch();
    }
    public void changeSearchThumbReplaceTo(string info)
    {
        searchIndex[2].replaceTo = info;
        //PreviewInfoSearch();
    }

    //ImagemCapture
    public void changeImageSearchStart(string info)
    {
        image.start = info;
        postImagePreview();
    }
    public void changeImageSearchFinish(string info)
    {
        image.finish = info;
        postImagePreview();
    }
    public void changeImageSearchReplace(string info)
    {
        image.replace = info;
        postImagePreview();
    }
    public void changeImageSearchReplaceTo(string info)
    {
        image.replaceTo = info;
        postImagePreview();
    }
    public void changeImageSearchPrefix(string info)
    {
        image.replaceTo = info;
        postImagePreview();
    }
    public void changeImageSearchSuffix(string info)
    {
        image.replaceTo = info;
        postImagePreview();
    }

    public void PreviewInfo()
    {
        //DebugText.text = JsonUtility.ToJson(source, true); 
        string _options = "";
        if (source.Options != "")
        {
            _options = source.Options + "Tirinhas";
        }
        linkPreview.text = source.PostsURL + source.PageSyntax + "1" + source.PerPageSyntax + "5" + source.SuffixSyntax + _options;
    }
    public void PreviewInfoSearch()
    {
        //DebugText.text = JsonUtility.ToJson(source, true); 
        string _nome = parsePreview(0);
        string _link = parsePreview(1);
        Debug.Log(_link);
        SearchPreview.text = "Nome: \"" + _nome + "\" ||  Link: \"" + _link + "\"";
    }

    public string parsePreview(int index)
    {
        string _preview = "";
        string _toSend = "";
        
        _preview = HTMLContent.Substring(HTMLContent.IndexOf(searchIndex[index].start) + searchIndex[index].start.Length);
        _toSend = _preview.Substring(0, _preview.IndexOf(searchIndex[index].finish));
        if (searchIndex[index].replace != "" || searchIndex[index].replaceTo != "")
        {
            _toSend = _toSend.Replace(searchIndex[index].replace, searchIndex[index].replaceTo);
        }
        return _toSend;
    }
    public string postImagePreview()
    {
        string _preview = "";
        string toSend = "";

        _preview = PostHTMLContent.Substring(PostHTMLContent.IndexOf(image.start) + image.start.Length);
        toSend = _preview.Substring(0, _preview.IndexOf(image.finish));
        if (image.replace != "" || image.replaceTo != "")
        {
            toSend = toSend.Replace(image.replace, image.replaceTo);
        }
        return toSend;
    }

    public void getHTMLContent()
    {
        string info = "";
        info = source.PostsURL + source.PageSyntax + "1" + source.PerPageSyntax + "5" + source.SuffixSyntax;
        StartCoroutine(Get(info));
    }
    public void getPostHTMLContent()
    {
        string _post = parsePreview(1);
        StartCoroutine(GetPost(_post));
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

    public void previewJson()
    {

        source.searchs = searchIndex;
        source.imageSearch = image;

        jsonToSave = JsonUtility.ToJson(source, true);
        jsonPreview.text = jsonToSave;
    }
    public void savePostParser()
    {
        FileSystemEntry _sourcePath = new FileSystemEntry();
        string _newSourcePath = "";
        //Create Directory if it does not exist
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(PlayerPrefs.GetString("defaultFolder"), false);
        for (int i = 0; i < _folders.Length; i++)
        {
            if (_folders[i].Name == "sources")
            {
                _newSourcePath = _folders[i].Path;
            }
        }
        if (_sourcePath.Path == null)
        {
            _newSourcePath = FileBrowserHelpers.CreateFolderInDirectory(PlayerPrefs.GetString("defaultFolder"), "sources");
            Debug.Log("Criando pasta em: " + _newSourcePath);
        }
        else
        {
            //Debug.Log(path + " does exist");
        }
        if (_newSourcePath == "")
        {
            _newSourcePath = _sourcePath.Path;
        }

        try
        {
            previewJson();
            string _filePath = FileBrowserHelpers.CreateFileInDirectory(_newSourcePath, FileFolderName+".json");
            FileBrowserHelpers.WriteTextToFile(_filePath, jsonToSave);
            Debug.Log("Saved Data to: " + (_filePath).Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + _newSourcePath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public IEnumerator Get(string urli)
    {
        using (UnityWebRequest html = UnityWebRequest.Get(urli))
        {
            yield return html.SendWebRequest();

            if (html.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + html.error);
            }
            else
            {
                Debug.Log("Received: " + html.error);

                HTMLContent = html.downloadHandler.text;
            }
        }
    }

    public IEnumerator GetPost(string urli)
    {
        using (UnityWebRequest html = UnityWebRequest.Get(urli))
        {
            yield return html.SendWebRequest();

            if (html.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + html.error);
            }
            else
            {
                Debug.Log("Received: " + html.error);

                PostHTMLContent = html.downloadHandler.text;
            }
        }
    }
}
