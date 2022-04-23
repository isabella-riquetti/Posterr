using FluentAssertions;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Repository;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Posterr.Tests.Repository
{
    public class UserRepositoryTest
    {
        #region GetUser
        [Theory, MemberData(nameof(GetUserTests))]
        public void GetUserTest(GetUserTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new UserRepository(apiContext);
            IQueryable<User> response = repository.GetUser(test.UserIdToSearch);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static readonly TheoryData<GetUserTestInput> GetUserTests = new()
        {
            new GetUserTestInput()
            {
                TestName = "User not found",
                UserIdToSearch = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedResponse = new List<User>().AsQueryable()
            },
            new GetUserTestInput()
            {
                TestName = "User found",
                UserIdToSearch = 1,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                }.AsQueryable()
            },
        };
        public class GetUserTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int UserIdToSearch { get; set; }
            public IQueryable<User> ExpectedResponse { get; set; }
        }
        #endregion GetUser

        #region UserExists
        [Theory, MemberData(nameof(UserExistsTests))]
        public void UserExistsTest(UserExistsTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new UserRepository(apiContext);
            bool response = repository.UserExists(test.UserIdToSearch);

            Assert.Equal(test.ExpectedResponse, response);
        }

        public static readonly TheoryData<UserExistsTestInput> UserExistsTests = new()
        {
            new UserExistsTestInput()
            {
                TestName = "User not found",
                UserIdToSearch = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedResponse = false
            },
            new UserExistsTestInput()
            {
                TestName = "User found",
                UserIdToSearch = 1,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedResponse = true
            },
        };
        public class UserExistsTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int UserIdToSearch { get; set; }
            public bool ExpectedResponse { get; set; }
        }
        #endregion UserExists

        #region UserExistsOutUserId
        [Theory, MemberData(nameof(UserExistsOutUserIdTests))]
        public void UserExistsOutUserIdTest(UserExistsOutUserIdTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new UserRepository(apiContext);
            bool response = repository.UserExists(test.UsernameToSearch, out int? userId);

            Assert.Equal(test.ExpectedResponse, response);
            Assert.Equal(test.ExpectedUserId, userId);
        }

        public static readonly TheoryData<UserExistsOutUserIdTestInput> UserExistsOutUserIdTests = new()
        {
            new UserExistsOutUserIdTestInput()
            {
                TestName = "User not found",
                UsernameToSearch = "user2",
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedUserId = (int?)null,
                ExpectedResponse = false
            },
            new UserExistsOutUserIdTestInput()
            {
                TestName = "User found",
                UsernameToSearch = "user1",
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "User 1",
                        Username = "user1"
                    }
                },
                ExpectedUserId = 1,
                ExpectedResponse = true
            },
        };
        public class UserExistsOutUserIdTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public string UsernameToSearch { get; set; }
            public int? ExpectedUserId { get; set; }
            public bool ExpectedResponse { get; set; }
        }
        #endregion UserExistsOutUserId
    }
}
