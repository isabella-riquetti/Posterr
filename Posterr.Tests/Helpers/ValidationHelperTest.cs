using Posterr.Services.Helpers;
using Posterr.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Posterr.Tests.Helpers
{
    public class ValidationHelperTest
    {
        #region IsSkipPossible
        [Theory, MemberData(nameof(IsSkipPossibleTests))]
        public void IsSkipPossibleTest(IsSkipPossibleTestInput test)
        {
            bool response = ValidationHelper.IsSkipPagePossible(test.Skip, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static TheoryData<IsSkipPossibleTestInput> IsSkipPossibleTests = new TheoryData<IsSkipPossibleTestInput>()
        {
            new IsSkipPossibleTestInput()
            {
                TestName = "Fail, skip negative",
                Skip = -5,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Cannot skip negative number of records"
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

        public static TheoryData<IsValidUserIdTestInput> IsValidUserIdTests = new TheoryData<IsValidUserIdTestInput>()
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

        public static TheoryData<IsValidUserTestInput> IsValidUserTests = new TheoryData<IsValidUserTestInput>()
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

        public static TheoryData<IsValidUsernameTestInput> IsValidUsernameTests = new TheoryData<IsValidUsernameTestInput>()
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

        #region IsValidContent
        [Theory, MemberData(nameof(IsValidContentTests))]
        public void IsValidContentTest(IsValidContentTestInput test)
        {
            bool response = ValidationHelper.IsValidPostContent(test.Content, out string errorMessage);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedErrorMessage, errorMessage);
        }

        public static TheoryData<IsValidContentTestInput> IsValidContentTests = new TheoryData<IsValidContentTestInput>()
        {
            new IsValidContentTestInput()
            {
                TestName = "Fail, null",
                Content = null,
                ExpectedResponse = false,
                ExpectedErrorMessage = "Post content cannot be empty and should be under 777 characters"
            },
            new IsValidContentTestInput()
            {
                TestName = "Fail, empty",
                Content = "",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Post content cannot be empty and should be under 777 characters"
            },
            new IsValidContentTestInput()
            {
                TestName = "Fail, over 777 characters (801)",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus. Vestibulum eu sapien maximus, dictum orci eget, tincidunt libero. Nam euismod sem neque, eu congue mi volutpat ut. Pellentesque accumsan metus vel sem congue pretium. Fusce eget facilisis magna, eget sollicitudin magna. Etiam lobortis tellus hendrerit pellentesque fringilla. Nullam tristique auctor mauris, sit amet aliquam odio congue vel. Donec volutpat tortor vitae justo luctus tempor. Phasellus laoreet ex eu ipsum suscipit vestibulum. Nullam placerat scelerisque convallis. Aenean nunc sapien, consectetur eget hendrerit pulvinar, sodales convallis dui. Ut diam turpis, molestie ut ligula sed, scelerisque rutrum massa. Aenean non tristique eros. Ut a vulputate nisi. Maecenas venenatis sed enim a condimentum.",
                ExpectedResponse = false,
                ExpectedErrorMessage = "Post content cannot be empty and should be under 777 characters"
            },
            new IsValidContentTestInput()
            {
                TestName = "Succes, valid, under 777 characters",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla non convallis lacus. Vestibulum eu sapien maximus, dictum orci eget, tincidunt libero. Nam euismod sem neque, eu congue mi volutpat ut. Pellentesque accumsan metus vel sem congue pretium. Fusce eget facilisis magna, eget sollicitudin magna. Etiam lobortis tellus hendrerit pellentesque fringilla. Nullam tristique auctor mauris, sit amet aliquam odio congue vel. Donec volutpat tortor vitae justo luctus tempor. Phasellus laoreet ex eu ipsum suscipit vestibulum. Nullam placerat scelerisque convallis. Aenean nunc sapien, consectetur eget hendrerit pulvinar, sodales convallis dui. Ut diam turpis, molestie ut ligula sed, scelerisque rutrum massa. Aenean non tristique eros. Ut a vulputate nisi. Maecenas venenatis",
                ExpectedResponse = true
            }
        };
        public class IsValidContentTestInput
        {
            public string TestName { get; set; }
            public string Content { get; set; }
            public bool ExpectedResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion IsValidUserId
    }
}
