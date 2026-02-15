using Microsoft.AspNetCore.Mvc;
using Moq;
using Voting.Services.DTO.Requests;
using Voting.Services.DTO.Responses;
using Voting.Services.FluentValidators;
using Voting.Services.Interfaces;
using Voting.Web.API.Controllers;

namespace VotingTests.ControllersTests
{
    public class VotingControllerTests
    {
        private VotingController _controller;
        private Mock<IVotingService> _votingServiceMock;

        [SetUp]
        public void Setup()
        {
            _votingServiceMock = new();
            _controller = new(_votingServiceMock.Object);
        }

        [Test]
        public async Task GetVoters_ReturnsVoters()
        {
            IEnumerable<VoterResponse> voters = [
                new VoterResponse{
                    Id = 1,
                    Name = "Pluto",
                    HasVoted = false,
                },
                new VoterResponse{
                    Id = 2,
                    Name = "Rumcajs",
                    HasVoted = true
                }];
            _votingServiceMock.Setup(x => x.GetVotersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(voters);

            var result = await _controller.GetVoters();

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result.Result;
            var returnValue = (ICollection<VoterResponse>)okObjectResult.Value;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Count, Is.EqualTo(2));
            Assert.That(returnValue.ElementAt(0).Id, Is.EqualTo(1));
            Assert.That(returnValue.ElementAt(0).Name, Is.EqualTo("Pluto"));
            Assert.That(returnValue.ElementAt(0).HasVoted, Is.False);
            Assert.That(returnValue.ElementAt(1).Id, Is.EqualTo(2));
            Assert.That(returnValue.ElementAt(1).Name, Is.EqualTo("Rumcajs"));
            Assert.That(returnValue.ElementAt(1).HasVoted, Is.True);
        }

        [Test]
        public async Task GetCandidates_ReturnsCandidates()
        {
            IEnumerable<CandidateResponse> candidates = [
                new CandidateResponse{
                    Id = 1,
                    Name = "Pluto",
                    Votes = 1,
                },
                new CandidateResponse{
                    Id = 2,
                    Name = "Rumcajs",
                    Votes = 3
                }];

            _votingServiceMock.Setup(x => x.GetCandidatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(candidates);

            var result = await _controller.GetCandidates();

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result.Result;
            var returnValue = (ICollection<CandidateResponse>)okObjectResult.Value;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Count, Is.EqualTo(2));
            Assert.That(returnValue.ElementAt(0).Id, Is.EqualTo(1));
            Assert.That(returnValue.ElementAt(0).Name, Is.EqualTo("Pluto"));
            Assert.That(returnValue.ElementAt(0).Votes, Is.EqualTo(1));
            Assert.That(returnValue.ElementAt(1).Id, Is.EqualTo(2));
            Assert.That(returnValue.ElementAt(1).Name, Is.EqualTo("Rumcajs"));
            Assert.That(returnValue.ElementAt(1).Votes, Is.EqualTo(3));
        }

        [Test]
        public void CastVote_InvalidRequest_ReturnsBadRequest()
        {
            var result = _controller.CastVote(new VoteRequest { CandidateId = default, VoterId = 1 }, new VoteValidator());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddVoter_InvalidRequest_ReturnsBadRequest()
        {
            var result = await _controller.AddVoter(new VoterRequest { Name = "" }, new VoterValidator());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddVoter_Success_ReturnsVoter()
        {
            var expected = new VoterResponse
            {
                Id = 1,
                Name = "Pluto",
                HasVoted = false
            };

            _votingServiceMock.Setup(x => x.AddVoterAsync(It.IsAny<VoterRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.AddVoter(new VoterRequest { Name = "Pluto" }, new VoterValidator());

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result.Result;
            var returnValue = (VoterResponse)okObjectResult.Value;

            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Id, Is.EqualTo(expected.Id));
            Assert.That(returnValue.Name, Is.EqualTo(expected.Name));
            Assert.That(returnValue.HasVoted, Is.EqualTo(expected.HasVoted));
        }

        [Test]
        public async Task AddCandidate_InvalidRequest_ReturnsBadRequest()
        {
            var result = await _controller.AddCandidate(new CandidateRequest { Name = "" }, new CandidateValidator());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddCandidate_Success_ReturnsCandidate()
        {
            var expected = new CandidateResponse
            {
                Id = 1,
                Name = "Rumcajs",
                Votes = 0
            };

            _votingServiceMock.Setup(x => x.AddCandidateAsync(It.IsAny<CandidateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.AddCandidate(new CandidateRequest { Name = "Rumcajs" }, new CandidateValidator());

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result.Result;
            var returnValue = (CandidateResponse)okObjectResult.Value;

            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Id, Is.EqualTo(expected.Id));
            Assert.That(returnValue.Name, Is.EqualTo(expected.Name));
            Assert.That(returnValue.Votes, Is.EqualTo(expected.Votes));
        }
    }
}
