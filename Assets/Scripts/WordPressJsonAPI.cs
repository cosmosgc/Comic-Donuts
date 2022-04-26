using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordPressJsonAPI : MonoBehaviour
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Guid
    {
        public string rendered { get; set; }
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

    public class Excerpt
    {
        public string rendered { get; set; }
        public bool @protected { get; set; }
    }

    public class Meta
    {
        public bool _mi_skip_tracking { get; set; }
    }

    public class Styles
    {
        public string desktop { get; set; }
        public string tablet { get; set; }
        public string mobile { get; set; }
    }

    public class StylesDescriptor
    {
        public Styles styles { get; set; }
        public List<object> google_fonts { get; set; }
        public int version { get; set; }
    }

    public class BlocksyMeta
    {
        public StylesDescriptor styles_descriptor { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

    public class About
    {
        public string href { get; set; }
    }

    public class Author
    {
        public bool embeddable { get; set; }
        public string href { get; set; }
    }

    public class Reply
    {
        public bool embeddable { get; set; }
        public string href { get; set; }
    }

    public class VersionHistory
    {
        public int count { get; set; }
        public string href { get; set; }
    }

    public class WpFeaturedmedia
    {
        public bool embeddable { get; set; }
        public string href { get; set; }
    }

    public class WpAttachment
    {
        public string href { get; set; }
    }

    public class WpTerm
    {
        public string taxonomy { get; set; }
        public bool embeddable { get; set; }
        public string href { get; set; }
    }

    public class Cury
    {
        public string name { get; set; }
        public string href { get; set; }
        public bool templated { get; set; }
    }

    public class Links
    {
        public List<Self> self { get; set; }
        public List<Collection> collection { get; set; }
        public List<About> about { get; set; }
        public List<Author> author { get; set; }
        public List<Reply> replies { get; set; }

        public List<VersionHistory> VersionHistory { get; set; }

        public List<WpFeaturedmedia> WpFeaturedmedia { get; set; }

        public List<WpAttachment> WpAttachment { get; set; }

        public List<WpTerm> WpTerm { get; set; }
        public List<Cury> curies { get; set; }
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
        public Excerpt excerpt { get; set; }
        public int author { get; set; }
        public int featured_media { get; set; }
        public string comment_status { get; set; }
        public string ping_status { get; set; }
        public bool sticky { get; set; }
        public string template { get; set; }
        public string format { get; set; }
        public Meta meta { get; set; }
        public List<int> categories { get; set; }
        public List<int> tags { get; set; }
        public BlocksyMeta blocksy_meta { get; set; }
        public Links _links { get; set; }
    }


}
