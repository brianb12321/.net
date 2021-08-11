CREATE PROCEDURE [dbo].[GetAlbumsByArtistId]
	@artistId INT
AS
	SELECT Album.*
	FROM Album
	WHERE Album.artistID = @artistID