using Enigma.Models;

namespace Enigma.Analysis;

/// <summary>
/// A setting the search thinks worth reporting, and what it scored.
///
/// The setting is a <see cref="KeySheet"/> because that is the only settings
/// representation in this library, and because it means an answer can be handed
/// straight back to the factory and run. What it is not is <em>the</em> answer: a
/// message pins the wheels only as far as it is long, and settings that differ on
/// paper can be one machine. Judging that is <see cref="SettingsEquivalence"/>.
/// </summary>
public sealed record Candidate(KeySheet Settings, double Score)
{
    public override string ToString() =>
        $"{Settings.Rotors,-16} {Settings.Reflector,-7} " +
        $"Ringstellung {Settings.RingSettings}  Grundstellung {Settings.Positions}  {Score:F5}";
}
