class LocalizedString
{
    public string Language { get; set; }
    public string Data { get; set; }

    public LocalizedString(string language, string data)
    {
        Language = language;
        Data = data;
    }
}
