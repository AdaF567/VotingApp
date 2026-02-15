using FluentValidation.TestHelper;
using Voting.Services.Consts;
using Voting.Services.FluentValidators;

namespace VotingTests.ValidatorsTests
{
    public class CandidateValidatorTests
    {
        private CandidateValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new CandidateValidator();
        }

        [TestCase(null)]
        [TestCase("")]
        public void NameEmpty_Error(string? name)
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.CandidateRequest { Name = name });
            
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorCode(ErrorKeys.REQUIRED);
        }

        [Test]
        public void NameNotEmpty_NoError()
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.CandidateRequest { Name = "Rumcajs" });

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
