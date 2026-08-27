namespace Enigma.Tests;

public class PlugBoardTests
{
    private static PlugBoard BuildPlugBoard() => new(new DefaultCharacterMap());

    [Fact]
    public void AnUnpatchedBoardPassesEveryLetterStraightThrough()
    {
        var board = BuildPlugBoard();

        Assert.All(Enumerable.Range(0, 26), letter => Assert.Equal(letter, board.Translate(letter)));
        Assert.Empty(board.GetConnections());
    }

    [Fact]
    public void ConnectSwapsBothDirections()
    {
        var board = BuildPlugBoard();

        board.Connect(0, 25);

        Assert.Equal(25, board.Translate(0));
        Assert.Equal(0, board.Translate(25));
        Assert.True(board.IsConnected(0, 25));
        Assert.True(board.IsConnected(25, 0));
    }

    [Fact]
    public void DisconnectRestoresBothLetters()
    {
        var board = BuildPlugBoard();

        board.Connect(0, 25);
        board.Disconnect(0, 25);

        Assert.Equal(0, board.Translate(0));
        Assert.Equal(25, board.Translate(25));
        Assert.False(board.IsConnected(0, 25));
        Assert.Empty(board.GetConnections());
    }

    [Fact]
    public void DisconnectingAPairThatWasNeverCabledIsRefused()
    {
        // Silently resetting them would strand the letters they are really joined
        // to, leaving two contacts pointing at the same letter.
        var board = BuildPlugBoard();

        board.Connect(0, 2);

        Assert.Throws<ArgumentException>(() => board.Disconnect(0, 1));
        Assert.Equal(2, board.Translate(0));
        Assert.Equal(0, board.Translate(2));
    }

    [Fact]
    public void TheBoardIsAlwaysItsOwnInverse()
    {
        var board = BuildPlugBoard();

        board.Connect(0, 25);
        board.Connect(1, 2);
        board.Disconnect(1, 2);

        for (var letter = 0; letter < 26; letter++)
        {
            Assert.Equal(letter, board.Translate(board.Translate(letter)));
        }
    }

    [Fact]
    public void GetConnectionsReportsOnlyPatchedPairs()
    {
        var board = BuildPlugBoard();

        board.Connect(0, 25);
        board.Connect(1, 2);

        var connections = board.GetConnections().ToList();

        // Two cables, each reported once rather than once per end.
        Assert.Equal(2, connections.Count);
        Assert.Contains(Tuple.Create(0, 25), connections);
        Assert.Contains(Tuple.Create(1, 2), connections);
    }
}
