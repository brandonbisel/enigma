namespace Enigma;

public interface IRotor
{
    string Name { get; }
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
