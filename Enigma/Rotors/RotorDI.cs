namespace Enigma.Rotors;

/// <summary>
/// Wheel I of the commercial Enigma D (A26) of 1926. The wiring is the one the
/// commercial machines shared; what marks a D wheel is its notch, which is cut into
/// the rotor body rather than the letter ring, and which every wheel carries at the
/// same place.
/// </summary>
public class RotorDI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString(CommercialWiring.First);

    private static readonly int[] Turnover = WiringTable.Notches("Z");

    // "The notch ring is attached to the body of the rotor (rather than to the
    // letter ring)", so the Ringstellung carries the turnover with it and adds
    // nothing to the key space. -- Crypto Museum, Enigma D.
    protected override bool NotchOnTheIndexRing => false;

    public override string Name => "D-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
