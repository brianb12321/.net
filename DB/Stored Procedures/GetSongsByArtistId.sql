CREATE PROCEDURE [dbo].[GetSongsByArtistId]
	@artistId INT,
	@includeArtist BIT = 0,
	@includeAlbum BIT = 0
AS
	SELECT *
	FROM Song
	WHERE Song.artistID = @artistId
	IF @includeArtist = 1
		SELECT *
		FROM Artist
		WHERE Artist.artistID = @artistId
	IF @includeAlbum = 1
		SELECT *
		FROM Album
		WHERE Album.artistID = @artistId