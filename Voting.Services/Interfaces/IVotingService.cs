using Voting.Services.DTO.Requests;
using Voting.Services.DTO.Responses;

namespace Voting.Services.Interfaces
{
    public interface IVotingService
    {
        Task<IEnumerable<VoterResponse>> GetVotersAsync(CancellationToken cancellationToken);
        Task<VoterResponse> AddVoterAsync(VoterRequest voter, CancellationToken cancellationToken);

        Task<IEnumerable<CandidateResponse>> GetCandidatesAsync(CancellationToken cancellationToken);
        Task<CandidateResponse> AddCandidateAsync(CandidateRequest candidate, CancellationToken cancellationToken);

        Task CastVoteAsync(VoteRequest vote, CancellationToken cancellationToken);
    }
}
