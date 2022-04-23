using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Posterr.Controllers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using Xunit;

namespace Posterr.Tests.Controllers
{
    public class PostsControllerTest
    {
        #region POST [Route("create")]
        [Theory, MemberData(nameof(CreatePostTests))]
        public void CreatePostTest(CreatePostTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();

            postServiceSubstitute.CreatePost(test.Request, Arg.Any<int>()).Returns(test.CreatePostResponse);

            var controller = new PostController(postServiceSubstitute);
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
