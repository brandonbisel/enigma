namespace Enigma;

public interface IRotor
{
    string Name { get; }

    /// <summary>
    /// True for the half-width rotors of the M4. A thin rotor has no ratchet, so
    /// no pawl can drive it, and no notch, so it drives nothing either.
    /// </summary>
    bool IsThin { get; }

    int Position { get; }
    int RingSetting { get; }

    int Step();
    int GetPosition();
    void SetPosition(int position);
    void SetRingSetting(int ringSetting);
    int Translate(int input);
    int TranslateReverse(int input);

    bool IsTurnoverPosition();
    IEnumerable<int> GetTurnoverPositions();

}
