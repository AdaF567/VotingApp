using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voting.DAL.Entities
{
    public class Vote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }
        [ForeignKey(nameof(Candidate))]
        public required int CandidateId { get; set; }
        public Candidate Candidate { get; set; }
    }
}
