namespace Enigma.Cmd;

/// <summary>
/// The parts of the command line that shape how the console app reads, writes and
/// keys a message, as opposed to how the machine itself is put together.
/// </summary>
/// <param name="Table">
/// The Doppelbuchstabentauschtafel, when the naval procedure is in use. Its presence
/// is what makes the procedure naval: the Navy's indicator cannot be worked without
/// a table, and the Army's never needs one.
/// </param>
/// <param name="KeyGroup">The Schlüsselkenngruppe, when sending the naval way.</param>
/// <param name="MessageGroup">The Verfahrenkenngruppe, when sending the naval way.</param>
public record ConsoleOptions(
    FileInfo? Input,
    FileInfo? Output,
    string? MessageKey = null,
    string? Indicator = null,
    bool Doubled = false,
    bool Prepare = false,
    int? Groups = null,
    BigramTable? Table = null,
    string? KeyGroup = null,
    string? MessageGroup = null,
    char FirstFiller = 'X',
    char LastFiller = 'X')
{
    // A banner would corrupt piped ciphertext or a file, so it is only shown when
    // a person is plainly sitting at the terminal.
    public bool ShowBanner =>
        Input is null && Output is null && !Console.IsInputRedirected && !Console.IsOutputRedirected;
}
