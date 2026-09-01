namespace Enigma.Analysis;

/// <summary>
/// How far a search has got. Reported per wheel order rather than per setting: a
/// front end wants to draw a bar, not to be called a million times.
/// </summary>
public sealed record SearchProgress(long Done, long Total, Candidate? Best)
{
    public double Fraction => Total > 0 ? (double)Done / Total : 0;
}
