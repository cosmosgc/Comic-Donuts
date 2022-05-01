using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using PhotoViewer.Scripts;
using SimpleFileBrowser;
using Madhur.InfoPopup;
using Proyecto26;

public class ComicObject : MonoBehaviour
{
    public string comicName;
    public string url;
    public string totalPages;
    public bool offline;
    [SerializeField] public List<FileSystemEntry> pagesURL = new List<FileSystemEntry>();
    public FileSystemEntry comicPath = new FileSystemEntry();
    public FileSystemEntry sourcePath = new FileSystemEntry();

    public string comicFolder;

    public List<string> OnlinePagesURL = new List<string>();
    public string rawPostData;
    public string sourceSelected;
    public TextMeshProUGUI comicNameText;
    public TextMeshProUGUI totalPagesText;
    public Button commicButton;

    public GameObject comicScreen;
    public GameObject comicSelectScreen;

    public Slider downloadBar;


    [SerializeField] public PhotoViewer.Scripts.PhotoViewer photoViewer = null;
    private List<ImageData> image = new List<ImageData>();
    private List<string> DownloadSucess = new List<string>();

    [TextArea(3, 10)]
    public string textSample;

    public SourceClass.source source = new SourceClass.source();

    //public List<Texture2D> PagesImages = new List<Texture2D>();
    private Texture2D thumbnail;
    public Image sprite;

    FileSystemEntry[] _ComicPages;

    private CoroutineQueue queue;
    private uint downloadLimit = 1;

    private void Start()
    {
        startQueue();
    }

    private void startQueue()
    {
        if (queue == null)
        {
            if (PlayerPrefs.GetInt("defaultDownloads") > 1)
            {
                downloadLimit = (uint)PlayerPrefs.GetInt("defaultDownloads");
            }
            queue = new CoroutineQueue(downloadLimit, StartCoroutine);
        }
    }
    public void UpdatePostInfo()
    {
        startQueue();
        comicNameText.text = comicName;
        if (offline)
        {
            GetLocalComicInfo(url);
        }
        else
        {
            GetSourceComicInfo(url);
        }
    }

    public void getSourceClass()
    {
        if (sourcePath.Path == null)
        {
            getFoldersPath();
        }

        FileSystemEntry[] fileEntries = FileBrowserHelpers.GetEntriesInDirectory(sourcePath.Path, false);
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
    }
    public void GetLocalComicInfo(string info)
    {
        FileSystemEntry[] fileEntries = FileBrowserHelpers.GetEntriesInDirectory(info, false);
        for (int i = 0; i < fileEntries.Length; i++)
        {
            pagesURL.Add(fileEntries[i]);
        }
        if (pagesURL.Count > 0)
        {
            queue.Run(LoadLocalImage(pagesURL[0]));
        }
        commicButton.onClick.AddListener(offLineComicShow);
    }

    public void GetSourceComicInfo(string comicPath)
    {
        if (OnlinePagesURL.Count == 0)
        {
            getPagesLink();
        }
        
        
        commicButton.onClick.AddListener(DownloadSourceComic);
    }

    public void getThumbnailOnline()
    {
        if(source.searchs[2].name == "thumbnail" && source.searchs[2].link != null)
        {
            Uri thumb = new Uri(source.searchs[2].link);
            queue.Run(StreamImage(thumb));
            return;
        }
        string _comicName = comicName.Replace("\\", "");
        _comicName = _comicName.Replace("/", "");
        bool _exist = false;
        if (comicPath.Path == null)
        {
            getFoldersPath();
        }
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(comicPath.Path, false);
        for (int i = 0; i < _folders.Length; i++)
        {
            if (_folders[i].Name == _comicName)
            {
                _exist = true;
                break;
            }
        }
        if (_exist)
        {
            PopulateComicPagesPath();
            if(_ComicPages.Length > 0)
            {
                queue.Run(LoadLocalImage(_ComicPages[0]));
            }
            return;
        }

        Uri _dImage = new Uri(OnlinePagesURL[0]);
        queue.Run(StreamImage(_dImage));
    }

    public void offLineComicShow()
    {
        Debug.Log("Fetching Comic");
        downloadBar.gameObject.SetActive(true);
        downloadBar.maxValue = pagesURL.Count;
        if (pagesURL.Count == 0)
        {
            GetLocalComicInfo(url);
        }
        for (int i = 0; i < pagesURL.Count; i++)
        {
            if (image.Count == 1 && i == 0)
            {
                continue;
            }
            queue.Run(LoadLocalImage(pagesURL[i], i));
        }
    }

    public void ShowComic()
    {
        comicScreen.SetActive(true);
        //photoViewer.Clear();
        image.Sort((x, y) => string.Compare(x.Name, y.Name)); ;
        photoViewer.AddImageData(image);
        photoViewer.Show();
        Debug.Log("Galeria Criada");
        image.Clear();
        //PagesImages.Clear();
        clearMemory();
        comicSelectScreen.SetActive(false);
    }

    public void PopulateComicPagesPath()
    {
        string _comicName = comicName.Replace("\\", "");
        _comicName = _comicName.Replace("/", "");
        Debug.Log("PopulandoComicPagesPath de: " + _comicName);
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(comicPath.Path, false);
        if (_folders != null && _folders.Length > 0)
        {
            for (int x = 0; x < _folders.Length; x++)
            {
                if (_folders[x].Name == _comicName)
                {
                    _ComicPages = FileBrowserHelpers.GetEntriesInDirectory(_folders[x].Path, false);
                }
            }
        }
    }
    public void DownloadSourceComic()
    {
        if(comicPath.Path == null)
        {
            getFoldersPath();
        }
        Debug.Log("Baixando comic: " + comicName);
        if(OnlinePagesURL.Count == 0)
        {
            getPagesLink();
        }
        image.Clear();

        downloadBar.gameObject.SetActive(true);
        downloadBar.maxValue = OnlinePagesURL.Count;

        if (_ComicPages == null)
        {
            PopulateComicPagesPath();
        }

        for (int i = 0; i < OnlinePagesURL.Count; i++)
        {
            string fileName = Path.GetFileName(OnlinePagesURL[i]);
            string _comicName = comicName.Replace("\\", "");
            _comicName = _comicName.Replace("/", "");
            Debug.Log("FileName: " + fileName + " ComicName: " + _comicName);

            //Verifica se arquivo existe
            bool _exist = false;

            if (_ComicPages != null)
            {
                for (int y = 0; y < _ComicPages.Length; y++)
                {
                    if (_ComicPages[y].Name == fileName)
                    {
                        long _fileSize = FileBrowserHelpers.GetFilesize(_ComicPages[y].Path);
                        if (_fileSize > 30569)
                        {
                            Debug.Log("Arquivo já existe: " + _ComicPages[y].Path + "Possui kb: " + _fileSize);
                            _exist = true;
                            break;
                        }
                    }
                }
            }
            if (_exist)
            {
                continue; 
            }
            Debug.Log("Link: " + OnlinePagesURL[i]);
            Uri _url = new Uri(OnlinePagesURL[i]);
            if (Application.platform == RuntimePlatform.Android)
            {
                queue.Run(DownloadingImageAndroid(_url, i, true));
            }
            else
            {
                queue.Run(DownloadingImage(_url, i, true));
            }
        }
    }
    private IEnumerator LoadLocalImage(FileSystemEntry _fileToLoad, int index = 0)
    {
        Debug.Log("Carregando: " + _fileToLoad.Path);
        if (!FileBrowserHelpers.FileExists(_fileToLoad.Path))
        {
            yield return null;
        }
        byte[] results = FileBrowserHelpers.ReadBytesFromFile(_fileToLoad.Path);
        Texture2D _tex = new Texture2D(4,4);

        _tex.LoadImage(results) ;


        ImageData _img = new ImageData();
        _img.Name = FileBrowserHelpers.GetFilename(_fileToLoad.Name);
        _img.Sprite = Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0, 0));
        image.Add(_img);
        downloadBar.value = image.Count;
        if (sprite.sprite != image[0].Sprite)
        {
            displayThumbnail();
        }
        totalPagesText.text = "Páginas " + pagesURL.Count.ToString();
        if (pagesURL.Count == image.Count)
        {
            downloadBar.gameObject.SetActive(false);
            ShowComic();
        }
        yield return null;
    }
    IEnumerator DownloadingImage(Uri url2, int index = 0, bool download = false)
    {
        Debug.Log("Baixando " + url2);

        string fileName = Path.GetFileName(OnlinePagesURL[index]);
        string _comicName = comicName.Replace("\\", "");
        _comicName = _comicName.Replace("/", "");

        if (comicFolder == "")
        {
            comicFolder = GetComicFolder(_comicName);
        }
        string _filePath = "";

        try
        {
            _filePath = Path.Combine(comicFolder, fileName);
            Debug.Log("Criando arquivo " + _filePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + comicFolder.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        UnityWebRequest uwr = new UnityWebRequest(url2);
        uwr.method = UnityWebRequest.kHttpVerbGET;
        DownloadHandlerFile dh = new DownloadHandlerFile(_filePath, true);
        dh.removeFileOnAbort = true;
        uwr.downloadHandler = dh;

        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
            Debug.Log(uwr.error);
        else
        {
            Debug.Log("Salvo em: " + _filePath);

            //Barra de progresso
            totalPagesText.text = "Páginas " + OnlinePagesURL.Count.ToString();

            if (OnlinePagesURL.Count == DownloadSucess.Count)
            {
                InfoPopupUtil.ShowInformation("Baixou a comic[" + comicName + "] com sucesso!");
                Debug.Log("Baixou a comic[" + comicName + "] com sucesso!");
                clearMemory();
            }
            uwr.Dispose();
        }
    }

    IEnumerator DownloadingImageAndroid(Uri url2, int index = 0, bool download = false)
    {
        Debug.Log("Baixando " + url2);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url2))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Erro UnityWebRequest: " + uwr.error);
            }
            else
            {
                Debug.Log("Success" + uwr.error);
                if (download)
                {
                    //byte[] results = uwr.downloadHandler.data;
                    string fileName = Path.GetFileName(OnlinePagesURL[index]);
                    string _comicName = comicName.Replace("\\", "");
                    _comicName = _comicName.Replace("/", "");
                    //string _folder = comicPath + _comicName + "\\";
                    Debug.Log("saveImage(" + comicPath.Path + ", " + uwr.downloadHandler.data + ", " + _comicName + ", " + fileName + ");");
                    saveImage(comicPath.Path, uwr.downloadHandler.data, _comicName, fileName);  // give filename
                    DownloadSucess.Add(fileName);
                    downloadBar.value = DownloadSucess.Count;
                    uwr.Dispose();
                }
            }

        }
        totalPagesText.text = "Páginas " + OnlinePagesURL.Count.ToString();

        if (OnlinePagesURL.Count == DownloadSucess.Count)
        {
            InfoPopupUtil.ShowInformation("Baixou a comic[" + comicName + "] com sucesso!");
            Debug.Log("Baixou a comic[" + comicName + "] com sucesso!");
            clearMemory();
        }
    }


    //baixa a imagem sem salvar no HD, fica na memória
    IEnumerator StreamImage(Uri url2, int index = 0, bool download = false)
    {
        Debug.Log("Baixando " + url2);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url2))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Erro UnityWebRequest: " + uwr.error);
            }
            else
            {
                Debug.Log("Success" + uwr.error + " " + url2);
                ImageData _img = new ImageData();

                _img.Name = FileBrowserHelpers.GetFilename(OnlinePagesURL[index]);


                _img.Sprite = Sprite.Create(DownloadHandlerTexture.GetContent(uwr), new Rect(0, 0, DownloadHandlerTexture.GetContent(uwr).width, DownloadHandlerTexture.GetContent(uwr).height), new Vector2(0, 0));
                image.Add(_img);
            }
        }
        if (sprite.sprite != image[0].Sprite)
        {
            displayThumbnail();
        }
        totalPagesText.text = "Páginas " + OnlinePagesURL.Count.ToString();

        if (OnlinePagesURL.Count == image.Count)
        {
            clearMemory();
        }
    }

    void saveImage(string path, byte[] imageBytes, string _comicName = "semNome", string _fileName ="imagemSemNome.png")
    {
        FileSystemEntry _comicPath = new FileSystemEntry();
        string _newComicPath = "";
        //Create Directory if it does not exist
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(path, false);
        for (int i = 0; i < _folders.Length; i++)
        {
            if (_folders[i].Name == _comicName)
            {
                _comicPath = _folders[i];
            }
        }
        if (_comicPath.Path == null)
        {
            Debug.Log("Criando pasta em: " + path + _comicName);
            _newComicPath = FileBrowserHelpers.CreateFolderInDirectory(path, _comicName);
        }
        else
        {
            //Debug.Log(path + " does exist");
        }
        if (_newComicPath == "")
        {
            _newComicPath = _comicPath.Path;
        }
        //Debug.Log("info recebida para salvar: " + _newComicPath + ", " + _fileName);
        try
        {
            string _filePath = FileBrowserHelpers.CreateFileInDirectory(_newComicPath, _fileName);
            FileBrowserHelpers.WriteBytesToFile(_filePath, imageBytes);
            Debug.Log("Saved Data to: " + (_filePath).Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + _newComicPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
        imageBytes = null;
        clearMemory();
    }

    public string GetComicFolder(string _comicName)
    {
        FileSystemEntry _comicPath = new FileSystemEntry();
        string _newComicPath = "";
        //Create Directory if it does not exist
        FileSystemEntry[] _folders = FileBrowserHelpers.GetEntriesInDirectory(comicPath.Path, false);
        for (int i = 0; i < _folders.Length; i++)
        {
            if (_folders[i].Name == _comicName)
            {
                _comicPath = _folders[i];
            }
        }
        if (_comicPath.Path == null)
        {
            Debug.Log("Criando pasta em: " + comicPath.Path + _comicName);
            _newComicPath = FileBrowserHelpers.CreateFolderInDirectory(comicPath.Path, _comicName);
        }
        else
        {
            //Debug.Log(path + " does exist");
        }
        if (_newComicPath == "")
        {
            _newComicPath = _comicPath.Path;
        }
        return _newComicPath;
    }

    //atualiza o caminho das pastas
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

    public void displayThumbnail(Texture2D _thumbnail = null)
    {
        if (_thumbnail && sprite.sprite == null)
        {
            sprite.sprite = Sprite.Create(_thumbnail, new Rect(0, 0, _thumbnail.width, _thumbnail.height), new Vector2(0, 0));
        }
        else
        {
            sprite.sprite = image[0].Sprite;
        }
    }

    //popula OnlinePagesURL
    public void getPagesLink()
    {
        //pega o source selecionado
        getSourceClass();

        OnlinePagesURL.Clear();
        GetPostHTML();
        
    }
    public void GetPostHTML()
    {
        Uri _url = new Uri(url);
        queue.Run(DownloadPostsPages(_url));
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
                string Text = uwr.downloadHandler.text;

                rawPostData = Text;
                if (rawPostData != "")
                {
                    textSample = rawPostData;
                }
                string _textSample;
                int cycleProtection = 0;

                //o parse json começa aqui
                textSample = textSample.Substring(textSample.IndexOf(source.imageSearch.searchIndexReference) + source.imageSearch.searchIndexReference.Length);
                while (textSample.IndexOf(source.imageSearch.start) != -1 && cycleProtection < 300)
                {
                    cycleProtection++;
                    textSample = textSample.Substring(textSample.IndexOf(source.imageSearch.start) + source.imageSearch.start.Length);
                    _textSample = source.imageSearch.PrefixMissing + textSample.Substring(0, textSample.IndexOf(source.imageSearch.finish));
                    string newImageURL = _textSample;
                    if (source.imageSearch.replace != "" || source.imageSearch.replaceTo != "")
                    {
                        newImageURL = newImageURL.Replace(source.imageSearch.replace, source.imageSearch.replaceTo);
                    }
                    newImageURL = newImageURL.Substring(0, newImageURL.Length - source.imageSearch.cutLastIndex);

                    OnlinePagesURL.Add(newImageURL);
                }
            }
        }
        getThumbnailOnline();
    }
    public void CreateImagesData()
    {
        for (int i = 0; i < pagesURL.Count; i++)
        {
            Uri _url = new Uri(pagesURL[i].Path);
            queue.Run(DownloadingImage(_url));
        }
    }
    public void clearMemory()
    {
        Resources.UnloadUnusedAssets();
    }
}