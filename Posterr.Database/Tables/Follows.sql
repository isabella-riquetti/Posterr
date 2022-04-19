CREATE TABLE [dbo].[Follows]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FollowerId] INT NOT NULL CONSTRAINT FK_FollowerId_UserId
        FOREIGN KEY REFERENCES [Users](Id), 
    [FollowingId] INT NOT NULL CONSTRAINT FK_FollowingId_UserId
        FOREIGN KEY REFERENCES [Users](Id),
    [Unfollowed] BIT NOT NULL DEFAULT 0
)
