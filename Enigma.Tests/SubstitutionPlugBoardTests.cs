using Enigma.Reflectors;
using Enigma.Rotors;

namespace Enigma.Tests;

public class SubstitutionPlugBoardTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    [Fact]
    public void ACabledBoardIsItsOwnInverse()
    {
        var board = new PlugBoard(CharacterMap);
        board.Connect(0, 21);

        Assert.Equal(board.Translate(0), board.TranslateReverse(0));
        Assert.Equal(21, board.TranslateReverse(0));
    }

    [Fact]
    public void ASubstitutionBoardNeedNotBeItsOwnInverse()
    {
        // A goes to B, but B does not come back to A: exactly what the Uhr did to
        // the plugboard, and impossible with cables.
        var substitution = Enumerable.Range(0, 26).ToArray();
        (substitution[0], substitution[1], substitution[2]) = (1, 2, 0);

        var board = new SubstitutionPlugBoard(substitution);

        Assert.Equal(1, board.Translate(0));
        Assert.NotEqual(0, board.Translate(1));
        Assert.Equal(0, board.TranslateReverse(1));
    }

    [Fact]
    public void ASubstitutionMustBeAPermutation()
    {
        var substitution = Enumerable.Range(0, 26).ToArray();
        substitution[1] = 0; // two letters now arrive at A

        Assert.Throws<ArgumentException>(() => new SubstitutionPlugBoard(substitution));
    }

    [Fact]
    public void ABoardBuiltFromCablesMatchesTheCabledBoard()
    {
        var cables = new[] { Tuple.Create(0, 21), Tuple.Create(1, 18) };

        var cabled = new PlugBoard(CharacterMap);
        cabled.Connect(0, 21);
        cabled.Connect(1, 18);

        var substitution = SubstitutionPlugBoard.FromCables(CharacterMap, cables);

        Assert.All(
            Enumerable.Range(0, 26),
            letter => Assert.Equal(cabled.Translate(letter), substitution.Translate(letter)));
    }

    [Fact]
    public void AMachineWithANonPairedBoardIsStillReciprocal()
    {
        // Reciprocity needs the reflector to be paired, not the plugboard: the board
        // is applied one way in and the other way out.
        var substitution = Enumerable.Range(0, 26).ToArray();
        (substitution[0], substitution[1], substitution[2]) = (1, 2, 0);

        var cipher = Encipher(new SubstitutionPlugBoard(substitution), "ATTACKATDAWN");
        var back = Encipher(new SubstitutionPlugBoard(substitution), cipher);

        Assert.NotEqual("ATTACKATDAWN", cipher);
        Assert.Equal("ATTACKATDAWN", back);
    }

    [Fact]
    public void PatchingASubstitutionBoardIsRefused()
    {
        var board = new SubstitutionPlugBoard(Enumerable.Range(0, 26).ToArray());

        Assert.Throws<NotSupportedException>(() => board.Connect(0, 1));
        Assert.Throws<NotSupportedException>(() => board.Disconnect(0, 1));
    }

    private static string Encipher(IPlugBoard board, string text)
    {
        var machine = new EnigmaMachine(
            board, [new RotorI(), new RotorII(), new RotorIII()], new ReflectorB());

        return string.Concat(machine.Translate(text.Select(CharacterMap.GetIndex))
            .Select(CharacterMap.GetCharacter));
    }
}
