using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceClass 
{
    [Serializable]
    public class PostParse
    {
        [SerializeField] public string TextToFind;
        [SerializeField] public string TextToFindFinish;
        [SerializeField] public string searchIndexReference;
        [SerializeField] public string PrefixMissing;
        [SerializeField] public string SuffixMissing;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
        [SerializeField] public int cutLastIndex;
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
        [SerializeField] public string searchIndexReference;
        [SerializeField] public string start;
        [SerializeField] public string finish;
        [SerializeField] public string replace;
        [SerializeField] public string replaceTo;
        [SerializeField] public string PrefixMissing;
        [SerializeField] public string SuffixMissing;
        [SerializeField] public int cutLastIndex;
    }
    [Serializable]
    public class source
    {
        [SerializeField] public string PostsURL;
        [SerializeField] public string PageSyntax;
        [SerializeField] public string PerPageSyntax;
        [SerializeField] public int PageNumber;
        [SerializeField] public int PerPage;
        [SerializeField] public string SuffixSyntax;
        [SerializeField] public string Options;

        [SerializeField] public List<SearchIndex> searchs;
        [SerializeField] public imageSearch imageSearch;
        [SerializeField] public PostParse postParser;
    }
}
