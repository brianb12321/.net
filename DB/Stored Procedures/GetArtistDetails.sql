CREATE PROCEDURE [dbo].[GetArtistDetails]
	@artistId INT
AS
	/*General select, you would want to create other stored procedures for filtering*/
	SELECT *
	FROM Artist
	WHERE Artist.artistID = @artistId