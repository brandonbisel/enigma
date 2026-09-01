namespace Enigma;

/// <summary>
/// A reflector wired in pairs like any other, but which turns. Its wiring is read
/// through the same offset a rotor uses, so setting it to A leaves it behaving
/// exactly as the fixed reflector it is wired like.
/// </summary>
public abstract class RotatingReflectorBase : ReflectorBase, IRotatingReflector
{
    public int Position { get; private set; }
    public int RingSetting { get; private set; }

    private int Offset => Wheel.Mod(Position - RingSetting, Contacts);

    public int Step()
    {
        Position = Wheel.Mod(Position + 1, Contacts);

        return Position;
    }

    public void SetPosition(int position) => Position = Wheel.Mod(position, Contacts);

    public void SetRingSetting(int ringSetting) => RingSetting = Wheel.Mod(ringSetting, Contacts);

    public override int Translate(int input) => Wheel.Translate(Table, input, Offset);
}
