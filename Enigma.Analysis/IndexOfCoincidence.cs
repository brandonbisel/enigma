namespace Enigma.Analysis;

/// <summary>
/// How often two characters drawn from the text at random turn out to be the same.
/// German runs at about 0.0762 and text with no structure at 1/26, near 0.0385, so
/// a wheel setting that is even roughly right stands above one that is wrong.
///
/// The reason it works against a machine with a plugboard is that it counts
/// agreements rather than letters: relabelling the alphabet moves every count to a
/// different letter and changes none of them. A plugboard is exactly such a
/// relabelling, so a decipherment at the right wheel setting but with no cables in
/// still shows German's rate of coincidence and not a random one. That is what lets
/// the wheels be recovered first and the board be worried about afterwards.
///
/// It needs no corpus, which is the whole of why it is the measure this library
/// starts with: nothing here has to be sourced, because nothing here is data.
/// </summary>
public sealed class IndexOfCoincidence : IScore
{
    private readonly int _symbols;

    public IndexOfCoincidence(ICharacterMap? alphabet = null)
    {
        _symbols = (alphabet ?? CharacterMap.Latin).Count;
    }

    public string Name => "Index of coincidence";

    public double Of(ReadOnlySpan<int> text)
    {
        // Two characters cannot agree if there are not two of them.
        if (text.Length < 2)
        {
            return 0;
        }

        Span<int> counts = stackalloc int[_symbols];

        foreach (var contact in text)
        {
            counts[contact]++;
        }

        var coincidences = 0L;

        foreach (var count in counts)
        {
            coincidences += (long)count * (count - 1);
        }

        return (double)coincidences / ((long)text.Length * (text.Length - 1));
    }
}
