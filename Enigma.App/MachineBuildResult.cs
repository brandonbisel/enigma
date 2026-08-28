namespace Enigma.App;

/// <summary>
/// The outcome of keying a machine from a key sheet. Building the machine is the
/// step that can fail, and it fails for a reason the operator can act on — a wheel
/// that does not exist, a cable given twice, a plugboard on a machine that has
/// none. A front end needs to show that rather than fall over, so the failure is
/// returned instead of thrown.
/// </summary>
public sealed class MachineBuildResult
{
    private MachineBuildResult(EnigmaSession? session, string? error, Exception? fault)
    {
        Session = session;
        Error = error;
        Fault = fault;
    }

    public EnigmaSession? Session { get; }

    /// <summary>What was wrong with the key sheet, in words fit to show an operator.</summary>
    public string? Error { get; }

    /// <summary>
    /// The fault behind <see cref="Error"/>, for a diagnostic log. A front end shows
    /// the message; only a trace wants the rest.
    /// </summary>
    public Exception? Fault { get; }

    public bool Succeeded => Session is not null;

    internal static MachineBuildResult Keyed(EnigmaSession session) => new(session, null, null);

    internal static MachineBuildResult Rejected(Exception fault) => new(null, fault.Message, fault);
}
