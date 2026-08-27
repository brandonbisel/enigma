namespace Enigma.Reflectors;

/// <summary>
/// A reflector the operator could rewire in the field, as UKW-D was: thirteen
/// removable wires, so the reflector became part of the key rather than a fixed
/// property of the machine.
///
/// The pairs here are plain letters of the alphabet. The printed UKW-D settings
/// used the wheel's own contact lettering, which is not the alphabet order, and
/// that mapping is deliberately not applied: it is a data-entry convention and
/// this implementation has no verified source for it.
/// </summary>
public class RewirableReflector : ReflectorBase
{
    private readonly IDictionary<int, int> _wiring;

    public RewirableReflector(string name, string pairs, int contacts = 26)
    {
        Name = name;
        _wiring = Wire(pairs, contacts);
    }

    public override string Name { get; }
    protected override IDictionary<int, int> Wiring => _wiring;

    private static IDictionary<int, int> Wire(string pairs, int contacts)
    {
        var wiring = new Dictionary<int, int>();

        foreach (var pair in (pairs ?? string.Empty)
                     .Split([' ', '\t', ',', '-', '/'], StringSplitOptions.RemoveEmptyEntries))
        {
            var cable = pair.ToUpperInvariant();

            if (cable.Length != 2 || !cable.All(char.IsAsciiLetterUpper))
            {
                throw new ArgumentException(
                    $"Reflector wiring '{pair}' is not a pair of letters, such as \"AQ\".", nameof(pairs));
            }

            var left = cable[0] - 'A';
            var right = cable[1] - 'A';

            if (left == right)
            {
                throw new ArgumentException(
                    $"Reflector wiring '{pair}' joins a letter to itself.", nameof(pairs));
            }

            if (!wiring.TryAdd(left, right) || !wiring.TryAdd(right, left))
            {
                throw new ArgumentException(
                    $"Reflector wiring '{pair}' reuses a letter that is already wired.", nameof(pairs));
            }
        }

        // Every contact must be wired, or a letter would have nowhere to go: a
        // reflector of this size takes exactly half that many wires.
        if (wiring.Count != contacts)
        {
            throw new ArgumentException(
                $"A rewirable reflector needs {contacts / 2} pairs covering all {contacts} letters, " +
                $"but {wiring.Count / 2} pairs covering {wiring.Count} were given.",
                nameof(pairs));
        }

        return wiring;
    }
}
