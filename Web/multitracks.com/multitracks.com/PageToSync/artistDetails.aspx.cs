using System;
using DataAccess;
using MTDataAccess;
using MTDataAccess.Models;

public partial class artistDetails : System.Web.UI.Page
{
    public string ArtistName { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string artistIdString = Request.QueryString["artistId"] ?? "1";
            int artistId;
            if (int.TryParse(artistIdString, out artistId))
            {
                IDataAccess dataAccess = new SQLDecorator(new SQL());
                Artist artist = dataAccess.GetArtistById(artistId);
                ArtistName = artist.Title;
                setupBanner(artist);
                setupDisplayedSongsList(artist);
                setupAlbumsList(artist);
                setupBiography(artist);
            }
            else
            {
                throw new ArgumentException("ArtistId must be a valid number.", nameof(artistIdString));
            }
        }
    }

    private void setupBanner(Artist artist)
    {
        banner.HeroImageUrl = artist.HeroUrl.ToString();
        banner.HeroImageAltText = artist.Title;
        banner.DetailsImageUrl = artist.ImageUrl.ToString();
        banner.DetailsImageAltText = artist.Title;
        banner.DetailsName = artist.Title;
    }

    private void setupAlbumsList(Artist artist)
    {
        albumRepeater.DataSource = artist.Albums;
        albumRepeater.DataBind();
    }

    private void setupDisplayedSongsList(Artist artist)
    {
        songListRepeater.DataSource = artist.Songs;
        songListRepeater.DataBind();
    }

    private void setupBiography(Artist artist)
    {
        //Attempt to split the biography into two parts--main and extended--using the following delimiter
        //<!-- read more -->
        string delimiter = "<!-- read more -->";
        //TODO: Determine if delimiter should be treated as culture insensitive. Since the biography could be translated, I left it as culturally sensitive.
        int delimiterIndex = artist.Biography.IndexOf(delimiter, StringComparison.CurrentCulture);
        //If delimiterIndex = -1, no need to split the string.
        if (delimiterIndex != -1)
        {
            string biographyMain = artist.Biography.Substring(0, delimiterIndex);
            string biographyExtended =
                artist.Biography.Substring(delimiterIndex + delimiter.Length);
            //It would be pretty pointless to have a read-more button when there isn't anything to show.
            if (string.IsNullOrWhiteSpace(biographyExtended))
            {
                readMoreLink.Style["display"] = "none";
            }
            biographyMainParagraph.InnerText = biographyMain;
            biographyExtendedParagraph.InnerText = biographyExtended;
        }
        else
        {
            biographyMainParagraph.InnerText = artist.Biography;
            readMoreLink.Style["display"] = "none";
        }
    }
}