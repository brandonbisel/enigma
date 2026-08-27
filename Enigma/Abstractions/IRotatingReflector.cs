namespace Enigma;

/// <summary>
/// A reflector that can be set to a position and, on the Zählwerk machines, is
/// driven round during encipherment. On every other Enigma the reflector is fixed,
/// which is why <see cref="IReflector"/> alone has no position at all.
/// </summary>
public interface IRotatingReflector : IReflector
{
    int Position { get; }
    int RingSetting { get; }

    int Step();
    void SetPosition(int position);
    void SetRingSetting(int ringSetting);
}
