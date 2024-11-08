using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Users.PutAvatar;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Avatar)
            .NotNull().WithMessage("Avatar file is required.");

        RuleFor(x => x.Avatar.Length)
            .LessThan(10 * 1024 * 2024).WithMessage("Avatar file is too large.");

        RuleFor(x => x.Avatar.FileName)
            .Must(x => _permittedExtensions.Contains(Path.GetExtension(x).ToLower())).WithMessage("Avatar must be an image.");
    }

    private readonly string[] _permittedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
}