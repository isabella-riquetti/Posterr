 #### Using InMemory DB
* No changes required

 #### Using localdb
In order to use a localdb you need to first create the server and database.
The expected server: **(localdb)\Strider**
The expected database name: **Posterr**
* Run on command line: `cd {YOUR_PROJECT_LOCATION}\Posterr\Posterr.DB`
* Run update database and create the tables using migrations: `dotnet ef database update`
* Comment line 35 `services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Posterr"));` in `Posterr.API/Startup.cs`
* Uncomment lines 38 and 39 in `Posterr.API/Startup.cs`

 ### Testing the API
* Rebuild the entire solution to download Nuget Packages
* Run `Posterr.API` project
* No authentication is required
  * If you want mock to be authenticated with one user that is not the first user, add a header with the name `AuthenticatedUserId` and the User Id you want to be authenticated in the value
* Test endpoints
  * Use Swagger on the API first page [https://localhost:44338/swagger/]
  * Or import the following Postman collection to test the endpoints [https://www.getpostman.com/collections/f5a6a37d55f23292d783]. Also available as `postman_collection_posterr.json` in the solution folder.

**I strongly recommend using Postman since a few test responses examples are there.**


#### Changes:
**Database**:
* The posts are going to be stored in the table `Posts` same table as the normal posts, reposts, and quote posts. This way any future features associated with the Post, such as adding images, and emoticons, could be easily applied to the reply.
* The new column `IsReply` would be added to the `Posts` table. This column would be a nullable `bit` with 0 as the default value.
* New replies would be saved with the text in the `Content`, the `OriginalPostId` with the ID of the post that is being replied to, and `IsReply` with 1 for true.
* If users are going to be able to delete, a `Deleted` flag would need to be implemented, and if they are going to be able to edit a new `UpdatedAt` property would need to be created.

**API**
* Need to create a new endpoint to grab the replies. This endpoint can be similar to the timelines but would need to return the main post and a list of replies.
* The filter for the replies would be Posts with `IsReply = true` and `OriginalPostId = {The main post ID}`
* No need to change anything in the search feature since this will be a normal register in the `Posts` table
* The reply should be handled in a separate service and controller since they are going to be loaded on a different page
* The return format would be somewhat like this:
```
{
    "postId": 1,
    "username": "username1",
    "createdAt": "4/19/2022 8:26:00 PM",
    "content": "The post content",
    "isRepost": false,
    "isRequote": false,
	"replies": [
		{
			"postId": 2,
			"username": "username2",
			"createdAt": "4/19/2022 8:30:00 PM",
			"content": "One reply"
		},
		{
			"postId": 3,
			"username": "username3",
			"createdAt": "4/19/2022 8:35:00 PM",
			"content": "One reply"
		}
	]
}

```
Every time a user posts something, this post would be saved in the timeline of every user that follows him. The posts can even be saved in a UI-friendly format, so would just grab them from Redis and send them back to the UI.
