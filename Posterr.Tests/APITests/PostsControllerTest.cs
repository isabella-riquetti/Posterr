using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Posterr.Controllers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using Xunit;

namespace Posterr.Tests
{
    public class PostsControllerTest
    {
        #region [Route("byUser/{userId}/{skip?}")]
        [Theory, MemberData(nameof(GetUserPostsTests))]
        public async void GetUserPostsTest(GetUserPostsTestInput test)
        {
            var postServiceSubstitute = Substitute.For<IPostService>();
            
            postServiceSubstitute.GetUserPosts(Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetUserPostsResponse);

            var controller = new PostController(postServiceSubstitute, null);
            IActionResult response = await controller.Get(test.UserId, test.Skip);

            if(!test.ExpectSuccess)
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

        public static TheoryData<GetUserPostsTestInput> GetUserPostsTests = new TheoryData<GetUserPostsTestInput>()
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
                TestName = "Fail, invalid skip",
                ExpectSuccess = false,
                UserId = 1,
                Skip = -5,
                ExpectedErrorMessage = "Cannot skip negative number of records"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Fail at getting posts",
                ExpectSuccess = false,
                UserId = 1,
                Skip = 0,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateError("Error"),
                ExpectedErrorMessage = "Error"
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, no posts",
                ExpectSuccess = true,
                UserId = 1,
                Skip = 0,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()),
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, with posts",
                ExpectSuccess = true,
                UserId = 1,
                Skip = 0,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>.CreateSuccess(new List<PostResponseModel>()
                {
                    new PostResponseModel(new PostModel()
                    {
                        Id = 1,
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
            public int? Skip { get; set; }
            public BaseResponse<IList<PostResponseModel>> GetUserPostsResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("byUser/{userId}/{skip?}")]
    }
}
