using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Services.Consts;
using Voting.Services.DTO.Requests;
using Voting.Services.DTO.Responses;
using Voting.Services.Exceptions;
using Voting.Services.Interfaces;

namespace Voting.Services.Services
{
    public class VotingService(DAL.Entities.VotingDbContext votingDbContext, ILogger<VotingService> logger) : IVotingService
    {
        private readonly DAL.Entities.VotingDbContext _votingDbContext = votingDbContext ?? throw new ArgumentNullException(nameof(votingDbContext));
        private readonly ILogger<VotingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<IEnumerable<VoterResponse>> GetVotersAsync(CancellationToken cancellationToken)
        {
            return (await _votingDbContext
                .Voters
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .Select(x => new VoterResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    HasVoted = x.HasVoted,
                });
        }

        public async Task<VoterResponse> AddVoterAsync(VoterRequest voter, CancellationToken cancellationToken)
        {
            try
            {
                var entityEntry = _votingDbContext.Voters.Add(new DAL.Entities.Voter
                {
                    Name = voter.Name
                });
                await _votingDbContext.SaveChangesAsync(cancellationToken);

                return new VoterResponse
                {
                    Id = entityEntry.Entity.Id,
                    Name = entityEntry.Entity.Name,
                    HasVoted = entityEntry.Entity.HasVoted,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding a Voter has failed!");
                throw new VotingInvalidOperationException(ErrorKeys.VOTER_ADD_FAILURE);
            }
        }

        public async Task<IEnumerable<CandidateResponse>> GetCandidatesAsync(CancellationToken cancellationToken)
        {
            return (await _votingDbContext
                .Candidates
                .Include(x => x.Votes)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .Select(x => new CandidateResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Votes = x.Votes.Count()
                });
        }

        public async Task<CandidateResponse> AddCandidateAsync(CandidateRequest candidate, CancellationToken cancellationToken)
        {
            try
            {
                var entityEntry = _votingDbContext.Candidates.Add(new DAL.Entities.Candidate
                {
                    Name = candidate.Name
                });
                await _votingDbContext.SaveChangesAsync(cancellationToken);

                return new CandidateResponse
                {
                    Id = entityEntry.Entity.Id,
                    Name = entityEntry.Entity.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding a Candidate has failed!");
                throw new VotingInvalidOperationException(ErrorKeys.CANDIDATE_ADD_FAILURE);
            }
        }

        public async Task CastVoteAsync(VoteRequest vote, CancellationToken cancellationToken)
        {
            var voter = await _votingDbContext.Voters.FindAsync(vote.VoterId, cancellationToken)
                ?? throw new KeyNotFoundException($"Voter of id '{vote.VoterId}' not found!");

            _ = await _votingDbContext.Candidates.FindAsync(vote.CandidateId, cancellationToken)
                ?? throw new KeyNotFoundException($"Candidate of id '{vote.CandidateId}' not found!");

            try
            {
                _votingDbContext.Votes.Add(new DAL.Entities.Vote
                {
                    CandidateId = vote.CandidateId,
                });

                voter.HasVoted = true;
                await _votingDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Voting has failed!");
                throw new VotingInvalidOperationException(ErrorKeys.VOTING_FAILURE);
            }
        }
    }
}
