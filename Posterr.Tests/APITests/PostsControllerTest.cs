using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Posterr.Controllers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using Xunit;

namespace Posterr.Tests.Controllers
{
    public class PostsControllerTest
    {
        #region [Route("timeline/following/{skipPages?}")]
        [Theory, MemberData(nameof(GetUserTimelineTests))]
        public void GetUserTimelineTest(GetUserTimelineTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            var userServiceSubstitute = Substitute.For<IUserService>();

            postServiceSubstitute.GetUserFollowingTimeline(Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetUserFollowingTimelineResponse);

            var controller = new PostController(postServiceSubstitute, userServiceSubstitute);
            IActionResult response = controller.GetUserTimeline(test.Skip);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.GetUserFollowingTimelineResponse.Data, ((OkObjectResult)response).Value);
            }
        }

        public static readonly TheoryData<GetUserTimelineTestInput> GetUserTimelineTests = new()
        {
            new GetUserTimelineTestInput()
            {
                TestName = "Fail, invalid skip",
                ExpectSuccess = false,
                Skip = -5,
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new GetUserTimelineTestInput()
            {
                TestName = "Fail, at getting",
                ExpectSuccess = false,
                Skip = 0,
                GetUserFollowingTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateError("Error"),
                ExpectedErrorMessage = "Error"
            },
            new GetUserTimelineTestInput()
            {
                TestName = "Success, with posts",
                ExpectSuccess = true,
                GetUserFollowingTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()
                {
                    new PostResponseModel(new PostsModel()
                    {
                        PostId = 1,
                        Username = "test",
                        Content = "test",
                        CreatedAt = DateTime.Now.ToString(),
                        OriginalPost = null
                    })
                }),
            },
        };
        public class GetUserTimelineTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int Skip { get; set; }
            public BaseResponse<IList<PostResponseModel>> GetUserFollowingTimelineResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("timeline/following/{skipPages?}")]

        #region [Route("timeline/{skipPages?}")]
        [Theory, MemberData(nameof(GetTimelineTests))]
        public void GetTimelineTest(GetTimelineTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            var userServiceSubstitute = Substitute.For<IUserService>();

            postServiceSubstitute.GetTimeline(Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetTimelineResponse);

            var controller = new PostController(postServiceSubstitute, userServiceSubstitute);
            IActionResult response = controller.GetTimeline(test.Skip);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.GetTimelineResponse.Data, ((OkObjectResult)response).Value);
            }
        }

        public static readonly TheoryData<GetTimelineTestInput> GetTimelineTests = new()
        {
            new GetTimelineTestInput()
            {
                TestName = "Fail, invalid skip",
                ExpectSuccess = false,
                Skip = -5,
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new GetTimelineTestInput()
            {
                TestName = "Fail, at getting",
                ExpectSuccess = false,
                Skip = 0,
                GetTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateError("Error"),
                ExpectedErrorMessage = "Error"
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, with posts",
                ExpectSuccess = true,
                GetTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()
                {
                    new PostResponseModel(new PostsModel()
                    {
                        PostId = 1,
                        Username = "test2",
                        Content = "test",
                        CreatedAt = DateTime.Now.ToString(),
                        OriginalPost = null
                    })
                }),
            },
        };
        public class GetTimelineTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int Skip { get; set; }
            public BaseResponse<IList<PostResponseModel>> GetTimelineResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("timeline/{skipPages?}")]

        #region [Route("search/{text}/{skipPages?}")]
        [Theory, MemberData(nameof(SearchTests))]
        public void SearchTest(SearchTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            var userServiceSubstitute = Substitute.For<IUserService>();

            postServiceSubstitute.SearchByText(Arg.Is(test.SearchText), Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetTimelineResponse);

            var controller = new PostController(postServiceSubstitute, userServiceSubstitute);
            IActionResult response = controller.Search(test.SearchText, test.Skip);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.GetTimelineResponse.Data, ((OkObjectResult)response).Value);
            }
        }

        public static readonly TheoryData<SearchTestInput> SearchTests = new()
        {
            new SearchTestInput()
            {
                TestName = "Fail, invalid skip",
                ExpectSuccess = false,
                Skip = -5,
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new SearchTestInput()
            {
                TestName = "Fail, empty search",
                ExpectSuccess = false,
                Skip = -5,
                SearchText = "",
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new SearchTestInput()
            {
                TestName = "Fail, at getting posts",
                ExpectSuccess = false,
                Skip = 0,
                SearchText = "Test",
                GetTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateError("Error"),
                ExpectedErrorMessage = "Error"
            },
            new SearchTestInput()
            {
                TestName = "Success, with posts",
                ExpectSuccess = true,
                Skip = 0,
                SearchText = "test",
                GetTimelineResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()
                {
                    new PostResponseModel(new PostsModel()
                    {
                        PostId = 1,
                        Username = "test2",
                        Content = "test",
                        CreatedAt = DateTime.Now.ToString(),
                        OriginalPost = null
                    })
                }),
            },
        };
        public class SearchTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int Skip { get; set; }
            public string SearchText { get; set; }
            public BaseResponse<IList<PostResponseModel>> GetTimelineResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("search/{text}/{skipPages?}")]

        #region [Route("byUser/{userId}/{skipPages?}")]
        [Theory, MemberData(nameof(GetUserPostsTests))]
        public void GetUserPostsTest(GetUserPostsTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            var userServiceSubstitute = Substitute.For<IUserService>();

            userServiceSubstitute.UserExists(Arg.Any<int>()).Returns(test.UserExistExpectedResponse);
            postServiceSubstitute.GetUserPosts(Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetUserPostsResponse);

            var controller = new PostController(postServiceSubstitute, userServiceSubstitute);
            IActionResult response = controller.GetUserPosts(test.UserId, test.Skip);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.GetUserPostsResponse.Data, ((OkObjectResult)response).Value);
            }
        }

        public static readonly TheoryData<GetUserPostsTestInput> GetUserPostsTests = new()
        {
            new GetUserPostsTestInput()
            {
                TestName = "Fail, invalid user id",
                ExpectSuccess = false,
                UserId = -1,
                Skip = 0,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Fail, invalid user id",
                ExpectSuccess = false,
                UserId = 0,
                Skip = 0,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Fail, user does not exist",
                ExpectSuccess = false,
                UserId = 10,
                Skip = 0,
                UserExistExpectedResponse = BaseResponse.CreateError("User not found"),
                ExpectedErrorMessage = "User not found"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Fail, invalid skip",
                ExpectSuccess = false,
                UserId = 1,
                Skip = -5,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Fail at getting posts",
                ExpectSuccess = false,
                UserId = 1,
                Skip = 0,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateError("Error"),
                ExpectedErrorMessage = "Error"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, no posts",
                ExpectSuccess = true,
                UserId = 1,
                Skip = 0,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()),
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, with posts",
                ExpectSuccess = true,
                UserId = 1,
                Skip = 0,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()
                {
                    new PostResponseModel(new PostsModel()
                    {
                        PostId = 1,
                        Username = "test",
                        Content = "test",
                        CreatedAt = DateTime.Now.ToString(),
                        OriginalPost = null
                    })
                }),
            },
        };
        public class GetUserPostsTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public int Skip { get; set; }
            public BaseResponse<IList<PostResponseModel>> GetUserPostsResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public BaseResponse UserExistExpectedResponse { get; set; }
        }
        #endregion [Route("byUser/{userId}/{skipPages?}")]

        #region POST [Route("create")]
        [Theory, MemberData(nameof(CreatePostTests))]
        public void CreatePostTest(CreatePostTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            var userServiceSubstitute = Substitute.For<IUserService>();

            postServiceSubstitute.CreatePost(test.Request, Arg.Any<int>()).Returns(test.CreatePostResponse);

            var controller = new PostController(postServiceSubstitute, userServiceSubstitute);
            IActionResult response = controller.CreatePost(test.Request);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.CreatePostResponse.Data, ((OkObjectResult)response).Value);
            }
        }

        public static readonly TheoryData<CreatePostTestInput> CreatePostTests = new()
        {
            new CreatePostTestInput()
            {
                TestName = "Fail, invalid content",
                ExpectSuccess = false,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    Content = ""
                },
                ExpectedErrorMessage = "Post must have a content or be a repost"
            },
            new CreatePostTestInput()
            {
                TestName = "Success, create post",
                ExpectSuccess = true,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    Content = "Test Content"
                },
                CreatePostResponse = BaseResponse<PostResponseModel>
                    .CreateSuccess(
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Test Content",
                            CreatedAt = new DateTime(2022,4,19,19,00,00).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }),
            },
            new CreatePostTestInput()
            {
                TestName = "Success, create quotepost",
                ExpectSuccess = true,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    OriginalPostId = 1,
                    Content = "Test Content"
                },
                CreatePostResponse = BaseResponse<PostResponseModel>
                    .CreateSuccess(
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Test Content 2",
                            CreatedAt = new DateTime(2022,4,19,19,00,00).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername2"
                            }
                        }),
            },
        };
        public class CreatePostTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public CreatePostRequestModel Request { get; set; }
            public BaseResponse<PostResponseModel> CreatePostResponse { get; set; }
        }
        #endregion POST [Route("create")]
    }
}
