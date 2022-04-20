using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Services.Model;
using Posterr.Services.User;

namespace Posterr.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;

        public FollowService(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public BaseResponse FollowUser(int id, int authenticatedUserId)
        {
            if (_followRepository.IsUserFollowedBy(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse.CreateError("User is already followed by you");
            }

            // Case we don't have a follow yet, we should create one
            if (follow != null)
            {
                _followRepository.UpdateUnfollowedStatus(follow, false);
            }
            // Case we already have a followed with the Unfollowed true, we should change it to false
            else
            {
                _followRepository.CreateFollow(authenticatedUserId, id);
            }

            return BaseResponse.CreateSuccess();
        }

        public BaseResponse UnfollowUser(int id, int authenticatedUserId)
        {
            if (!_followRepository.IsUserFollowedBy(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse.CreateError("You don't follow this user");
            }

            _followRepository.UpdateUnfollowedStatus(follow, true);

            return BaseResponse.CreateSuccess();
        }
    }
}
