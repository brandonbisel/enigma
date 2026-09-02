namespace Enigma;

/// <summary>
/// The Enigma Uhr (Schaltuhr), a box that replaced the plugboard cables with a
/// forty position switch. Exactly ten cables plugged into it, in the order the key
/// sheet gave them, and turning the dial selected one of forty fixed scramblings.
///
/// Its cryptographic point is that it is <em>not</em> a set of paired cables. An
/// ordinary board joins A to V and V back to A; the Uhr may send A to V while V
/// goes somewhere else entirely. The machine stays reciprocal regardless, because
/// the current passes through the board twice, once each way, so only the
/// reflector has to be its own inverse.
///
/// The dial had a documented flaw: because the b wires were paired rather than
/// fully scrambled, every fourth position is reciprocal after all, and so gives
/// away the advantage the box was fitted for. <see cref="IsReciprocalAt"/> reports
/// that, and a test asserts it holds for exactly positions 0, 4, 8 and so on.
/// </summary>
public class EnigmaUhr : IPlugBoard
{
    public const int Cables = 10;
    public const int Positions = 40;

    // Which cable's b plug each cable's a plug reaches, per dial position, and the
    // same the other way. Derived from published test vectors covering all forty
    // positions, and checked against a second vector with a different plug set.
    // See docs/sources.md for where they come from.
    private static readonly string[] AToB =
    [
        "0123456789",  // 00
        "2387460915",
        "6549873012",
        "4839152670",
        "5498730126",  // 04
        "9108362574",
        "7821096543",
        "1925743068",
        "8210965437",  // 08
        "5619347082",
        "1456237890",
        "2470896315",
        "4562378901",  // 12
        "3529806147",
        "8734901265",
        "8061239574",
        "7349012658",  // 16
        "7421635809",
        "0982654371",
        "6354927081",
        "9826543710",  // 20
        "8453971620",
        "2143789056",
        "9782406153",
        "1437890562",  // 24
        "8792053461",
        "5890126734",
        "0148635792",
        "8901267345",  // 28
        "0246798351",
        "1265430987",
        "5813970246",
        "2654309871",  // 32
        "4830219756",
        "4378962105",
        "1592064837",
        "3789621054",  // 36
        "1964520738",
        "9012345678",
        "7246381905"
    ];

    private static readonly string[] BToA =
    [
        "0123456789",  // 00
        "7503182946",
        "9012345678",
        "5790384216",
        "6785109432",  // 04
        "8351946702",
        "5674098321",
        "1053864729",
        "3218765904",  // 08
        "6915427380",
        "2107654893",
        "6183490572",
        "8934012567",  // 12
        "2796085134",
        "7823901456",
        "4619705832",
        "4561287093",  // 16
        "0234861795",
        "3450176982",
        "7214053968",
        "9826543710",  // 20
        "6840219573",
        "8715432609",
        "8572016493",
        "6092178345",  // 24
        "4628375019",
        "5981067234",
        "3825647091",
        "2347895601",  // 28
        "9084153627",
        "1236784590",
        "9806172354",
        "5904321876",  // 32
        "5162798403",
        "4893210765",
        "2431978605",
        "7650984123",  // 36
        "3927504861",
        "6549873012",
        "5947231680"
    ];

    private readonly int[] _forward;
    private readonly int[] _reverse;
    private readonly List<Tuple<int, int>> _cables;

    /// <param name="cables">
    /// The ten cables in the order the key sheet gives them: the first is 1a-1b,
    /// the second 2a-2b, and so on. The order is part of the setting.
    /// </param>
    public EnigmaUhr(ICharacterMap characterMap, IEnumerable<Tuple<int, int>> cables, int position)
    {
        _cables = cables.ToList();

        if (_cables.Count != Cables)
        {
            throw new ArgumentException(
                $"The Uhr takes exactly {Cables} cables, but {_cables.Count} were given.", nameof(cables));
        }

        if (position is < 0 or >= Positions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position), position,
                $"The Uhr dial has {Positions} positions, numbered 0 to {Positions - 1}.");
        }

        if (characterMap.Count < Cables * 2)
        {
            throw new ArgumentException(
                $"The Uhr needs {Cables * 2} contacts for its cables, but the " +
                $"{characterMap.Name} alphabet has {characterMap.Count}.",
                nameof(characterMap));
        }

        Position = position;

        _forward = Enumerable.Range(0, characterMap.Count).ToArray();

        for (var cable = 0; cable < Cables; cable++)
        {
            var (a, b) = (_cables[cable].Item1, _cables[cable].Item2);

            _forward[a] = _cables[AToB[position][cable] - '0'].Item2;
            _forward[b] = _cables[BToA[position][cable] - '0'].Item1;
        }

        _reverse = new int[_forward.Length];

        for (var letter = 0; letter < _forward.Length; letter++)
        {
            _reverse[_forward[letter]] = letter;
        }
    }

    public int Position { get; }

    /// <summary>
    /// True where the dial happens to give a reciprocal substitution, which is
    /// every fourth position and the weakness in the device.
    /// </summary>
    public static bool IsReciprocalAt(int position) => position % 4 == 0;

    public int Translate(int input) => _forward[input];

    public int TranslateReverse(int input) => _reverse[input];

    public void Connect(int input, int output) =>
        throw new NotSupportedException("The Uhr is wired by its cables and dial, not by patching.");

    public void Disconnect(int input, int output) =>
        throw new NotSupportedException("The Uhr is wired by its cables and dial, not by patching.");

    public bool IsConnected(int input, int output) => _forward[input] == output;

    public IEnumerable<Tuple<int, int>> GetConnections() => _cables;
}
