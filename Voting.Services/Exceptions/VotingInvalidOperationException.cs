namespace Voting.Services.Exceptions;

public class VotingInvalidOperationException(string message) : Exception(message)
{
}
