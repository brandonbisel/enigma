namespace Enigma.App;

/// <summary>
/// What a fault says to whoever is working the machine.
///
/// The library raises exceptions whose messages are written to be read — a wheel
/// that does not exist, a cable given twice, a Kenngruppe that is not a trigram.
/// <see cref="ArgumentException"/> then appends the parameter it was thrown for,
/// which is a note to whoever called the method and names an argument no operator
/// passed. In a trimmed WebAssembly build the resource strings are gone and that
/// note arrives as its own key — "Arg_ParamName_Name, keyGroup" — so it reached the
/// panel as neither English nor German. Only the sentence is kept.
/// </summary>
internal static class OperatorMessage
{
    public static string Of(Exception fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        if (fault is not ArgumentException { ParamName: { Length: > 0 } parameter })
        {
            return fault.Message;
        }

        var note = Note(parameter);

        return fault.Message.EndsWith(note, StringComparison.Ordinal)
            ? fault.Message[..^note.Length].TrimEnd()
            : fault.Message;
    }

    /// <summary>
    /// The note the framework appends, asked for rather than matched against: an
    /// exception carrying nothing but the parameter renders the default sentence
    /// and the note, so what the default does not account for is the note itself.
    /// Written this way it holds however the runtime words it, or fails to.
    /// </summary>
    private static string Note(string parameter) =>
        new ArgumentException(null, parameter).Message[new ArgumentException().Message.Length..];
}
