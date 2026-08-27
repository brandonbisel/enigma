# Enigma

A simulator of the Wehrmacht and Kriegsmarine Enigma machines, written in C# on .NET 10.

It models the machine as it actually worked: a plugboard, a set of rotors that step
as you type, and a reflector that sends the current back through the rotors in
reverse. Both the three-rotor Wehrmacht and Kriegsmarine machines and the
four-rotor naval M4 are supported. Because the reflector never maps a letter to itself, the machine is
reciprocal — the same settings both encipher and decipher, which is exactly the
property that made it breakable.

## Layout

| Project | What it is |
|---|---|
| `Enigma` | The library: rotors, reflectors, plugboard, machine, and DI wireup |
| `Enigma.Cmd` | A console app, installable as a tool named `enigma` |
| `Enigma.Tests` | xUnit tests, including a differential suite against an independent oracle |

## Building and running

```bash
dotnet build
dotnet test
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd
```

## Command line

```
-s, --settings <file>   JSON file describing the machine
-i, --input <file>      Read the message from this file instead of standard input
-o, --output <file>     Write the result to this file instead of standard output
-v, --verbose           Trace every character through the plugboard, rotors and reflector
    --log-file <file>   Also write the diagnostic log to this file
    --init-settings <file>
                        Write a settings file holding the default machine, then exit
-?, -h, --help          Show help
```

With no `--input`, it reads standard input line by line. The rotors keep stepping
across lines, exactly as the real machine does — running the program again is what
returns them to their configured starting position. So enciphering and deciphering
are two separate runs with the same settings:

```bash
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd   # BZHGNOCRRTCM
echo "BZHGNOCRRTCM" | dotnet run --project Enigma.Cmd   # ATTACKATDAWN
```

Diagnostics go to standard error and enciphered text to standard output, so piping
and redirection stay clean.

Individual settings can be overridden with configuration keys, which compose with
the named options:

```bash
echo "AAAAA" | dotnet run --project Enigma.Cmd -- --KeySheet:Reflector=C
```

## Key sheets

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

### Packaged key sheets

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

Decrypting the Barbarossa intercept:

```bash
$ echo "EDPUDNRGYSZRCXNUYTPOMRMBOFKTBZREZKMLXLVEFGUEY..." \
    | dotnet run --project Enigma.Cmd -- --preset barbarossa
AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZ...
```

That is *Aufklärungsabteilung von Kurtinowa nordwestlich Sebez* — a reconnaissance
report, with `X` for spaces and `Q` for *ch*.

## The indicator procedure

A key sheet fixed the wheel order, ring settings and plugboard for the day, but not
where the rotors started. The sender chose that himself: he set the rotors to a
**ground setting** (*Grundstellung*), enciphered his chosen **message key**
(*Spruchschlüssel*) at it, and transmitted the ground setting in clear together
with the enciphered result — the **indicator**. The receiver set the same ground
setting, deciphered the indicator to recover the message key, and set his rotors
to it.

With `--message-key` the key sheet's positions are taken as the ground setting,
and the indicator is reported on standard error so it does not contaminate the
ciphertext:

```bash
$ echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd -- --message-key RTZ
Ground setting AAA, indicator VZA
YDFDLZKIPIQE
```

The receiver needs only the day's key sheet, the ground setting and that indicator:

```bash
$ echo "YDFDLZKIPIQE" | dotnet run --project Enigma.Cmd -- --indicator VZA
ATTACKATDAWN
```

Until 1938 the message key was sent **twice**, so the receiver could tell a
garbled indicator from a good one. `--doubled` does that, and a six-letter
indicator whose halves disagree is rejected on the way back. The two halves
encipher differently because the rotors move between them — the repetition that
gave Rejewski the relation he used to reconstruct the wiring.

## Writing a message out

An Enigma has twenty six letter keys, no space bar and no digits, so a signaller
fitted his text to the machine before typing it. `--prepare` does the same:
umlauts expanded, the sharp s doubled, digits spelled out in German, spaces
written as `X`, and everything else dropped. `--groups` breaks the result into the
groups it was transmitted in, five letters at a time by convention, so that a
miscount shows up at the far end.

```bash
$ echo "Angriff um 06:30 Uhr, Größe 12 Männer" | enigma --prepare --groups 5
BQIUD QDAPV HMVYV DTYJT FQTUS HMOEC QJPTL BSJZT AURHZ ISDUH RHJCY H
```

Deciphering ignores the grouping, and gives back the prepared text:

```
ANGRIFFXUMXNULLSECHSDREINULLXUHRXGROESSEXEINSZWOXMAENNER
```

Operators also wrote *ch* as `Q`, which is why the intercepts read `BEOBAQTET` and
`RIQTUNG`. That is deliberately **not** applied: it was a habit rather than a rule,
it was not universal, and it would quietly rewrite any word containing those two
letters.

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
cryptographic significance is that the plugboard stopped being a set of pairs: an
ordinary board joins A to V and V back to A, while under the Uhr A may go to V
with V going somewhere else entirely.

That part **is** modelled. `SubstitutionPlugBoard` takes any permutation, and the
machine stays reciprocal because the return leg runs the substitution backwards
rather than forwards. It is worth knowing why that works: reciprocity requires the
*reflector* to be paired, and never required it of the plugboard — the ordinary
board simply happens to be its own inverse.

What is **not** modelled is the dial. Turning it selected one of forty fixed
scramblings of the ten cables, and this implementation has no verified source for
that table, nor for which position reproduces a plain board. Rather than ship a
plausible guess, the substitution is taken directly. If you know the machine's
setting, you can express it; you cannot yet say "cables X, dial 27".

## The entry wheel

The entry wheel (*Eintrittswalze*) is the fixed stator between the plugboard and
the first rotor, deciding which contact each key is wired to. Every service Enigma
wired it straight through in alphabet order, so it changes nothing and needs no
mention. The commercial and railway machines wired it in **keyboard** order
instead, which is why their traffic could not be read on a service machine even
with identical wheels.

Name it on the key sheet with `"EntryWheel": "QWERTZ"`, or define your own in a
parts file. A definition gives the keyboard in the order its keys are wired to
contacts, which is the form the wiring is published in:

```json
{ "EntryWheels": [ { "Name": "Railway", "Keyboard": "QWERTZUIOASDFGHJKPYXCVBNML" } ] }
```

So `Q` is wired to the first contact, `W` to the second, and a wheel reading
`ABCDEFGHIJKLMNOPQRSTUVWXYZ` is wired straight through.

## Custom alphabets

Every service Enigma worked in the twenty six capital letters, but nothing in the
mechanism requires that. A parts file can define an alphabet, and the machine is
then built entirely in it — the wiring, the notches, the ring settings, the
plugboard pairs and the message itself:

```json
{
  "CharacterMaps": [ { "Name": "Digits", "Characters": "0123456789" } ],
  "Rotors": [
    { "Name": "D-I",   "Wiring": "1357902468", "Notches": "4" },
    { "Name": "D-II",  "Wiring": "2468013579", "Notches": "2" },
    { "Name": "D-III", "Wiring": "3052749618", "Notches": "7" }
  ],
  "Reflectors": [ { "Name": "D-UKW", "Wiring": "5678901234" } ]
}
```

Name it from the key sheet with `"CharacterMap": "Digits"`. Parts in the file take
that alphabet automatically when the file defines exactly one; otherwise each names
its own.

The alphabet is the **single authority** for how many contacts a machine has. A
wheel wired for a different one is refused by name rather than failing later with
an index error:

```
'I' has 26 contacts, but the Digits alphabet has 10 characters.
```

Ring settings and positions are numbered within the alphabet, so a ten character
machine numbers its wheels 01 to 10. A reflector needs an even number of
characters, since it wires them in pairs.

Two things stay Latin. `--prepare` expands German umlauts and spells out German
numerals, which only means anything in that alphabet. And the `QWERTZ` entry wheel
is a fixed twenty six key layout; the straight-through one exists for any alphabet,
being the identity.

## Custom rotors and reflectors

A parts file defines wheels and reflectors the library does not ship with, which
is enough to run machines it knows nothing about:

```json
{
  "Rotors": [
    { "Name": "K-I",  "Wiring": "PEZUOHXSCVFMTBGLRINQJWAYDK", "Notches": "Y" },
    { "Name": "K-II", "Wiring": "ZOUESYDKFWPCIQXHMVBLGNJRAT", "Notches": "E" }
  ],
  "Reflectors": [
    { "Name": "UKW-K", "Wiring": "IMETCGFRAYSQBZXWLHKDVUPOJN" }
  ]
}
```

`Notches` lists the letters showing in the window when the wheel turns the one to
its left. Add `"Thin": true` for a half-width wheel, which then takes no notches.
Definitions are checked when the file is read, so a wiring that is not a
permutation, or a reflector that is not paired, is reported before any enciphering
starts.

The file is laid over the built-in parts rather than replacing them, so it need
only define what is missing. A definition whose name matches a built-in part
**takes its place**, which is what makes it possible to model a machine whose
wheels happen to share our names — the substitution is reported as a warning,
because ciphertext produced that way cannot be reproduced without the same file.

## Using the library

```csharp
services.AddEnigmaServices();

var machine = factory.Create(keySheet);
var cipher = machine.Translate(plaintextIndices);
```

`AddEnigmaServices` registers rotors and reflectors as keyed services under the
names used on a key sheet. Rotors and plugboards are **transient** because they
carry state — two machines sharing a rotor instance would silently corrupt each
other's positions. `IEnigmaMachineFactory` builds a configured machine, which is
what selects three rotors out of eight and applies their positions and ring
settings.

## Watching it work

`--verbose` traces each character through every component, with the rotor window
alongside. Primed rotor names are the return leg, after the reflector.

```
A -> B   window AAB   plug A>A | III A>C | II C>D | I D>F | B F>S | I' S>S | II' S>E | III' E>B | plug B>B
```

## Testing

The suite covers published test vectors, the reciprocity and no-self-encipherment
invariants, rotor stepping including the double step, and the DI lifetimes.

Alongside it sits an independent implementation in `Enigma.Tests/Reference`, written
from the mechanical description of the machine rather than ported from the library,
and anchored on the published vectors. Randomised configurations are run through
both and compared, which covers ground fixed vectors cannot: a bug that reads the
notch from the ring-adjusted offset rather than the window position, for instance,
is invisible to every vector taken at ring setting zero.
