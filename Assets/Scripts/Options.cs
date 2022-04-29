using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using TMPro;

public class Options : MonoBehaviour
{

    public string folder;
    public string website;

    public TMP_InputField folderText;

    // Start is called before the first frame update
    void Start()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".jpg");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        if (folder == "")
        {
            if (PlayerPrefs.GetString("defaultFolder") == null)
            {
                PlayerPrefs.SetString("defaultFolder", Application.persistentDataPath);
                Debug.Log("Defalt Folder salvo em: " + Application.persistentDataPath);
            }
            folder = PlayerPrefs.GetString("defaultFolder");
            folderText.text = folder;
        }
        if (website == "")
        {
            if (PlayerPrefs.GetString("defaultWebsite") == null)
            {
                PlayerPrefs.SetString("defaultWebsite", "https://yiffbrasil.com.br/wp-json/wp/v2/posts/");
                Debug.Log("Defalt Website salvo como: " + "https://yiffbrasil.com.br/wp-json/wp/v2/posts/");
            }
            folder = PlayerPrefs.GetString("defaultFolder");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveFolder()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void changeDownloadLimit(int info)
    {
        PlayerPrefs.SetInt("defaultDownloads", info);
    }

    public void newFolder(string folderPath)
    {
        PlayerPrefs.SetString("defaultFolder", folderPath);
        folder = PlayerPrefs.GetString("defaultFolder");
        
        if (folderText)
        {
            folderText.text = folder;
        }
    }

    public void clearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, true, null, null, "Load Folders", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        

        if (FileBrowser.Success)
        {
            Debug.Log(FileBrowser.Result[0]);
            newFolder(FileBrowser.Result[0]);
        }
    }
}
