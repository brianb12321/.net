CREATE PROCEDURE [dbo].[GetArtistDetailsByName]
	@artistName VARCHAR(100),
	@exact BIT = 0
AS
IF @exact = 1
	SELECT *
	FROM Artist
	WHERE title = @artistName
ELSE
	SELECT *
	FROM Artist
	WHERE title LIKE @artistName