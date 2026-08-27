namespace Enigma;

public class PlugBoard : PlugBoardBase
{
    private readonly IDictionary<int, int> _wiring;

    public PlugBoard(ICharacterMap characterMap)
    {
        // An unpatched board passes every letter straight through.
        _wiring = new Dictionary<int, int>(characterMap.Count);

        for (var i = 0; i < characterMap.Count; i++)
        {
            _wiring[i] = i;
        }
    }

    protected override IDictionary<int, int> Wiring => _wiring;
}
