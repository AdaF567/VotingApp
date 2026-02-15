using Microsoft.EntityFrameworkCore;

namespace Voting.DAL.Entities
{
    public class VotingDbContext(DbContextOptions<VotingDbContext> options) : DbContext(options)
    {
        public DbSet<Voter> Voters { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
    }
}
