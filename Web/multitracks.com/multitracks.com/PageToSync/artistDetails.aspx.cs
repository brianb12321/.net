using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataAccess;
using MTDataAccess;
using MTDataAccess.Models;

public partial class artistDetails : System.Web.UI.Page
{
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
                banner.HeroImageUrl = artist.HeroUrl.ToString();
                banner.HeroImageAltText = artist.Title;
                banner.DetailsImageUrl = artist.ImageUrl.ToString();
                banner.DetailsImageAltText = artist.Title;
                banner.DetailsName = artist.Title;
                //Attempt to split the biography into two parts--main and extended--using the following delimiter
                //<!-- read more -->
                string delimiter = "<!-- read more -->";
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
            else
            {
                throw new ArgumentException("ArtistId must be a valid number.", nameof(artistIdString));
            }
        }
    }
}