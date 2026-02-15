using FluentValidation;
using Voting.Services.Consts;
using Voting.Services.DTO.Requests;

namespace Voting.Services.FluentValidators
{
    public class CandidateValidator : AbstractValidator<CandidateRequest>
    {
        public CandidateValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage($"{nameof(VoterRequest.Name)} is required")
                .WithErrorCode(ErrorKeys.REQUIRED);
        }
    }
}
