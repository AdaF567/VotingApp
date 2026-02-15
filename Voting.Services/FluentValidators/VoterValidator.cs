using FluentValidation;
using Voting.Services.Consts;
using Voting.Services.DTO.Requests;

namespace Voting.Services.FluentValidators
{
    public class VoterValidator : AbstractValidator<VoterRequest>
    {
        public VoterValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage($"{nameof(VoterRequest.Name)} is required")
                .WithErrorCode(ErrorKeys.REQUIRED);
        }
    }
}
