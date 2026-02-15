namespace Voting.Services.DTO.Responses
{
    public class VoterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasVoted { get; set; } = false;
    }
}
