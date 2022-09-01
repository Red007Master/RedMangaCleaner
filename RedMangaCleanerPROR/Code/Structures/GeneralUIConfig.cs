public class GeneralUIConfig
{
    public bool Debug { get; set; }
    public bool IsFirstStart { get; set; }
    public bool UserAgreementIsAccepted { get; set; }
    public bool ShowUpdateNotification { get; set; }
    public bool ShowNewsNotification { get; set; }
    public bool ShowPromotion { get; set; }

    public void SetDefault()
    {
        Debug = true;
        IsFirstStart = true;
        UserAgreementIsAccepted = false;
        ShowUpdateNotification = true;
        ShowNewsNotification = true;
        ShowPromotion = true;
    }
}