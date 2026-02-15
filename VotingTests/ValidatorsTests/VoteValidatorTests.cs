using FluentValidation.TestHelper;
using Voting.Services.Consts;
using Voting.Services.FluentValidators;

namespace VotingTests.ValidatorsTests
{
    public class VoteValidatorTests
    {
        private VoteValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new VoteValidator();
        }

        [Test]
        public void IdsProvided_NoError()
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.VoteRequest { VoterId = 1, CandidateId = 1 });

            result.ShouldNotHaveValidationErrorFor(x => x.VoterId);
            result.ShouldNotHaveValidationErrorFor(x => x.CandidateId);
        }

        [Test]
        public void VoterIdNotProvided_Error()
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.VoteRequest { VoterId = default, CandidateId = 1 });

            result.ShouldHaveValidationErrorFor(x => x.VoterId).WithErrorCode(ErrorKeys.REQUIRED);
            result.ShouldNotHaveValidationErrorFor(x => x.CandidateId);
        }

        [Test]
        public void CandidateIdNotProvided_Error()
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.VoteRequest { VoterId = 1, CandidateId = default });

            result.ShouldNotHaveValidationErrorFor(x => x.VoterId);
            result.ShouldHaveValidationErrorFor(x => x.CandidateId).WithErrorCode(ErrorKeys.REQUIRED);
        }
    }
}
