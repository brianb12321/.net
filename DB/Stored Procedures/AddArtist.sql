CREATE PROCEDURE [dbo].[AddArtist]
	@dateCreation SMALLDATETIME,
	@title VARCHAR(100),
	@biography VARCHAR(MAX),
	@imageURL VARCHAR(500),
	@heroURL VARCHAR(500),
	@output BIT = 1
AS
BEGIN
IF @output = 1
	INSERT INTO Artist OUTPUT INSERTED.* VALUES(@dateCreation, @title, @biography, @imageURL, @heroURL); 
ELSE
	INSERT INTO Artist VALUES(@dateCreation, @title, @biography, @imageURL, @heroURL); 
END