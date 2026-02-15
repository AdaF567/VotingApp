using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.DAL.Entities;
using Voting.Services.Services;
using Moq;

namespace VotingTests.ServicesTests
{
    public class VotingServiceTests
    {
        private VotingService _votingService;
        private VotingDbContext _votingDbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _votingDbContext = new VotingDbContext(options);
            _votingService = new VotingService(_votingDbContext, new Mock<ILogger<VotingService>>().Object);
        }

        [TearDown]
        public void TearDown()
        {
            _votingDbContext.Dispose();
        }

        #region Voters
        [Test]
        public async Task GetVotersAsync_NoVoters_ReturnsEmptyList()
        {
            var result = await _votingService.GetVotersAsync(CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetVotersAsync_VotersInDatabase_ReturnsAllVoters()
        {
            _votingDbContext.AddRange([
                new Voter
                {
                    Id = 1,
                    Name = "Peppa"
                },
                new Voter
                {
                    Id = 2,
                    Name = "Rumcajs",
                    HasVoted = true,
                }
            ]);
            _votingDbContext.SaveChanges();

            var result = await _votingService.GetVotersAsync(CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ElementAt(0).Id, Is.EqualTo(1));
            Assert.That(result.ElementAt(0).Name, Is.EqualTo("Peppa"));
            Assert.That(result.ElementAt(0).HasVoted, Is.False);
            Assert.That(result.ElementAt(1).Id, Is.EqualTo(2));
            Assert.That(result.ElementAt(1).Name, Is.EqualTo("Rumcajs"));
            Assert.That(result.ElementAt(1).HasVoted, Is.True);
        }
        #endregion

        #region Candidates
        [Test]
        public async Task GetCandidatesAsync_NoCandidates_ReturnsEmptyList()
        {
            var result = await _votingService.GetCandidatesAsync(CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetCandidatesAsync_CandidatesInDatabase_ReturnsAllCandidates()
        {
            _votingDbContext.AddRange([
                new Candidate
                {
                    Id = 1,
                    Name = "Johnny Bravo"
                },
                new Candidate
                {
                    Id = 2,
                    Name = "Pluto",
                }
            ]);

            _votingDbContext.Votes.AddRange([
                new Vote
                {
                    Id = 1,
                    CandidateId = 1,
                },
                new Vote
                {
                    Id = 2,
                    CandidateId = 1,
                },
                new Vote
                {
                    Id = 3,
                    CandidateId = 2,
                }
            ]);

            _votingDbContext.SaveChanges();

            var result = await _votingService.GetCandidatesAsync(CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ElementAt(0).Id, Is.EqualTo(1));
            Assert.That(result.ElementAt(0).Name, Is.EqualTo("Johnny Bravo"));
            Assert.That(result.ElementAt(0).Votes, Is.EqualTo(2));
            Assert.That(result.ElementAt(1).Id, Is.EqualTo(2));
            Assert.That(result.ElementAt(1).Name, Is.EqualTo("Pluto"));
            Assert.That(result.ElementAt(1).Votes, Is.EqualTo(1));
        }
        #endregion

        #region Votes
        [Test]
        public async Task VoteAsync_CandidateNotFound_ThrowsKeyNotFoundException()
        {
            _votingDbContext.Add(new Voter
            {
                Id = 1,
                Name = "Johnny Bravo"
            });

            _votingDbContext.SaveChanges();

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async ()
                => await _votingService.CastVoteAsync(new Voting.Services.DTO.Requests.VoteRequest
                {
                    CandidateId = 1,
                    VoterId = 1,
                }, CancellationToken.None));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Candidate of id '1' not found!"));
        }

        [Test]
        public async Task VoteAsync_VoterNotFound_ThrowsKeyNotFoundException()
        {
            _votingDbContext.Add(new Candidate
            {
                Id = 1,
                Name = "Johnny Bravo"
            });

            _votingDbContext.SaveChanges();

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async ()
                => await _votingService.CastVoteAsync(new Voting.Services.DTO.Requests.VoteRequest
                {
                    CandidateId = 1,
                    VoterId = 1,
                }, CancellationToken.None));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Voter of id '1' not found!"));
        }

        [Test]
        public async Task VoteAsync_VotedSuccessfully()
        {
            _votingDbContext.AddRange(new Candidate
            {
                Id = 1,
                Name = "Johnny Bravo"
            });

            _votingDbContext.Add(new Voter
            {
                Id = 1,
                Name = "Peppa"
            });

            _votingDbContext.SaveChanges();

            var result = _votingService.CastVoteAsync(new Voting.Services.DTO.Requests.VoteRequest
            {
                CandidateId = 1,
                VoterId = 1,
            }, CancellationToken.None);

            Assert.That(result.IsCompletedSuccessfully, Is.True);

            var voter = _votingDbContext.Voters.Find(1);
            Assert.That(voter?.HasVoted, Is.True);

            var vote = _votingDbContext.Votes.SingleOrDefault();
            Assert.That(vote, Is.Not.Null);
            Assert.That(vote.CandidateId, Is.EqualTo(1));
        }
        #endregion
    }
}
