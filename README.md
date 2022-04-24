Isabella Riquetti Emidio

# Coding and Running the API
    Phase 1, coding
    Estimated time: 7 hours
    
    - Build out a RESTful API and corresponding backend system to handle the features detailed above. This RESTful API would communicate with a single-page JS app. This API you build should enable all the features on both of the pages.
    - You should implement a real, production-ready database, and queries should be performant.
    - Do not implement additional features beyond what is explained in the overview.
    - Write some automated tests for this project.
    - Do not build a front-end.
 ## Running the project
 **Required .NET Version:5.0**
 
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


# Planning
    Phase 2, planning
    Estimated time: 30 minutes
    
    The Product Manager wants to implement a new feature called "reply-to-post" (it's a lot like Twitter's These are regular posts that use "@ mentioning" at the beginning to indicate that it is a reply directly to a post. Reply posts should only be shown in a new, secondary feed on the user profile called "Posts and Replies" where all original posts and reply posts are shown. They should not be shown in the homepage feed.
    
    What you need to do:
    - Write down questions you have for the Product Manager about implementation.
    - Write about how you would solve this problem in as much detail as possible. Write about all of the changes to database/front-end/api/etc that you expect. You should write down any assumptions you are making from any questions for the Product Manager that you previously mentioned.
    - **Be thorough!**

    This should be added as a section called "Planning" in the README.
    
#### Questions for the PM:
* Will the user be able to reply to a reply, thus creating a cascade conversation?
* Will the user be able to reply to users that they don't follow, nor are followed by?
* Will users be able to edit or delete replies?
* Will users be able to reply with a repost or quote post?
* Can users reply to a quote post?
* Can users reply to a repost? If they can, the reply would be associated with the repost or the original post?
* Users can mention more than one user in a reply?
* Since the mention is going to get implemented, this will be available in posts and quote posts as well?

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

**Frontend**
Does not apply for the assessment

# Critique
    ## Phase 3, self-critique & scaling
    Estimated time: 30 minutes
    
    In any project, it is always a challenge to get the code perfectly how you'd want it. Here is what you need to do for this section:
    
    - Reflect on this project, and write what you would improve if you had more time.
    - Write about scaling. If this project were to grow and have many users and posts, which parts do you think would fail first? In a real-life situation, what steps would you take to scale this product? What other types of technology and infrastructure might you need to use?
    - **Be thorough!**
    
    This should be added as a section called "Critique" in the README.

### First thing that would fail
Probably the first thing that would fail once the number of users and posts increased, would be the timeline of followed users posts.
This is an extremely consuming query that needs to go in the `Follows` table, join with the `Users` table to grab the username, and of course, join with the `Posts` table. And since we have quote posts and reposts, we may need to go to the `Posts` table again to grab the original post content and go to the `Users` table again to grab the original post username.

**Possible solution:**

One good solution for this problem would be to use an in-memory data storage such as `Redis`, so the timeline of followed users would be precompiled.
Every time a user posts something, this post would be saved in the timeline of every user that follows him. The posts can even be saved in a UI-friendly format, so would just grab them from Redis and send them back to the UI.

### Database changes I would make
##### Repost and Quote post flags
Small issue, the posts are not flagged by type, so we need to check if the post is basic, a repost, or a quote post by checking which properties that have value. This not only increases by a small amount the API workload but also is hard to read in the DB and not easily extended for future functionalities.

##### IDs type
The IDs were created as `int` in the distant future this could be an issue if the platform reaches more than 2 billion posts, so I would change it to at least a `bigint`.

### API changes I would make
##### Unit of work
At first, I implemented a basic Context, without Unit Work, so currently is not possible to make changes in more than one repository and save the changes in a single transaction.

#### New API
I create a new API just for the timeline endpoints, since these endpoints would require a lot more processing, this could be in a different more potent server. Even using some In-Memory Cache DB I would still place it in a different server.

##### Mediator
I wish I had used a Mediator in the Controllers to avoid having many services there, so I would refactor the code to use a Mediator to call the functionalities.

### Tests changes I would make
#### Split into more projects
I think one test project for each project would be better for an app that can grow a lot. Everything into one project could make it hard to keep organized and harder to run just what's required.
