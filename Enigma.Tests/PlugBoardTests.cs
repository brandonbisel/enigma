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
    public void GetConnectionsReportsOnlyPatchedPairs()
    {
        var board = BuildPlugBoard();

        board.Connect(0, 25);
        board.Connect(1, 2);

        var connections = board.GetConnections().ToList();

        Assert.Equal(4, connections.Count); // Both directions of both pairs.
        Assert.Contains(Tuple.Create(0, 25), connections);
        Assert.Contains(Tuple.Create(25, 0), connections);
    }
}
