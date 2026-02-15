using FluentValidation.TestHelper;
using Voting.Services.Consts;
using Voting.Services.FluentValidators;

namespace VotingTests.ValidatorsTests
{
    public class VoterValidatorTests
    {
        private VoterValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new VoterValidator();
        }

        [TestCase(null)]
        [TestCase("")]
        public void NameEmpty_Error(string? name)
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.VoterRequest { Name = name });
            
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorCode(ErrorKeys.REQUIRED);
        }

        [Test]
        public void NameNotEmpty_NoError()
        {
            var result = _validator.TestValidate(new Voting.Services.DTO.Requests.VoterRequest { Name = "Rumcajs" });

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
