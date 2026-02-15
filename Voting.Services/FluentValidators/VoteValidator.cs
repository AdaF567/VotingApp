using FluentValidation;
using Voting.Services.Consts;
using Voting.Services.DTO.Requests;

namespace Voting.Services.FluentValidators
{
    public class VoteValidator : AbstractValidator<VoteRequest>
    {
        public VoteValidator()
        {
            RuleFor(x => x.VoterId).NotEmpty()
                .WithMessage($"{nameof(VoteRequest.VoterId)} is required")
                .WithErrorCode(ErrorKeys.REQUIRED);

            RuleFor(x => x.CandidateId).NotEmpty()
                .WithMessage($"{nameof(VoteRequest.CandidateId)} is required")
                .WithErrorCode(ErrorKeys.REQUIRED);
        }
    }
}
