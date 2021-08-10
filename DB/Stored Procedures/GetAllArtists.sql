CREATE PROCEDURE [dbo].[GetAllArtists]
AS
	/*General select, you would want to create other stored procedures for filtering*/
	SELECT *
	FROM Artist