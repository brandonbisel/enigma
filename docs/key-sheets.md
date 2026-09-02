# Key sheets

A key sheet is how a machine is described here, in the notation a real one used. It
is the only settings format: there is deliberately no second, index-based one. The
short version is in [the README](../README.md#key-sheets).

The machine is configured with a **key sheet**, written the way a real one was: the
rotor order (*Walzenlage*), the ring settings (*Ringstellung*), the starting
positions (*Grundstellung*) and the plugboard pairs (*Steckerverbindungen*).

```json
{
  "Name": "Wehrmacht Enigma I",
  "Reflector": "B",
  "Rotors": "I II III",
  "RingSettings": "AAA",
  "Positions": "AAA",
  "Plugboard": ""
}
```

Rotors are listed left to right as they sit in the machine, so the last is the fast
rotor. Ring settings and positions may be written as letters (`AAA`, or `A A A`) or
as the one-based numbers printed on real sheets (`01 01 01`), where 01 is A.
Plugboard pairs are letter pairs, `AV BS CG DL`, and each letter may take only one
cable. Rotors `I`–`V` are the Wehrmacht set and `VI`–`VIII` the Kriegsmarine additions.
Reflectors `A`, `B` and `C` are the full-width ones.

## The four-rotor naval M4

For the four-rotor naval **M4**, give four wheels with a thin rotor leftmost and a
thin reflector, which is the only pairing that physically fits: `Beta` or `Gamma`
with `B-Thin` ("Bruno") or `C-Thin` ("Caesar"). The thin rotor has no ratchet and
no notch, so it never steps and never drives its neighbour — it is set once and
stays put for the whole message. Combinations that could not be assembled, such as
four wheels with a full-width reflector or a thin rotor anywhere but leftmost, are
rejected.

Setting an M4's thin rotor to `A` with the `B-Thin` reflector reproduces a plain
`B` reflector exactly, which is how an M4 could exchange traffic with an M3.

```json
{
  "Name": "U-boat M4",
  "Reflector": "B-Thin",
  "Rotors": "Beta II IV I",
  "RingSettings": "AAAV",
  "Positions": "VJNA",
  "Plugboard": "AT BL DF GJ HM NW OP QY RZ VX"
}
```

## Writing one out

Have the program write one for you and edit it:

```bash
dotnet run --project Enigma.Cmd -- --init-key-sheet my-machine.json
dotnet run --project Enigma.Cmd -- --key-sheet my-machine.json
```

It will not overwrite an existing file. A key sheet file may hold the settings at
the root or wrapped in a `KeySheet` section, so a copy of `appsettings.json` works
as a starting point either way. Individual fields can be overridden from the
command line, which composes with everything else:

```bash
echo "AAAAA" | dotnet run --project Enigma.Cmd -- --KeySheet:Rotors="II IV V"
```

## Packaged key sheets

Some historical key sheets ship with the program. Each is verified in the test
suite by decrypting the published ciphertext of the real message it belongs to.

```bash
dotnet run --project Enigma.Cmd -- --list-presets
```

| Preset | Message |
|---|---|
| `default` | Wehrmacht Enigma I, everything at A |
| `barbarossa` | Operation Barbarossa, 7 July 1941 |
| `scharnhorst` | Scharnhorst, 26 December 1943 |
| `instruction-manual` | Enigma Instruction Manual, 1930 |
| `u264` | U-264 (Looks), 19 November 1942 — four-rotor M4 |
| `rasch` | U-106 (Rasch), 19 November 1942 — four-rotor M4 |
| `doenitz` | The signal naming Dönitz Hitler's successor — M4 with the thin UKW-C |
| `graf-spee` | Graf Spee, 12 December 1939 — naval M3, and eight plugs rather than ten |

Decrypting the Barbarossa intercept:

```bash
$ echo "EDPUDNRGYSZRCXNUYTPOMRMBOFKTBZREZKMLXLVEFGUEY..." \
    | dotnet run --project Enigma.Cmd -- --preset barbarossa
AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZ...
```

That is *Aufklärungsabteilung von Kurtinowa nordwestlich Sebez* — a reconnaissance
report, with `X` for spaces and `Q` for *ch*.

## See also

- [The machines](machines.md) for what `Model`, `ReflectorPosition` and
  `CharacterMap` are for, and which wheels each machine takes.
- [The plugboard, the Uhr and UKW-D](plugboard.md) for `Plugboard`, `Uhr` and
  `ReflectorPairs`.
- [Parts and alphabets](parts-and-alphabets.md) for naming a wheel the library does
  not ship with.
