using Enigma.Models;

namespace Enigma;

/// <summary>
/// The Kriegsmarine's way of telling the receiving station where to start, which is
/// not the Army's.
///
/// The Army sent its ground setting in clear. The Navy sent nothing in clear at all:
/// the operator took two trigrams from the Kenngruppenbuch — a Schlüsselkenngruppe
/// saying which key was in force, and a Verfahrenkenngruppe which, typed at the
/// day's Grundstellung, gave him the message key — and then hid both under a
/// double-letter conversion table before transmitting them.
/// </summary>
public interface INavalIndicatorProcedure
{
    /// <summary>
    /// Prepares a message. The Verfahrenkenngruppe is typed at the day's ground
    /// setting to give the message key, and the two trigrams are padded and
    /// substituted into the eight letters that travel with the message.
    /// </summary>
    NavalIndicator Send(
        KeySheet dailyKey,
        BigramTable table,
        string keyGroup,
        string messageGroup,
        char firstFiller = 'X',
        char lastFiller = 'X');

    /// <summary>Reads an eight letter indicator back into its trigrams and message key.</summary>
    NavalIndicator Receive(KeySheet dailyKey, BigramTable table, string indicator);
}

/// <summary>
/// What travels with a naval message, and what the operator does with it.
/// </summary>
/// <param name="KeyGroup">
/// The Schlüsselkenngruppe, saying which key was used. It selects the key sheet
/// rather than the rotor positions, so nothing here enciphers with it.
/// </param>
/// <param name="MessageGroup">
/// The Verfahrenkenngruppe, which becomes the message key when typed at the
/// ground setting.
/// </param>
/// <param name="MessageKey">
/// Where the rotors are set for the message itself. On an M4 this is four letters:
/// the trigram sets the three right-hand wheels and the Greek wheel stays at the
/// position the key sheet gave it.
/// </param>
/// <param name="Indicator">The eight letters transmitted, after substitution.</param>
public sealed record NavalIndicator(
    string KeyGroup,
    string MessageGroup,
    string MessageKey,
    string Indicator);
