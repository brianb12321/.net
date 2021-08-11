CREATE PROCEDURE [dbo].[GetSongsByArtistId]
	@artistId INT
AS
	SELECT *
	FROM Song
	WHERE Song.artistID = @artistId