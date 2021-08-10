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
        IDataAccess sql = new SQLDecorator(new SQL());
        Artist artist = sql.GetArtistById(5);
        debug.InnerText = artist.Title;
    }
}