using Enigma.Models;

namespace Enigma.App;

/// <summary>
/// Working out where the rotors actually start.
///
/// A key sheet fixed the wheel order, the rings and the board for the day, but not
/// where the rotors began. The sender chose that himself: he set them to the
/// <em>ground setting</em> the sheet gave (Grundstellung), enciphered his own
/// <em>message key</em> (Spruchschlüssel) at it, and sent the result in clear as
/// the indicator. The receiver did the same in reverse.
///
/// So with a procedure in use the sheet's positions are the ground setting, and
/// the machine is keyed at the message key instead. Faults come back as messages
/// rather than exceptions, because mistyping an indicator is an ordinary thing for
/// an operator to do.
/// </summary>
public static class MessageKeying
{
    /// <summary>
    /// Sending: enciphers a chosen message key at the sheet's ground setting and
    /// keys the machine at that message key.
    /// </summary>
    public static IndicatorResult Send(
        IIndicatorProcedure procedure, KeySheet daily, string messageKey, bool doubled = false)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);

        return Attempt(() =>
        {
            var indicator = procedure.EncipherMessageKey(daily, daily.Positions, messageKey, doubled);

            return new IndicatorResult(
                daily.WithPositions(messageKey), daily.Positions, messageKey, indicator, null);
        });
    }

    /// <summary>
    /// Receiving: recovers the message key from a transmitted indicator and keys
    /// the machine at it.
    /// </summary>
    public static IndicatorResult Receive(
        IIndicatorProcedure procedure, KeySheet daily, string indicator)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);

        return Attempt(() =>
        {
            var messageKey = procedure.RecoverMessageKey(daily, daily.Positions, indicator);

            return new IndicatorResult(
                daily.WithPositions(messageKey), daily.Positions, messageKey, indicator, null);
        });
    }

    /// <summary>
    /// Sending the naval way. Nothing goes out in clear: the Verfahrenkenngruppe is
    /// typed at the ground setting to give the message key, and both trigrams are
    /// padded and hidden under the bigram table before transmission.
    /// </summary>
    public static IndicatorResult SendNaval(
        INavalIndicatorProcedure procedure,
        KeySheet daily,
        BigramTable table,
        string keyGroup,
        string messageGroup,
        char firstFiller,
        char lastFiller)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);
        ArgumentNullException.ThrowIfNull(table);

        return Attempt(() => Keyed(
            daily,
            procedure.Send(daily, table, keyGroup, messageGroup, firstFiller, lastFiller)));
    }

    /// <summary>
    /// Receiving the naval way: the eight transmitted letters back into their
    /// trigrams, and the message key that follows from them.
    /// </summary>
    public static IndicatorResult ReceiveNaval(
        INavalIndicatorProcedure procedure, KeySheet daily, BigramTable table, string indicator)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);
        ArgumentNullException.ThrowIfNull(table);

        return Attempt(() => Keyed(daily, procedure.Receive(daily, table, indicator)));
    }

    private static IndicatorResult Keyed(KeySheet daily, NavalIndicator naval) =>
        new(daily.WithPositions(naval.MessageKey),
            daily.Positions,
            naval.MessageKey,
            naval.Indicator,
            null,
            naval.KeyGroup,
            naval.MessageGroup);

    private static IndicatorResult Attempt(Func<IndicatorResult> keying)
    {
        try
        {
            return keying();
        }
        catch (Exception exception)
            when (exception is ArgumentException or FormatException or InvalidOperationException)
        {
            return new IndicatorResult(null, null, null, null, OperatorMessage.Of(exception));
        }
    }
}

/// <summary>
/// What the indicator procedure worked out: the key sheet with the rotors moved to
/// the message key, and the pieces an operator writes down alongside the message.
/// </summary>
/// <param name="Sheet">The sheet to key the machine from, or null if it failed.</param>
/// <param name="GroundSetting">Where the rotors stood while the indicator was worked.</param>
/// <param name="MessageKey">Where the rotors start for the message itself.</param>
/// <param name="Indicator">What travels with the message, in clear.</param>
/// <param name="Error">Why it failed, in words fit to show an operator.</param>
/// <param name="KeyGroup">
/// Naval only: the Schlüsselkenngruppe, saying which key was in force. It names the
/// key sheet rather than the rotor positions, so nothing is enciphered with it.
/// </param>
/// <param name="MessageGroup">
/// Naval only: the Verfahrenkenngruppe, which becomes the message key when typed at
/// the ground setting.
/// </param>
public sealed record IndicatorResult(
    KeySheet? Sheet,
    string? GroundSetting,
    string? MessageKey,
    string? Indicator,
    string? Error,
    string? KeyGroup = null,
    string? MessageGroup = null)
{
    public bool Succeeded => Sheet is not null;

    /// <summary>Whether this came from the naval procedure rather than the army one.</summary>
    public bool IsNaval => KeyGroup is not null;
}
