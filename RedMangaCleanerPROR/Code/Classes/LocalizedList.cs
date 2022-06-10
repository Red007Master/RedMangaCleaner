using System.Collections.Generic;

class LocalizedList
{
    public string Language { get; set; }
    public List<string> Data { get; set; }

    public LocalizedList(string language, List<string> data)
    {
        Language = language;
        Data = data;
    }
}
