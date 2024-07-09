using Ardalis.GuardClauses;
using CQRS.Core.Domain;
using CQRS.Core.Extensions.GuardClauses;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        Guard.Against.Null(id);
        Guard.Against.NullOrWhiteSpace(author);
        Guard.Against.NullOrWhiteSpace(message);
        
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });
    }

    public void EditMessage(string message)
    {
        Guard.Against.NullOrWhiteSpace(message, nameof(message), $"The value of message cannot be null or empty. !");
        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }

    public void LikePost()
    {
        
        GuardExtension.Ensure.True(_active, "You cannot like an inactive post");
        
        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
    }

    public void AddComment(string comment, string username)
    {
        GuardExtension.Ensure.True(_active, "You cannot add a comment to and inactive post");

        Guard.Against.NullOrWhiteSpace(comment, nameof(comment), "The value of comment cannot be null or empty.");
        Guard.Against.NullOrWhiteSpace(username, nameof(username), "Username cannot be null or empty");

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
        });
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        GuardExtension.Ensure.True(_active, "You cannot edit a comment to and inactive post");
        GuardExtension.Ensure.True(
            _comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase),
            "You are not allowed to edit a comment that was made by another user!");

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now
        });
    }


    public void RemoveComment(Guid commentId, string username)
    {
        GuardExtension.Ensure.True(_active, "You cannot remove a comment to and inactive post");
        GuardExtension.Ensure.True(
            _comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase),
            "You are not allowed to remove a comment that was made by another user!");

        RaiseEvent(new CommentRemovedEvent
        {
            Id = _id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
    }

    public void DeletePost(string username)
    {
        GuardExtension.Ensure.True(_active, "The post has already been removed");
        GuardExtension.Ensure.True(_author.Equals(username, StringComparison.CurrentCultureIgnoreCase),
            "You are not allowed to delete a post that was made by somebody else!");

        RaiseEvent(new PostRemovedEvent
        {
            Id = _id
        });
    }


    protected override void Apply(object @event)
    {
        switch (@event)
        {
            case PostCreatedEvent e:
                _id = e.Id;
                _active = true;
                _author = e.Author;
                break;
            case MessageUpdatedEvent e:
                _id = e.Id;
                break;
            case PostLikedEvent e:
                _id = e.Id;
                break;
            case PostRemovedEvent e:
                _id = e.Id;
                _active = false;
                break;
            case CommentUpdatedEvent e:
                _id = e.Id;
                _comments[e.CommentId] = new Tuple<string, string>(e.Comment, e.Username);
                break;
            case CommentRemovedEvent e:
                _id = e.Id;
                _comments.Remove(e.CommentId);
                break;
            case CommentAddedEvent e:
                _id = e.Id;
                _comments.Add(e.CommentId, new Tuple<string, string>(e.Comment, e.Username));
                break;
        }
    }
}