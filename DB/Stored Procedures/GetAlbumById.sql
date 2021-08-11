CREATE PROCEDURE [dbo].[GetAlbumById]
	@albumId INT
AS
	SELECT *
	FROM Album
	WHERE albumID = @albumId
