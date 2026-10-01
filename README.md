# Enigma

A simulator of the Enigma machines, written in C# on .NET 10.

It models the machine as it actually worked: a plugboard, a set of rotors that step as
you type, and a reflector that sends the current back through the rotors in reverse.
Because the reflector never maps a letter to itself, the machine is reciprocal — the
same settings both encipher and decipher, which is exactly the property that made it
breakable.

It is not one machine but a family of them. The Wehrmacht Enigma I and M3, the
four-rotor naval M4, the commercial D and K and the machines built on them, the
Abwehr's gear-driven Zählwerk G-31, and the Enigma Z30, which has no letters at all.
Every wiring, table and vector here is taken from a published source and pinned by a
test that decrypts a real message or reproduces a published vector.

| | |
|---|---|
| **How to use it** | this file, and [docs](docs/) for the long form of any part of it |
| **What is planned** | [ROADMAP.md](ROADMAP.md) |
| **How to work in the repository** | [AGENTS.md](AGENTS.md) |

## Layout

| Project | What it is |
|---|---|
| `Enigma` | The library: rotors, reflectors, plugboard, machine, and DI wireup |
| `Enigma.Analysis` | Attacks on the machine: the ciphertext-only wheel search |
| `Enigma.App` | Orchestration shared by the front ends: sessions, key sheet catalogue |
| `Enigma.Cmd` | A console app, installable as a tool named `enigma` |
| `Enigma.Web` | A browser front end: the machine as an operator's panel |
| `Enigma.Tests` | xUnit tests, including a differential suite against an independent oracle |
| `Enigma.Analysis.Tests` | Tests for the attacks, including the ones that pin their limits |
| `Enigma.Cmd.Tests` | Console app tests: argument handling and the whole path |
| `Enigma.Web.Tests` | Component tests for the panel, on bUnit |

## Building and running

```bash
dotnet build
dotnet test
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd
```

## Documentation

Each feature below is described here in a few paragraphs and in full in `docs/`.

| | |
|---|---|
| [Key sheets](docs/key-sheets.md) | The settings file, and the packaged historical sheets |
| [The machines](docs/machines.md) | Every model the library builds |
| [The plugboard, the Uhr and UKW-D](docs/plugboard.md) | What a key sheet could rewire |
| [Parts and alphabets](docs/parts-and-alphabets.md) | Wheels, stators and alphabets of your own |
| [The indicator procedures](docs/indicator-procedures.md) | How each service sent its message key |
| [The bigram tables](docs/bigram-tables.md) | The three naval sets, and how far each is trusted |
| [Writing a message out](docs/message-text.md) | The signaller's conventions |
| [The command line](docs/command-line.md) | Every option |
| [The panel](docs/panel.md) | The browser front end |
| [Using the library](docs/library.md) | The API, the trace, and the suite |
| [Breaking a message](docs/cryptanalysis.md) | The wheel search and where it stops |
| [Sources](docs/sources.md) | Where all of it came from |

## The panel

```bash
dotnet run --project Enigma.Web
```

The machine as an operator faced it: the wheel windows above, the lamps below them and
the keys below those. Press a key and the wheels turn *before* the lamp lights, which
is the order the machine works in. The letter that lights, pressed back on a machine
returned to the same setting, gives the original.

The panel is built from whatever machine the key sheet names — three windows for an
Enigma I, four for an M4, four again for a Zählwerk machine because its reflector
turns and so is part of the setting. It is labelled as the machine was, Walzenlage and
Ringstellung and Steckerbrett, with the English alongside each term. Below the windows
are the message, both indicator procedures, the key sheet and the plugboard, all of
them live: change any setting and the same message is keyed again.

Tick **Follow the current** and each keypress is set out step by step, in through the
board and the stator, right to left across the wheels, back off the reflector and out
the way it came. What is shown is the machine's own `TranslationTrace` rather than a
retelling.

It is a WebAssembly page with no server behind it: the library runs in the browser
unchanged, and nothing typed into it is transmitted anywhere.

**Full detail: [docs/panel.md](docs/panel.md).**

## The command line

```bash
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd   # BZHGNOCRRTCM
echo "BZHGNOCRRTCM" | dotnet run --project Enigma.Cmd   # ATTACKATDAWN
```

It reads a message on standard input, or from `--input`, and writes the result on
standard output. Diagnostics go to standard error, so piping stays clean. The rotors
keep stepping across lines exactly as the real machine does; running the program again
is what returns them to their starting position, which is why enciphering and
deciphering are two runs of the same settings.

`--key-sheet` names a machine and `--preset` names a packaged one. `--verbose` traces
every character through every component. `--prepare` and `--groups` handle the
signaller's conventions, `--message-key` and `--indicator` the Army's indicator
procedure and `--tafel` and `--kenngruppen` the Navy's, and `--recover` attacks the
input instead of enciphering it.

**Full detail: [docs/command-line.md](docs/command-line.md).**

## Key sheets

The machine is configured with a **key sheet**, written the way a real one was: the
rotor order (*Walzenlage*), the ring settings (*Ringstellung*), the starting positions
(*Grundstellung*) and the plugboard pairs (*Steckerverbindungen*).

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
rotor. Settings may be written as letters or as the one-based numbers printed on real
sheets. `--init-key-sheet` writes one out to edit, and individual fields can be
overridden from the command line.

Eight historical key sheets ship, each verified in the test suite by decrypting the
published ciphertext of the real message it belongs to — Barbarossa, Scharnhorst, the
Graf Spee signal, U-264, U-106, the signal naming Dönitz Hitler's successor, and the
1930 instruction manual. `--list-presets` names them.

```bash
$ echo "EDPUDNRGYSZRCXNUYTPOMRMBOFKTBZREZKMLXLVEFGUEY..." \
    | dotnet run --project Enigma.Cmd -- --preset barbarossa
AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZ...
```

**Full detail: [docs/key-sheets.md](docs/key-sheets.md).**

## The machines

A model is *parts plus a layout*: what drive it runs, what stator it takes, whether it
has a plugboard at all, and which arrangements of parts could actually have been
assembled. A key sheet names one with `Model`, and the service machines are the
default.

| Model | What separates it |
|---|---|
| Enigma I, M3 | Pawls, the double step, and the Army's plugboard |
| Naval M4 | A thin reflector frees the width for a fourth wheel, which has no ratchet and never turns |
| Norenigma | An Enigma I rewired by the Norwegians after the war |
| Enigma D, K | The commercial pattern: keyboard-order stator, a reflector set but never driven, no plugboard |
| Swiss K, Railway, Tirpitz | The same pattern with other wheels; the Tirpitz has a stator of its own |
| Enigma KD | A commercial K whose reflector is a field-rewired UKW-D |
| Zählwerk G-31 | Cogwheels rather than pawls, so no double step, and a reflector that turns |
| Enigma Z30 | Ten contacts and no letters at all, its reflector driven by an extra pawl |

The D, the K and the A28/G31 were fitted with the same three wheel wirings and the
same reflector; what separates them is entirely in where the notches are cut and what
drives them. That is the kind of detail the machine documentation exists for.

**Full detail: [docs/machines.md](docs/machines.md).**

## The indicator procedures

A key sheet fixed the wheel order, the rings and the plugboard for the day, but not
where the rotors started. The sender chose that himself, and how he told the receiver
is the indicator procedure. Both services' are here, in both front ends.

The Army enciphered its message key at a ground setting and sent the ground setting in
clear beside it:

```bash
$ echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd -- --message-key RTZ
Ground setting AAA, indicator VZA
YDFDLZKIPIQE
```

The Kriegsmarine sent nothing in clear at all. Its operator took two trigrams from the
Kenngruppenbuch, padded each with a letter, wrote them one above the other and read
downwards; the four column pairs went through a **Doppelbuchstabentauschtafel** before
transmission, so letters that were sent together had never been next to each other.

```bash
$ echo "..." | dotnet run --project Enigma.Cmd -- \
      --preset u534 --kennziffer 6 --monatstag 1 --indicator "FNHC GVET"
Tauschtafelplan Bruno, Kennziffer 6, Monatstag 1: Tafel A
Ground setting IBFK, Schlüsselkenngruppe DUZ, Verfahrenkenngruppe YMU, indicator FNHCGVET, rotors ODFF
```

That is U-534's real message of 1 May 1945, run from the transmitted indicator through
to the message key its operator wrote on his own sheet.

**Full detail: [docs/indicator-procedures.md](docs/indicator-procedures.md).**

## The bigram tables

The naval procedure runs on tables that survive only as photographs of the originals,
because the Kriegsmarine printed its cipher documents in water-soluble ink. Three of
the Crypto Museum's booklets are transcribed here and ship as data.

| Set | Booklet | Tables | Calendar |
|---|---|---|---|
| Quelle | Prüf-Nr. 2499 | 8 of 9 — A to H | front only: Kennziffer one to six |
| Meer | Prüf-Nr. 3733 | 9 — A to J without I | both sides: one to twelve |
| Flußlauf | Prüf-Nr. 3633 | 15 — A to P without I | both sides: one to twelve |

Each set carries its Tauschtafelplan, the calendar naming which table a given
Kennziffer column used on a given day, so the table is chosen the way an operator
chose it rather than by hand. A day that falls on a table no source reproduces says so
instead of failing quietly.

A transcribed table can be checked in a way a transcribed wiring cannot: it is an
involution with no fixed point, so 676 independently read cells all have to pair up,
and a mistyped letter breaks its pair and is named rather than shipped.

**Full detail: [docs/bigram-tables.md](docs/bigram-tables.md).**

## The plugboard, the Uhr and UKW-D

Three things a key sheet could change about the wiring itself. The **plugboard** is
letter pairs, `AV BS CG DL`. The **Uhr** replaced its ten cables with a forty-position
switch, so that A might go to V while V went somewhere else entirely — and its wiring
had a flaw, in that every fourth position is reciprocal after all, giving away the
advantage the box was fitted for. **UKW-D** was a reflector the unit could rewire,
which made the reflector part of the daily key rather than a property of the machine.

```json
{ "Plugboard": "AV BS CG DL FU HZ IN KM OW RX", "Uhr": "GD" }
```

**Full detail: [docs/plugboard.md](docs/plugboard.md).**

## Custom parts and alphabets

A parts file defines rotors, reflectors, entry wheels and alphabets the library does
not ship with, which is enough to run machines it knows nothing about. It is laid over
the built-in parts rather than replacing them, so it need only define what is missing.

```json
{
  "Rotors": [
    { "Name": "K-I",  "Wiring": "PEZUOHXSCVFMTBGLRINQJWAYDK", "Notches": "Y" }
  ],
  "Reflectors": [
    { "Name": "UKW-K", "Wiring": "IMETCGFRAYSQBZXWLHKDVUPOJN" }
  ]
}
```

The alphabet is a first-class part of the machine rather than an assumption: define
one and the wiring, the notches, the ring settings, the plugboard pairs and the
message are all expressed in it. A wheel wired for a different alphabet is refused by
name rather than failing later with an index error.

**Full detail: [docs/parts-and-alphabets.md](docs/parts-and-alphabets.md).**

## Writing a message out

An Enigma has twenty six letter keys, no space bar and no digits, so a signaller
fitted his text to the machine before typing it. `--prepare` does the same — umlauts
expanded, the sharp s doubled, digits spelled out in German, spaces written as `X` —
and `--groups` breaks the result into the groups it was transmitted in, so that a
miscount showed up at the far end.

```bash
$ echo "Angriff um 06:30 Uhr, Größe 12 Männer" | enigma --prepare --groups 5
BQIUD QDAPV HMVYV DTYJT FQTUS HMOEC QJPTL BSJZT AURHZ ISDUH RHJCY H
```

Operators also wrote *ch* as `Q`, which is why the intercepts read `BEOBAQTET`. That
is deliberately **not** applied: it was a habit rather than a rule, and it would
quietly rewrite any word containing those two letters.

**Full detail: [docs/message-text.md](docs/message-text.md).**

## Breaking a message

`Enigma.Analysis` attacks the machine instead of running it. Given ciphertext and
nothing else, `RotorSearch` sweeps every arrangement of the wheels at every starting
position with the board empty, scores each decipherment by its **index of
coincidence**, and lets the right setting rise; then it sweeps the ring settings that
place the turnovers, and the alignment again against those, until neither improves. It
is Gillogly's method, and it runs the real machine.

```bash
dotnet run --project Enigma.Cmd -- --key-sheet machine.json --recover \
    --wheels "K-I K-II K-III" --candidates 3 < cipher.txt
```

```
Searching 6 arrangements, 105,456 settings, over 400 letters.
 1  0.05529  rotors K-II K-III K-I, reflector G, Ringstellung ABS, Grundstellung BXT
 2  0.04397  rotors K-III K-I K-II, reflector G, Ringstellung AER, Grundstellung XKZ
 3  0.04248  rotors K-I K-II K-III, reflector G, Ringstellung ACZ, Grundstellung ETV
```

**It breaks the machines with no plugboard** — the Enigma D and K, the Swiss K, the
Railway and Tirpitz machines, the Zählwerk G-31 and the Z30. Over 24 random settings
of an unsteckered Enigma I it recovered the wheel order 15 times, and every one of
those 15 read at least 378 of the message's 400 letters. The other 9 read fewer than
30: it either breaks a message or it is nowhere near, with nothing in between.

**It does not break a steckered service Enigma, and that is a test rather than a
caveat.** The board sits inside the rotor sandwich, so running the machine without
cables does not relabel the plaintext, it shreds it. Closing that gap needs a measure
that knows what German looks like, which is a table of statistics, which is data,
which under this repository's one rule needs a published source.

**Full detail: [docs/cryptanalysis.md](docs/cryptanalysis.md).**

## Using the library

```csharp
services.AddEnigmaServices();

var machine = factory.Create(keySheet);
var cipher = machine.Translate(plaintextIndices);
```

`AddEnigmaServices` registers rotors and reflectors as keyed services under the names
used on a key sheet. To work a machine rather than build one, `Enigma.App` has a
session: a machine keyed for the day, pressed a key at a time, whose failures come
back as messages rather than exceptions because that is what a front end needs.

```csharp
var session = EnigmaSession.Open(factory, keySheet).Session!;

session.Press('A');           // the lamp that lit, or null for a key it has not got
session.WindowText;           // where the wheels stand
session.Patch('A', 'V');      // run a cable, as the board itself allows it
```

**Full detail: [docs/library.md](docs/library.md).**

## Testing

752 tests. They cover published test vectors, the reciprocity and no-self-encipherment
invariants, rotor stepping including the double step, seven historical messages
verified against their published ciphertext, and the DI lifetimes.

Alongside it sits an independent implementation in `Enigma.Tests/Reference`, written
from the mechanical description of the machine rather than ported from the library.
Randomised configurations are run through both and compared, which covers ground fixed
vectors cannot: a bug that reads the notch from the ring-adjusted offset rather than
the window position, for instance, is invisible to every vector taken at ring setting
zero.

```bash
dotnet test
```

## Sources

The wirings, key sheets and test vectors here are taken from published sources rather
than reconstructed, and each is pinned by a test. One rule governs what may be added:
**no cipher data ships unless it is pinned to a published source or a published
vector.** A hand-written Uhr table was produced during development, failed its first
test, and was deleted rather than patched into agreement.

**[docs/sources.md](docs/sources.md)** states that rule in full and lists every source
held to it, with what each one settled.

## License

[MIT](LICENSE).
