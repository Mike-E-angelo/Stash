namespace Scrutor.Decorate.Model;

public sealed class CancelProcessException : Exception
{
	public CancelProcessException(string reason) : base($"A cancellation request was detected: {reason}")
		=> Reason = reason;

	public string Reason { get; }
}