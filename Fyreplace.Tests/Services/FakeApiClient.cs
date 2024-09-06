using Fyreplace.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fyreplace.Tests.Services
{
    public sealed partial class FakeApiClient : IApiClient
    {
        #region Chapters

        public Task<Chapter> CreateChapterAsync(Guid id, CancellationToken cancellationToken) => CreateChapterAsync(id);

        public Task DeleteChapterAsync(Guid id, int position, CancellationToken cancellationToken) => DeleteChapterAsync(id, position);

        public Task<string> SetChapterImageAsync(Guid id, int position, Stream body, CancellationToken cancellationToken) => SetChapterImageAsync(id, position, body);

        public Task SetChapterPositionAsync(Guid id, int position, ChapterPositionUpdate body, CancellationToken cancellationToken) => SetChapterPositionAsync(id, position, body);

        public Task<string> SetChapterTextAsync(Guid id, int position, string body, CancellationToken cancellationToken) => SetChapterTextAsync(id, position, body);

        #endregion

        #region Comments

        public Task AcknowledgeCommentAsync(Guid id, int position, CancellationToken cancellationToken) => AcknowledgeCommentAsync(id, position);

        public Task<long> CountCommentsAsync(Guid id, bool? read, CancellationToken cancellationToken) => CountCommentsAsync(id, read);

        public Task<Comment> CreateCommentAsync(Guid id, CommentCreation body, CancellationToken cancellationToken) => CreateCommentAsync(id, body);

        public Task DeleteCommentAsync(Guid id, int position, CancellationToken cancellationToken) => DeleteCommentAsync(id, position);

        public Task<ICollection<Comment>> ListCommentsAsync(Guid id, int? page, CancellationToken cancellationToken) => ListCommentsAsync(id, page);

        public Task SetCommentReportedAsync(Guid id, int position, ReportUpdate body, CancellationToken cancellationToken) => SetCommentReportedAsync(id, position, body);

        #endregion

        #region Emails

        public Task ActivateEmailAsync(EmailActivation body, CancellationToken cancellationToken) => ActivateEmailAsync(body);

        public Task<long> CountEmailsAsync(CancellationToken cancellationToken) => CountEmailsAsync();

        public Task<Email> CreateEmailAsync(EmailCreation body, CancellationToken cancellationToken) => CreateEmailAsync(body);

        public Task DeleteEmailAsync(Guid id, CancellationToken cancellationToken) => DeleteEmailAsync(id);

        public Task<ICollection<Email>> ListEmailsAsync(int? page, CancellationToken cancellationToken) => ListEmailsAsync(page);

        public Task SetMainEmailAsync(Guid id, CancellationToken cancellationToken) => SetMainEmailAsync(id);

        #endregion

        #region Posts

        public Task<long> CountPostsAsync(PostListingType type, CancellationToken cancellationToken) => CountPostsAsync(type);

        public Task<Post> CreatePostAsync(CancellationToken cancellationToken) => CreatePostAsync();

        public Task DeletePostAsync(Guid id, CancellationToken cancellationToken) => DeletePostAsync(id);

        public Task<Post> GetPostAsync(Guid id, CancellationToken cancellationToken) => GetPostAsync(id);

        public Task<ICollection<Post>> ListPostsAsync(bool? ascending, int? page, PostListingType type, CancellationToken cancellationToken) => ListPostsAsync(ascending, page, type);

        public Task<ICollection<Post>> ListPostsFeedAsync(CancellationToken cancellationToken) => ListPostsFeedAsync();

        public Task PublishPostAsync(Guid id, PostPublication body, CancellationToken cancellationToken) => PublishPostAsync(id, body);

        public Task SetPostReportedAsync(Guid id, ReportUpdate body, CancellationToken cancellationToken) => SetPostReportedAsync(id, body);

        public Task SetPostSubscribedAsync(Guid id, SubscriptionUpdate body, CancellationToken cancellationToken) => SetPostSubscribedAsync(id, body);

        public Task VotePostAsync(Guid id, VoteCreation body, CancellationToken cancellationToken) => VotePostAsync(id, body);

        #endregion

        #region Reports

        public Task<ICollection<Report>> ListReportsAsync(int? page, CancellationToken cancellationToken) => ListReportsAsync(page);

        #endregion

        #region Subscriptions

        public Task ClearUnreadSubscriptionsAsync(CancellationToken cancellationToken) => ClearUnreadSubscriptionsAsync();

        public Task DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken) => DeleteSubscriptionAsync(id);

        public Task<ICollection<Subscription>> ListUnreadSubscriptionsAsync(int? page, CancellationToken cancellationToken) => ListUnreadSubscriptionsAsync(page);

        #endregion

        #region Tokens

        public Task CreateNewTokenAsync(NewTokenCreation body, CancellationToken cancellationToken) => CreateNewTokenAsync(body);

        public Task<string> CreateTokenAsync(TokenCreation body, CancellationToken cancellationToken) => CreateTokenAsync(body);

        public Task<string> GetNewTokenAsync(CancellationToken cancellationToken) => GetNewTokenAsync();

        #endregion

        #region Users

        public Task<long> CountBlockedUsersAsync(CancellationToken cancellationToken) => CountBlockedUsersAsync();

        public Task<User> CreateUserAsync(UserCreation body, CancellationToken cancellationToken) => CreateUserAsync(body);

        public Task DeleteCurrentUserAsync(CancellationToken cancellationToken) => DeleteCurrentUserAsync();

        public Task DeleteCurrentUserAvatarAsync(CancellationToken cancellationToken) => DeleteCurrentUserAvatarAsync();

        public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken) => GetCurrentUserAsync();

        public Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken) => GetUserAsync(id);

        public Task<ICollection<Profile>> ListBlockedUsersAsync(int? page, CancellationToken cancellationToken) => ListBlockedUsersAsync(page);

        public Task<string> SetCurrentUserAvatarAsync(Stream body, CancellationToken cancellationToken) => SetCurrentUserAvatarAsync(body);

        public Task<string> SetCurrentUserBioAsync(string body, CancellationToken cancellationToken) => SetCurrentUserBioAsync(body);

        public Task SetUserBannedAsync(Guid id, CancellationToken cancellationToken) => SetUserBannedAsync(id);

        public Task SetUserBlockedAsync(Guid id, BlockUpdate body, CancellationToken cancellationToken) => SetUserBlockedAsync(id, body);

        public Task SetUserReportedAsync(Guid id, ReportUpdate body, CancellationToken cancellationToken)
        => SetUserReportedAsync(id, body);

        #endregion
    }

    #region Chapters

    public sealed partial class FakeApiClient
    {
        public Task<Chapter> CreateChapterAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChapterAsync(Guid id, int position)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetChapterImageAsync(Guid id, int position, Stream body)
        {
            throw new NotImplementedException();
        }

        public Task SetChapterPositionAsync(Guid id, int position, ChapterPositionUpdate body)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetChapterTextAsync(Guid id, int position, string body)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Comments

    public sealed partial class FakeApiClient
    {
        public Task AcknowledgeCommentAsync(Guid id, int position)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountCommentsAsync(Guid id, bool? read)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> CreateCommentAsync(Guid id, CommentCreation body)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommentAsync(Guid id, int position)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Comment>> ListCommentsAsync(Guid id, int? page)
        {
            throw new NotImplementedException();
        }

        public Task SetCommentReportedAsync(Guid id, int position, ReportUpdate body)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Emails

    public sealed partial class FakeApiClient
    {
        public Task ActivateEmailAsync(EmailActivation body)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountEmailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Email> CreateEmailAsync(EmailCreation body)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEmailAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Email>> ListEmailsAsync(int? page)
        {
            throw new NotImplementedException();
        }

        public Task SetMainEmailAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Posts

    public sealed partial class FakeApiClient
    {
        public Task<long> CountPostsAsync(PostListingType type)
        {
            throw new NotImplementedException();
        }

        public Task<Post> CreatePostAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeletePostAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Post>> ListPostsAsync(bool? ascending, int? page, PostListingType type)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Post>> ListPostsFeedAsync()
        {
            throw new NotImplementedException();
        }

        public Task PublishPostAsync(Guid id, PostPublication body)
        {
            throw new NotImplementedException();
        }

        public Task SetPostReportedAsync(Guid id, ReportUpdate body)
        {
            throw new NotImplementedException();
        }

        public Task SetPostSubscribedAsync(Guid id, SubscriptionUpdate body)
        {
            throw new NotImplementedException();
        }

        public Task VotePostAsync(Guid id, VoteCreation body)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Reports

    public sealed partial class FakeApiClient
    {
        public Task<ICollection<Report>> ListReportsAsync(int? page)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Subscriptions

    public sealed partial class FakeApiClient
    {
        public Task ClearUnreadSubscriptionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteSubscriptionAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Subscription>> ListUnreadSubscriptionsAsync(int? page)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Tokens

    public sealed partial class FakeApiClient
    {
        public static readonly string badIdentifier = "bad-identifier";
        public static readonly string goodIdentifier = "good-identifier";
        public static readonly string badSecret = "nopenope";
        public static readonly string goodSecret = "abcd1234";
        public static readonly string token = "token";

        public Task CreateNewTokenAsync(NewTokenCreation body) => body.Identifier == goodIdentifier
            ? Task.FromResult(token)
            : throw new FakeApiException(HttpStatusCode.NotFound);

        public Task<string> CreateTokenAsync(TokenCreation body) => body.Identifier != goodIdentifier
            ? throw new FakeApiException(HttpStatusCode.NotFound)
            : body.Secret != goodSecret
            ? throw new FakeApiException(HttpStatusCode.NotFound)
            : Task.FromResult(token);

        public Task<string> GetNewTokenAsync() => Task.FromResult(token);
    }

    #endregion

    #region Users

    public sealed partial class FakeApiClient
    {
        public Task<long> CountBlockedUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> CreateUserAsync(UserCreation body)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCurrentUserAvatarAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Profile>> ListBlockedUsersAsync(int? page)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetCurrentUserAvatarAsync(Stream body)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetCurrentUserBioAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task SetUserBannedAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SetUserBlockedAsync(Guid id, BlockUpdate body)
        {
            throw new NotImplementedException();
        }

        public Task SetUserReportedAsync(Guid id, ReportUpdate body)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
