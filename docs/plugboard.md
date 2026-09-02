# The plugboard, the Uhr and UKW-D

Three things a key sheet could change about the wiring itself, rather than about how
the wheels were set. The plugboard is the Army's own fitting; the Uhr replaced its
cables with a switch; UKW-D made the reflector part of the daily key.

## The plugboard

`Steckerverbindungen`, written as letter pairs — `AV BS CG DL` — each letter taking at
most one cable. The current passes through the board twice, once on the way in and
once on the way back, which is why an ordinary cabled board is its own inverse.

It sits **inside** the rotor sandwich rather than outside it, between the keyboard and
the entry wheel. That is a small-sounding fact with a large consequence for anyone
attacking the machine: pulling the cables out does not relabel the plaintext, it
shreds it. See [breaking a message](cryptanalysis.md#what-it-breaks).

A machine built without a board — a Zählwerk Enigma, an Enigma K, a Z30 — refuses a
key sheet that gives it cables, rather than quietly ignoring them.

## A reflector rewired in the field

UKW-D could be rewired by the unit, which made the reflector part of the daily key
rather than a fixed property of the machine. Give thirteen wire pairs on the key
sheet and the reflector is built from them:

```json
{
  "Name": "Luftwaffe with UKW-D",
  "Reflector": "D",
  "ReflectorPairs": "AQ BG CK DI EL FX HZ MW NV OT PU RS JY",
  "Rotors": "I II III",
  "RingSettings": "AAA",
  "Positions": "AAA"
}
```

Thirteen wires have to cover all twenty six contacts — a letter left unwired would
have nowhere to go — so anything else is refused.

Two things this does not do. The printed UKW-D settings used the wheel's own
contact lettering rather than the alphabet, and that mapping is **not** applied
here: it is a data-entry convention for which this implementation has no verified
source, so the pairs above are plain letters. And the pair that was fixed on the
real wheel is not forced, for the same reason.

## The Uhr

The Enigma Uhr replaced the plugboard cables with a forty position switch. Its
cryptographic point is that the plugboard stopped being a set of pairs: an ordinary
board joins A to V and V back to A, while under the Uhr A may go to V with V going
somewhere else entirely.

Fit one by naming the dial setting. Ten cables are required, and the order they are
written in is part of the setting, because the first pair is the box's cable 1:

```json
{
  "Rotors": "II IV V",
  "RingSettings": "BUL",
  "Positions": "BLA",
  "Plugboard": "AV BS CG DL FU HZ IN KM OW RX",
  "Uhr": "GD"
}
```

The setting may be written as the number, `"11"`, or as the two letter group read
off the plates inside the lid of the Uhr's box, `"GD"`. Those plates divide the
alphabet into four bands giving the tens digit and ten bands giving the units,
which is where the forty positions come from. Several groups stand for the same
setting, and between them the forty settings account for all 676 pairs of letters
exactly once.

```csharp
UhrSetting.Encode(11);        // "GD"
UhrSetting.Decode("GD");      // 11
UhrSetting.AllGroupsFor(11);  // GD, GE, HD, HE, ... fourteen in all
```

At position 00 the box reproduces ten ordinary patch cords exactly. The machine
stays reciprocal at every other position too, which is worth understanding: it
requires the *reflector* to be paired and never required it of the plugboard, since
the current passes through the board twice, once each way.

The device had a flaw. Because its `b` wires were paired rather than fully
scrambled, **every fourth position is reciprocal after all** — positions 0, 4, 8 and
so on give away the very advantage the box was fitted to provide.
`EnigmaUhr.IsReciprocalAt` reports it, and a test derives the same set from the
wiring rather than asserting it by hand.

## See also

- [Key sheets](key-sheets.md) for `Plugboard`, `Uhr` and `ReflectorPairs` in place.
- [The machines](machines.md) for the KD, the one model whose reflector is always a
  UKW-D.
- [Sources](sources.md) for the Uhr's forty-position table and where it was derived
  from.
