namespace Enigma.Models;

/// <summary>
/// Wheels and reflectors defined outside the library, so that machines the
/// library does not ship with can still be run.
/// </summary>
public class PartsFile
{
    public IList<RotorDefinition> Rotors { get; set; } = [];
    public IList<ReflectorDefinition> Reflectors { get; set; } = [];
    public IList<EntryWheelDefinition> EntryWheels { get; set; } = [];
}

public class EntryWheelDefinition
{
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The keyboard in the order its keys are wired to contacts, so
    /// "QWERTZUIOASDFGHJKPYXCVBNML" is the keyboard read left to right and
    /// "ABCDEFGHIJKLMNOPQRSTUVWXYZ" is a wheel wired straight through.
    /// </summary>
    public string Keyboard { get; set; } = string.Empty;
}

public class RotorDefinition
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Wiring in the usual notation, e.g. "EKMFLGDQVZNTOWYHXUSPAIBRCJ".</summary>
    public string Wiring { get; set; } = string.Empty;

    /// <summary>
    /// The letters showing in the window when this wheel turns the one to its
    /// left, e.g. "Q", or "MZ" for the two-notch naval wheels. Empty for a thin
    /// wheel, which has no notch.
    /// </summary>
    public string Notches { get; set; } = string.Empty;

    /// <summary>True for a half-width wheel, which cannot be driven and never steps.</summary>
    public bool Thin { get; set; }
}

public class ReflectorDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Wiring { get; set; } = string.Empty;

    /// <summary>True for a narrow reflector, which leaves room for a fourth rotor.</summary>
    public bool Thin { get; set; }
}
