using System;

public partial class PageToSync_UserControls_Banner : System.Web.UI.UserControl
{
    public string HeroImageUrl { get; set; }
    public string DetailsImageUrl { get; set; }
    public string HeroImageAltText { get; set; }
    public string DetailsImageAltText { get; set; }
    public string DetailsName { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        heroBannerImage.Src = HeroImageUrl;
        heroBannerImage.Alt = HeroImageAltText;
        detailsBannerImage.Src = DetailsImageUrl;
        detailsBannerImage.Alt = DetailsImageAltText;
        detailsName.InnerHtml = DetailsName;
    }
}