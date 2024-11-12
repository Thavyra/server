using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Thavyra.Contracts.User;

namespace Thavyra.Rest.Security;

public class FluentAuthorizationHandler<TRequirement>
    : AbstractValidator<(AuthorizationHandlerContext Context, TRequirement Requirement)>,
        IAuthorizationHandler where TRequirement : IAuthorizationRequirement
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.Requirements.OfType<TRequirement>())
        {
            var instance = (context, requirement);

            var result = await ValidateAsync(instance);

            if (result.IsValid)
            {
                context.Succeed(requirement);
            }
        }
    }
}

public class FluentAuthorizationHandler<TRequirement, TResource>
    : AbstractValidator<(AuthorizationHandlerContext Context, TRequirement Requirement, TResource Resource)>,
        IAuthorizationHandler where TRequirement : IAuthorizationRequirement
{
    protected void Scope(params string[] scope) =>
        RuleFor(x => x.Context.User)
            .Must(user => scope.Any(user.HasRelativeScope))
            .WithMessage($"User did not have required scope '{scope}'.");

    protected void Subject(Func<TResource, Guid> subject) =>
        RuleFor(x => x.Context.User)
            .Must((context, user) => user.HasSubject(subject(context.Resource)))
            .WithMessage("Subject claim did not match predicate.");

    protected void SubjectAsync(Func<TResource, CancellationToken, Task<Guid>> subjectTask) =>
        RuleFor(x => x.Context.User)
            .MustAsync(async (context, user, cancellationToken) =>
            {
                var subject = await subjectTask(context.Resource, cancellationToken);

                return user.HasSubject(subject);
            })
            .WithMessage("Subject claim did not match predicate.");

    protected void Client(Func<TResource, Guid> client) =>
        RuleFor(x => x.Context.User)
            .Must((context, user) => user.HasClient(client(context.Resource)))
            .WithMessage("ApplicationId claim did not match predicate.");

    protected void ClientAsync(Func<TResource, CancellationToken, Task<Guid>> clientTask) =>
        RuleFor(x => x.Context.User)
            .MustAsync(async (context, user, cancellationToken) =>
            {
                var client = await clientTask(context.Resource, cancellationToken);

                return user.HasClient(client);
            })
            .WithMessage("ApplicationId claim did not match predicate.");

    
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.Resource is not TResource resource)
        {
            return;
        }

        foreach (var requirement in context.Requirements.OfType<TRequirement>())
        {
            var instance = (context, requirement, resource);

            var result = await ValidateAsync(instance);

            if (!result.IsValid)
            {
                return;
            }

            await HandleRequirementAsync(context, requirement, resource);
        }
    }

    protected virtual Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement,
        TResource resource)
    {
        context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}