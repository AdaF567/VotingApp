using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voting.Services.DTO.Requests;
using Voting.Services.DTO.Responses;
using Voting.Services.Interfaces;

namespace Voting.Web.API.Controllers
{
    /// <summary>
    /// Allows to manage Candidates, Voters and Votes.
    /// </summary>
    /// <param name="votingService"></param>
    [ApiController]
    [AllowAnonymous]
    [Route("voting")]
    public class VotingController(IVotingService votingService) : ControllerBase
    {
        private readonly IVotingService _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));

        /// <summary>
        /// Gets all Voters.
        /// </summary>
        /// <returns>Collection of Voters</returns>
        [HttpGet]
        [Route("voters")]
        [ProducesResponseType(typeof(IEnumerable<VoterResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VoterResponse>>> GetVoters()
        {
            return Ok(await _votingService.GetVotersAsync(CancellationToken.None));
        }

        /// <summary>
        /// Adds a new Voter.
        /// </summary>
        /// <param name="voter">A Voter to be added.</param>
        /// <param name="validator">Validator to be used to validate incoming model.</param>
        /// <returns>Created Voter</returns>
        [HttpPost]
        [Route("voter")]
        [ProducesResponseType(typeof(VoterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VoterResponse>> AddVoter([FromBody] VoterRequest voter, [FromServices] IValidator<VoterRequest> validator)
        {
            var result = await validator.ValidateAsync(voter);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok(await _votingService.AddVoterAsync(voter, CancellationToken.None));
        }

        /// <summary>
        /// Gets all Candidates.
        /// </summary>
        /// <returns>Collection of Candidates</returns>
        [HttpGet]
        [Route("candidates")]
        [ProducesResponseType(typeof(IEnumerable<CandidateResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CandidateResponse>>> GetCandidates()
        {
            return Ok(await _votingService.GetCandidatesAsync(CancellationToken.None));
        }

        /// <summary>
        /// Adds a new Candidate.
        /// </summary>
        /// <param name="candidate">Candidate to be added.</param>
        /// <param name="validator">Validator to be used ti validate incoming model.</param>
        /// <returns>Created Candidate</returns>
        [HttpPost]
        [Route("candidate")]
        [ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CandidateResponse>> AddCandidate([FromBody] CandidateRequest candidate, [FromServices] IValidator<CandidateRequest> validator)
        {
            var result = await validator.ValidateAsync(candidate);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok(await _votingService.AddCandidateAsync(candidate, CancellationToken.None));
        }

        /// <summary>
        /// Casts a Vote.
        /// </summary>
        /// <param name="vote">Vote to be casted.</param>
        /// <param name="validator">Validator to be used to validate incoming model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cast-vote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CastVote([FromBody] VoteRequest vote, [FromServices] IValidator<VoteRequest> validator)
        {
            var result = await validator.ValidateAsync(vote);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            await _votingService.CastVoteAsync(vote, CancellationToken.None);
            return Ok();
        }
    }
}
