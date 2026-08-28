namespace Enigma.Rotors;

/// <summary>
/// The three wheel wirings the commercial machines shared. The A28/G31, the
/// Enigma D and the Enigma K were all fitted with these same wheels: what tells
/// the machines apart is where the notches sit, what the notches are cut into, and
/// how the wheels are driven — not how they are wired.
///
/// Written once so that six wheels cannot drift apart, in the way the shared
/// modulo arithmetic is written once.
/// </summary>
internal static class CommercialWiring
{
    internal const string First = "LPGSZMHAEOQKVXRFYBUTNICJDW";
    internal const string Second = "SLVGBTFXJQOHEWIRZYAMKPCNDU";
    internal const string Third = "CJGDPSHKTURAWZXFMYNQOBVLIE";
}
