namespace Enigma.Models;

/// <summary>One rotor's place in the machine, as read off a key sheet.</summary>
public record RotorPlacement(string Name, int Position, int RingSetting);
