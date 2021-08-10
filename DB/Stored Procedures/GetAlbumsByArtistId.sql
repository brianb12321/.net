CREATE PROCEDURE [dbo].[GetAlbumsByArtistId]
	@artistId INT
AS
	SELECT Album.*
	FROM Album INNER JOIN Artist ON Album.artistID = Artist.artistID
	WHERE Album.artistID = @artistID