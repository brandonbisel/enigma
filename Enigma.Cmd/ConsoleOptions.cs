namespace Enigma.Cmd;

/// <summary>
/// The parts of the command line that shape how the console app reads and writes,
/// as opposed to how the machine itself is configured.
/// </summary>
public record ConsoleOptions(FileInfo? Input, FileInfo? Output)
{
    // A banner would corrupt piped ciphertext or a file, so it is only shown when
    // a person is plainly sitting at the terminal.
    public bool ShowBanner =>
        Input is null && Output is null && !Console.IsInputRedirected && !Console.IsOutputRedirected;
}
