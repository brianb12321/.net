CREATE PROCEDURE [dbo].[GetSongByArtistId]
	@artistId INT
AS
	SELECT *
	FROM Song
	WHERE Song.artistID = @artistId
