using Posterr.DB;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Posterr.DB.Models;
using Posterr.Infra.Repository;
using System.Linq;

namespace Posterr.Tests.Repository
{
    public class FollowRepositoryTest
    {
        #region UpdateUnfollowedStatus
        [Theory, MemberData(nameof(UpdateUnfollowedStatusTests))]
        public void UpdateUnfollowedStatusTest(UpdateUnfollowedStatusTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new FollowRepository(apiContext);
            Follow follow = apiContext.Follows.First();
            repository.UpdateUnfollowedStatus(follow, test.Unfollow);

            apiContext.Follows.First().Unfollowed.Should().Be(test.Unfollow);
        }

        public static TheoryData<UpdateUnfollowedStatusTestInput> UpdateUnfollowedStatusTests = new TheoryData<UpdateUnfollowedStatusTestInput>()
        {
            new UpdateUnfollowedStatusTestInput()
            {
                TestName = "Update follow to unfollowed",
                Unfollow = true,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2,
                        Unfollowed =  false
                    }
                },
            },
            new UpdateUnfollowedStatusTestInput()
            {
                TestName = "Update follow to followed",
                Unfollow = false,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2,
                        Unfollowed =  false
                    }
                },
            },
        };
        public class UpdateUnfollowedStatusTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            
            public Follow Follow { get; set; }
            public bool Unfollow { get; set; }
        }
        #endregion FollowUser

        #region CreateFollow
        [Theory, MemberData(nameof(CreateFollowTests))]
        public void CreateFollowTest(CreateFollowTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new FollowRepository(apiContext);
            repository.CreateFollow(test.FollowerId, test.FollowingId);

            apiContext.Follows.Should().ContainSingle(f => f.FollowerId == test.FollowerId && f.FollowingId == test.FollowingId);
        }

        public static TheoryData<CreateFollowTestInput> CreateFollowTests = new TheoryData<CreateFollowTestInput>()
        {
            new CreateFollowTestInput()
            {
                TestName = "Create follow",
                FollowerId = 1,
                FollowingId = 2
            }
        };
        public class CreateFollowTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int FollowerId { get; set; }
            public int FollowingId { get; set; }
        }
        #endregion CreateFollow

        #region IsUserFollowedBy
        [Theory, MemberData(nameof(IsUserFollowedByTests))]
        public void IsUserFollowedByTest(IsUserFollowedByTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new FollowRepository(apiContext);
            repository.IsUserFollowedBy(test.FollowerId, test.FollowingId);

            if (test.ExpectedResponse)
            {
                apiContext.Follows.Should().ContainSingle(f => f.FollowerId == test.FollowerId && f.FollowingId == test.FollowingId);
            }
            else
            {
                apiContext.Follows.Should().NotContain(f => f.FollowerId == test.FollowerId && f.FollowingId == test.FollowingId);
            }
        }

        public static TheoryData<IsUserFollowedByTestInput> IsUserFollowedByTests = new TheoryData<IsUserFollowedByTestInput>()
        {
            new IsUserFollowedByTestInput()
            {
                TestName = "Is not followed",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = false
            },
            new IsUserFollowedByTestInput()
            {
                TestName = "Is followed, but follow",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = false,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1,
                        Unfollowed =  false
                    }
                }
            },
            new IsUserFollowedByTestInput()
            {
                TestName = "Is followed",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = true,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2,
                        Unfollowed =  false
                    }
                }
            }
        };
        public class IsUserFollowedByTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int FollowerId { get; set; }
            public int FollowingId { get; set; }
            public bool ExpectedResponse { get; set; }
        }
        #endregion IsUserFollowedBy

        #region IsUserFollowedByOutFollow
        [Theory, MemberData(nameof(IsUserFollowedByOutFollowTests))]
        public void IsUserFollowedByOutFollowTest(IsUserFollowedByOutFollowTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new FollowRepository(apiContext);
            repository.IsUserFollowedBy(test.FollowerId, test.FollowingId, out Follow follow);

            if (test.ExpectedResponse)
            {
                follow.Should().BeEquivalentTo(test.ExpectedFollow);
                apiContext.Follows.Should().ContainSingle(f => f.FollowerId == test.FollowerId && f.FollowingId == test.FollowingId);
            }
            else
            {
                follow.Should().BeNull();
                apiContext.Follows.Should().NotContain(f => f.FollowerId == test.FollowerId && f.FollowingId == test.FollowingId);
            }
        }

        public static TheoryData<IsUserFollowedByOutFollowTestInput> IsUserFollowedByOutFollowTests = new TheoryData<IsUserFollowedByOutFollowTestInput>()
        {
            new IsUserFollowedByOutFollowTestInput()
            {
                TestName = "Is not followed",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = false
            },
            new IsUserFollowedByOutFollowTestInput()
            {
                TestName = "Is followed, but follow",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = false,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1,
                        Unfollowed =  false
                    }
                }
            },
            new IsUserFollowedByOutFollowTestInput()
            {
                TestName = "Is followed",
                FollowerId = 1,
                FollowingId = 2,
                ExpectedResponse = true,
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2,
                        Unfollowed =  false
                    }
                },
                ExpectedFollow = new Follow()
                {
                    Id = 1,
                    FollowerId = 1,
                    FollowingId = 2,
                    Unfollowed =  false
                }
            }
        };
        public class IsUserFollowedByOutFollowTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int FollowerId { get; set; }
            public int FollowingId { get; set; }
            public bool ExpectedResponse { get; set; }
            public Follow ExpectedFollow { get; set; }
        }
        #endregion IsUserFollowedByOutFollow
    }
}
