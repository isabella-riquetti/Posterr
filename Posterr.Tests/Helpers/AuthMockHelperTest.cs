using Microsoft.AspNetCore.Http;
using Posterr.API.Helper;
using Xunit;

namespace Posterr.Tests.Helpers
{
    public class AuthMockHelperTest
    {
        #region GetUserFromHeader
        [Theory, MemberData(nameof(GetUserFromHeaderTests))]
        public void GetUserFromHeaderTest(GetUserFromHeaderTestInput test)
        {
            var httpContext = new DefaultHttpContext();
            if (test.AddHeader)
            {
                httpContext.Request.Headers.Add("AuthenticatedUserId", test.AuthenticatedUserId);
            }

            int response = AuthMockHelper.GetUserFromHeader(httpContext.Request);

            Assert.Equal(test.ExpectedResponse, response);
        }

        public static readonly TheoryData<GetUserFromHeaderTestInput> GetUserFromHeaderTests = new()
        {
            new GetUserFromHeaderTestInput()
            {
                TestName = "Empty header",
                AddHeader = false,
                ExpectedResponse = 1
            },
            new GetUserFromHeaderTestInput()
            {
                TestName = "Invalid ID",
                AddHeader = true,
                AuthenticatedUserId = "A",
                ExpectedResponse = 1
            },
            new GetUserFromHeaderTestInput()
            {
                TestName = "Valid ID",
                AddHeader = true,
                AuthenticatedUserId = "7",
                ExpectedResponse = 7
            }
        };
        public class GetUserFromHeaderTestInput
        {
            public string TestName { get; set; }
            public bool AddHeader { get; set; }
            public string AuthenticatedUserId { get; set; }
            public int ExpectedResponse { get; set; }
        }
        #endregion GetUserFromHeader
    }
}
