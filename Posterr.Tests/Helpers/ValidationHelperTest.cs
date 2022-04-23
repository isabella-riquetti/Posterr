using Posterr.Services.Helpers;
using Posterr.Services.Model;
using System;
using Xunit;

namespace Posterr.Tests.Helpers
{
    public class ValidationHelperTest
    {
        #region IsSkipPossible
        [Theory, MemberData(nameof(IsSkipPossibleTests))]
        public void IsSkipPossibleTest(IsSkipPossibleTestInput test)
        {
            bool response = ValidationHelper.IsValuePositiveOrNeutral(test.Skip, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsSkipPossibleTestInput> IsSkipPossibleTests = new()
        {
            new IsSkipPossibleTestInput()
            {
                TestName = "Fail, skip negative",
                Skip = -5,
                ExpectedResponse = false,
                ExpectedErrorMessage = "The value cannot be negative"
            },
            new IsSkipPossibleTestInput()
            {
                TestName = "Success, skip null",
                Skip = null,
                ExpectedResponse = true
            },
            new IsSkipPossibleTestInput()
            {
                TestName = "Success, skip 0",
                Skip = 0,
                ExpectedResponse = true
            },
            new IsSkipPossibleTestInput()
            {
                TestName = "Success, skip positive",
                Skip = 7,
                ExpectedResponse = true
            },
        };
        public class IsSkipPossibleTestInput
        {
            public string TestName { get; set; }
            public int? Skip { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsSkipPossible

        #region IsValidUserId
        [Theory, MemberData(nameof(IsValidUserIdTests))]
        public void IsValidUserIdTest(IsValidUserIdTestInput test)
        {
            bool response = ValidationHelper.IsValidUserId(test.UserId, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsValidUserIdTestInput> IsValidUserIdTests = new()
        {
            new IsValidUserIdTestInput()
            {
                TestName = "Fail, negative ID",
                UserId = -5,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new IsValidUserIdTestInput()
            {
                TestName = "Fail, ID 0",
                UserId = 0,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new IsValidUserIdTestInput()
            {
                TestName = "Success, positive ID",
                UserId = 7,
                ExpectedResponse = true
            },
        };
        public class IsValidUserIdTestInput
        {
            public string TestName { get; set; }
            public int UserId { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsValidUserId

        #region IsValidUser
        [Theory, MemberData(nameof(IsValidUserTests))]
        public void IsValidUserTest(IsValidUserTestInput test)
        {
            bool response = ValidationHelper.IsValidUser(test.UserId, test.ExpectedUserExistsFuncWithResponse, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsValidUserTestInput> IsValidUserTests = new()
        {
            new IsValidUserTestInput()
            {
                TestName = "Fail, negative ID",
                UserId = -5,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new IsValidUserTestInput()
            {
                TestName = "Fail, ID 0",
                UserId = 0,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new IsValidUserTestInput()
            {
                TestName = "Fail, user does not exist",
                UserId = 0,
                ExpectedResponse = false,
                ExpectedUserExistsFuncWithResponse = (id) => BaseResponse.CreateError("User does not exist"),
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new IsValidUserTestInput()
            {
                TestName = "Success, positive ID",
                UserId = 7,
                ExpectedUserExistsFuncWithResponse = (id) => BaseResponse.CreateSuccess(),
                ExpectedResponse = true
            },
        };
        public class IsValidUserTestInput
        {
            public string TestName { get; set; }
            public int UserId { get; set; }
            public Func<int, BaseResponse> ExpectedUserExistsFuncWithResponse { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsValidUser

        #region IsValidUsername
        [Theory, MemberData(nameof(IsValidUsernameTests))]
        public void IsValidUsernameTest(IsValidUsernameTestInput test)
        {
            bool response = ValidationHelper.IsValidUsername(test.Username, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsValidUsernameTestInput> IsValidUsernameTests = new()
        {
            new IsValidUsernameTestInput()
            {
                TestName = "Fail, empty",
                Username = "",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Username should be alphanumeric and under 14 characters"
            },
            new IsValidUsernameTestInput()
            {
                TestName = "Fail, spaces",
                Username = "user name",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Username should be alphanumeric and under 14 characters"
            },
            new IsValidUsernameTestInput()
            {
                TestName = "Fail, special characters",
                Username = "user-name",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Username should be alphanumeric and under 14 characters"
            },
            new IsValidUsernameTestInput()
            {
                TestName = "Fail, over 14",
                Username = "theusernamethatistoobig",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Username should be alphanumeric and under 14 characters"
            },
            new IsValidUsernameTestInput()
            {
                TestName = "Succes, valid",
                Username = "theusernameok",
                ExpectedResponse = true
            }
        };
        public class IsValidUsernameTestInput
        {
            public string TestName { get; set; }
            public string Username { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsValidUserId

        #region IsValidContentLength
        [Theory, MemberData(nameof(IsValidContentLengthTests))]
        public void IsValidContentLengthTest(IsValidContentLengthTestInput test)
        {
            bool response = ValidationHelper.IsValidContentLength(test.Content, out string errorMessage, test.Min, test.Max);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsValidContentLengthTestInput> IsValidContentLengthTests = new()
        {
            new IsValidContentLengthTestInput()
            {
                TestName = "Fail, null",
                Content = null,
                Min = 1,
                Max = 777,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text cannot be null"
            },
            new IsValidContentLengthTestInput()
            {
                TestName = "Fail, empty",
                Content = "",
                Min = 1,
                Max = 777,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text should have between 1 and 777 characters"
            },
            new IsValidContentLengthTestInput()
            {
                TestName = "Fail, over custom max",
                Content = "Test",
                Min = 1,
                Max = 3,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text should have between 1 and 3 characters"
            },
            new IsValidContentLengthTestInput()
            {
                TestName = "Fail, over 777 characters (801)",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus. Vestibulum eu sapien maximus, dictum orci eget, tincidunt libero. Nam euismod sem neque, eu congue mi volutpat ut. Pellentesque accumsan metus vel sem congue pretium. Fusce eget facilisis magna, eget sollicitudin magna. Etiam lobortis tellus hendrerit pellentesque fringilla. Nullam tristique auctor mauris, sit amet aliquam odio congue vel. Donec volutpat tortor vitae justo luctus tempor. Phasellus laoreet ex eu ipsum suscipit vestibulum. Nullam placerat scelerisque convallis. Aenean nunc sapien, consectetur eget hendrerit pulvinar, sodales convallis dui. Ut diam turpis, molestie ut ligula sed, scelerisque rutrum massa. Aenean non tristique eros. Ut a vulputate nisi. Maecenas venenatis sed enim a condimentum.",
                Min = 1,
                Max = 777,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text should have between 1 and 777 characters"
            },
            new IsValidContentLengthTestInput()
            {
                TestName = "Succes, valid, under 777 characters",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus. Vestibulum eu sapien maximus, dictum orci eget, tincidunt libero. Nam euismod sem neque, eu congue mi volutpat ut. Pellentesque accumsan metus vel sem congue pretium. Fusce eget facilisis magna, eget sollicitudin magna. Etiam lobortis tellus hendrerit pellentesque fringilla. Nullam tristique auctor mauris, sit amet aliquam odio congue vel. Donec volutpat tortor vitae justo luctus tempor. Phasellus laoreet ex eu ipsum suscipit vestibulum. Nullam placerat scelerisque convallis. Aenean nunc sapien, consectetur eget hendrerit pulvinar, sodales convallis dui. Ut diam turpis, molestie ut ligula sed, scelerisque rutrum massa. Aenean non tristique eros. Ut a vulputate nisi. Maecenas venenatis",
                Min = 1,
                Max = 777,
                ExpectedResponse = true
            },
            new IsValidContentLengthTestInput()
            {
                TestName = "Succes, valid, over 0 characters",
                Min = 0,
                Max = 777,
                Content = "",
                ExpectedResponse = true
            }
        };
        public class IsValidContentLengthTestInput
        {
            public string TestName { get; set; }
            public string Content { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public int Min { get; internal set; }
            public int Max { get; internal set; }
        }
        #endregion IsValidUserId

        #region IsValidPostRequest
        [Theory, MemberData(nameof(IsValidPostRequestTests))]
        public void IsValidPostRequestTest(IsValidPostRequestTestInput test)
        {
            bool response = ValidationHelper.IsValidPostRequest(test.Request, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static readonly TheoryData<IsValidPostRequestTestInput> IsValidPostRequestTests = new()
        {
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, null",
                Request = null,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Request cannot be null"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, empty",
                Request = new CreatePostRequestModel(),
                ExpectedResponse = false,
                ExpectedErrorMessage = "Post must have a content or be a repost"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, empty content",
                Request = new CreatePostRequestModel()
                {
                    Content = ""
                },
                ExpectedResponse = false,
                ExpectedErrorMessage = "Post must have a content or be a repost"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, original negative",
                Request = new CreatePostRequestModel()
                {
                    OriginalPostId = 0
                },
                ExpectedResponse = false,
                ExpectedErrorMessage = "OriginalPostId must be positive"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, quote post empty content",
                Request = new CreatePostRequestModel()
                {
                    OriginalPostId = 1,
                    Content = ""
                },
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text should have between 1 and 777 characters"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Fail, over 777 characters (801)",
                Request = new CreatePostRequestModel()
                {
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus. Vestibulum eu sapien maximus, dictum orci eget, tincidunt libero. Nam euismod sem neque, eu congue mi volutpat ut. Pellentesque accumsan metus vel sem congue pretium. Fusce eget facilisis magna, eget sollicitudin magna. Etiam lobortis tellus hendrerit pellentesque fringilla. Nullam tristique auctor mauris, sit amet aliquam odio congue vel. Donec volutpat tortor vitae justo luctus tempor. Phasellus laoreet ex eu ipsum suscipit vestibulum. Nullam placerat scelerisque convallis. Aenean nunc sapien, consectetur eget hendrerit pulvinar, sodales convallis dui. Ut diam turpis, molestie ut ligula sed, scelerisque rutrum massa. Aenean non tristique eros. Ut a vulputate nisi. Maecenas venenatis sed enim a condimentum."
                },
                ExpectedResponse = false,
                ExpectedErrorMessage = "Text should have between 0 and 777 characters"
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Succes, valid, under 777 characters",
                Request = new CreatePostRequestModel()
                {
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus."
                },
                ExpectedResponse = true
            },
            new IsValidPostRequestTestInput()
            {
                TestName = "Succes, content empty, but with original post id",
                Request = new CreatePostRequestModel()
                {
                    OriginalPostId = 1
                },
                ExpectedResponse = true
            }
        };
        public class IsValidPostRequestTestInput
        {
            public string TestName { get; set; }
            public CreatePostRequestModel Request { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsValidUserId
    }
}
