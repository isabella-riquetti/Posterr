﻿using Posterr.DB;
using Posterr.Services;
using Posterr.Services.Model;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Posterr.DB.Models;
using Posterr.Infra.Repository;
using System.Linq;

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

        public static TheoryData<GetUserTestInput> GetUserTests = new TheoryData<GetUserTestInput>()
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

        public static TheoryData<UserExistsTestInput> UserExistsTests = new TheoryData<UserExistsTestInput>()
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

        public static TheoryData<UserExistsOutUserIdTestInput> UserExistsOutUserIdTests = new TheoryData<UserExistsOutUserIdTestInput>()
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

        #region CreateUser
        [Theory, MemberData(nameof(CreateUserTests))]
        public void CreateUserTest(CreateUserTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new UserRepository(apiContext);
            service.CreateUser(test.Username);

            apiContext.Users.Should().ContainSingle(f => f.Username == test.Username);
        }

        public static TheoryData<CreateUserTestInput> CreateUserTests = new TheoryData<CreateUserTestInput>()
        {
            new CreateUserTestInput()
            {
                TestName = "Create User with name",
                Username = "user1"
            }
        };
        public class CreateUserTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public string Username { get; set; }
        }
        #endregion CreateUser
    }
}