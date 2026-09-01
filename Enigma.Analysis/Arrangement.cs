namespace Enigma.Analysis;

/// <summary>
/// One way the wheels could have been put in the machine: a rotor order, left to
/// right as a key sheet writes it, and the reflector behind them. It is a
/// possibility rather than a setting — where the wheels are turned to is what the
/// search is looking for.
/// </summary>
public sealed record Arrangement(IReadOnlyList<string> Wheels, string Reflector)
{
    public override string ToString() => $"{string.Join(" ", Wheels)} / {Reflector}";
}
