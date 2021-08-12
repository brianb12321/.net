CREATE PROCEDURE [dbo].[GetAllSongs]
	@pageNumber INT = 1,
	@pageSize INT = 999999999
AS
	SELECT *
	FROM Song
	ORDER BY Song.songID OFFSET @pageSize * (@pageNumber - 1) ROWS
	FETCH NEXT @pageSize ROWS ONLY