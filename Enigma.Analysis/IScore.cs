namespace Enigma.Analysis;

/// <summary>
/// How like plaintext a candidate decipherment looks. Higher is better, and the
/// scale is the measure's own — a score is only ever compared against another score
/// from the same measure.
///
/// This is an interface because the measure is the part of an attack most likely to
/// be replaced. The index of coincidence needs no language data at all, which is why
/// it is what ships here; anything stronger is a table of German statistics, and a
/// table is data and falls under the sourcing rule the rest of this repository is
/// held to. Swapping the measure must not mean rewriting the search.
/// </summary>
public interface IScore
{
    /// <summary>What the measure is called, for a front end that reports it.</summary>
    string Name { get; }

    /// <summary>
    /// Scores one candidate, given as contacts rather than characters. Called from
    /// every thread of a search, once per setting, so an implementation must keep no
    /// state between calls.
    /// </summary>
    double Of(ReadOnlySpan<int> text);
}
