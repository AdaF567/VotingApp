namespace Voting.Services.DTO.Requests
{
    public class VoteRequest
    {
        public required int VoterId { get; set; }
        public required int CandidateId { get; set; }
    }
}
