CREATE PROCEDURE [dbo].[GetArtistDetails]
	@artistId INT,
	@includeAlbum BIT = 0,
	@includeSong BIT = 0
AS
	/*General select, you would want to create other stored procedures for filtering*/
	SELECT *
	FROM Artist
	WHERE Artist.artistID = @artistId;
	IF @includeAlbum = 1
		SELECT *
		FROM Album
		WHERE Album.artistID = @artistId;
	IF @includeSong = 1
		SELECT *
		FROM Song
		WHERE SOng.artistID = @artistId;