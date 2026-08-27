using Enigma.Models;

namespace Enigma;

/// <summary>
/// The procedure an operator followed to tell the receiving station which rotor
/// positions a message was enciphered at.
///
/// The key sheet fixed the wheel order, ring settings and plugboard for the day,
/// but not where the rotors started. The sender chose that himself: he set the
/// rotors to a ground setting (Grundstellung), enciphered his chosen message key
/// (Spruchschlüssel) at it, and sent the ground setting in clear along with the
/// enciphered result. The receiver set the same ground setting, deciphered the
/// indicator to recover the message key, and set his rotors to it.
/// </summary>
public interface IIndicatorProcedure
{
    /// <summary>Enciphers a message key at the ground setting, giving the indicator to transmit.</summary>
    string EncipherMessageKey(KeySheet dailyKey, string groundSetting, string messageKey, bool doubled = false);

    /// <summary>
    /// Recovers the message key from a transmitted indicator. A six letter
    /// indicator is the doubled form used until 1938, and its two halves must
    /// agree or the transmission was corrupted.
    /// </summary>
    string RecoverMessageKey(KeySheet dailyKey, string groundSetting, string indicator);
}
