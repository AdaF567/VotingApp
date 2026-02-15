namespace Voting.Services.DTO.Responses
{
    public class CandidateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Votes { get; set; } = 0;
    }
}
