namespace Enigma.Rotors;

/// <summary>
/// Wheel I of the numbers-only Enigma Z30, from the Swedish machine s/n Z-103.
/// Ten contacts, and one notch: the wheel to its left is carried as 9 leaves the
/// window.
/// </summary>
public class RotorZI : RotorBase
{
    // Published indexed 1 to 0; written here 0 to 9, which is the order this
    // machine's alphabet runs in.
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("9641827035", CharacterMap.Digits);

    private static readonly int[] Turnover = WiringTable.Notches("9", CharacterMap.Digits);

    // "The notch is attached to the rotor body, which means that altering the
    // Ringstellung does not alter its position with respect to the wiring... this is
    // identical to the rotors of Enigma D, but different from the rotors of later
    // machines like Enigma K and Enigma I where the notch is attached to the index
    // ring." -- Crypto Museum, Enigma Z.
    protected override bool NotchOnTheIndexRing => false;

    public override string Name => "Z-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
