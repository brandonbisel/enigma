# Enigma

A simulator of the Wehrmacht and Kriegsmarine Enigma machines, written in C# on .NET 10.

It models the machine as it actually worked: a plugboard, a set of rotors that step
as you type, and a reflector that sends the current back through the rotors in
reverse. Both the three-rotor Wehrmacht and Kriegsmarine machines and the
four-rotor naval M4 are supported. Because the reflector never maps a letter to itself, the machine is
reciprocal — the same settings both encipher and decipher, which is exactly the
property that made it breakable.

What is planned next, and what is deliberately out of scope, is in
[ROADMAP.md](ROADMAP.md).

## Layout

| Project | What it is |
|---|---|
| `Enigma` | The library: rotors, reflectors, plugboard, machine, and DI wireup |
| `Enigma.App` | Orchestration shared by the front ends: sessions, key sheet catalogue |
| `Enigma.Cmd` | A console app, installable as a tool named `enigma` |
| `Enigma.Web` | A browser front end: the machine as an operator's panel |
| `Enigma.Tests` | xUnit tests, including a differential suite against an independent oracle |
| `Enigma.Web.Tests` | Component tests for the panel, on bUnit |

## Building and running

```bash
dotnet build
dotnet test
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd
```

## The panel

```bash
dotnet run --project Enigma.Web
```

The machine as an operator faced it: the wheel windows above, the lamps below them
and the keys below those. Press a key and the wheels turn *before* the lamp
lights, which is the order the machine works in. A lamp stays lit only while its
key is held, and the keyboard takes one key at a time, as the real one did. The
letter that lights, pressed back on a machine returned to the same setting, gives
the original — reciprocity, which is the first thing anyone tries.

The panel is built from whatever machine the key sheet names. An Enigma I shows
three windows and an M4 four, for its fourth wheel; a three-wheel Zählwerk machine
shows four as well, but the extra one is its reflector, which turns and so is part
of the setting. The keyboard is laid out from the machine's own alphabet rather
than from a constant, so a machine that does not work in letters gets its own keys.

The panel is labelled as the machine was — Walzenlage, Ringstellung, Steckerbrett —
with the English alongside each term, so the words can be picked up rather than
looked up.

Under *Spruchschlüssel* the panel offers both indicator procedures, because the two
services worked out where the rotors start in quite different ways. Choose **Heer /
Luftwaffe** and the message key is enciphered at the ground setting and shown in
clear, as it travelled. Choose **Kriegsmarine** and nothing goes in clear at all: the
two Kenngruppen and their padding letters when sending, the eight transmitted letters
when receiving, and both hidden under a Doppelbuchstabentauschtafel.

Which table is a choice the operator did not make. Give the panel a Kennziffer column
— the one his cipher net was allotted — and a day of the month, and the
Tauschtafelplan names the table, exactly as it did aboard. Pick nothing and any of the
eight shipped tables can be chosen by hand instead. A day that falls on Tafel J, the
one never recovered, says so rather than failing quietly.

U-534's real message runs through it: load the *U-534, 1 May 1945* key sheet, choose
Kriegsmarine receiving, Kennziffer six on the first of the month, and type the
transmitted `FNHC GVET`. The windows come up `ODFF` — the setting the operator wrote
on his own message sheet.

Under the machine, the current can be followed. Tick **Follow the current** and
each keypress is set out step by step: in through the board and the stator, right
to left across the wheels, back off the reflector — marked, because it is the
turning point — and out the way it came. Every letter shown comes from the
`TranslationTrace` the machine itself reports, which is the same structure the
diagnostic log is written from, so the view is the machine's own account rather
than a retelling.

It is asked for rather than assumed: a machine nobody is watching builds no trace,
which is what keeps a long message cheap.

Below the machine is the message. The keyboard and the plaintext pane are two ways
of entering one message, not two messages: type on the keys and the text grows a
letter at a time, or write the whole thing and it is keyed from the start. Either
way the ciphertext is the same, because the machine is the only thing deciding it.

Two conventions of the signaller sit behind toggles. **Fit to the keyboard** applies
`MessageText.Prepare`, expanding umlauts, spelling digits out and writing spaces as
X, so ordinary German can be keyed on a machine that has none of those. **Groups of**
breaks the ciphertext into fives, as it was transmitted, so a miscount showed up at
the far end. Grouping is how the message is written out rather than how it was
enciphered, so it moves no wheels — a lamp still held stays lit.

Changing any setting keys the same message again, which is the point of being able
to change them: the same text, keyed a different way. Clearing the message is what
puts the wheels back; there is no separate reset, because a machine with nothing
typed on it is a machine at its start.

Under the message is the indicator procedure, if one is in use. Sending, you choose
a message key and the page works out the indicator to transmit with it; the wheels
move to your key rather than to the sheet's Grundstellung, which becomes the ground
setting. Receiving, you enter the indicator and the message key is worked back.
Either way an indicator that will not work is reported and the wheels are left
where they were, because mistyping one is an ordinary thing to do.

Below that is its key sheet, set by hand. The wheels, reflector, entry
wheel and alphabet are chosen from the parts catalogue, so a wheel defined in a
parts file appears in the list without the page knowing it exists. Adding a
fourth wheel offers a thin one, because that is the only kind that fits beside a
thin reflector. Settings that will not build a machine are reported, and the
machine already in use is left alone — being midway through setting up is not the
same as holding a broken machine.

The Steckerbrett is below that: click a letter to take up a plug, click another to
run the cable, and click a cabled letter to pull it out. A machine built without a
board — a Zählwerk Enigma — is offered none at all rather than one whose cables
would be refused.

It is a WebAssembly page with no server behind it: the library runs in the browser
unchanged, and nothing typed into it is transmitted anywhere. `dotnet publish`
produces a folder of static files — about 2.1 MB over the wire once compressed —
that can be served from anywhere. Hosting it under a subpath means changing
`<base href="/" />` in `wwwroot/index.html` to match.

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

## The naval indicator procedure

The Army sent its ground setting in clear. The Kriegsmarine sent nothing in clear
at all.

Its operator took two trigrams from the **Kenngruppenbuch**: a *Schlüsselkenngruppe*
saying which key was in force, and a *Verfahrenkenngruppe* which, typed at the day's
Grundstellung, gave him the message key. He then padded the first with a letter in
front and the second with a letter behind, wrote them one above the other, and read
downwards:

```
A H L G      filler, then the Schlüsselkenngruppe
K Q K Z      the Verfahrenkenngruppe, then a filler
```

The four column pairs — `AK HQ LK GZ` — went through a
**Doppelbuchstabentauschtafel**, a double-letter conversion table, becoming
`BD BJ EM EJ`, and those eight letters travelled with the message. Letters that
were sent together had never been next to each other.

The table is reciprocal: if `AK` is written as `BD` then `BD` is written as `AK`, so
one table serves both stations without being reversed. It is an involution on pairs
of letters, exactly as a reflector is on single ones.

**Eight real tables ship**: `BigramTables.QuelleA` through `.QuelleH`, Tafels A to H
of the set "Quelle", transcribed from the Crypto Museum's scan of booklet
Prüf-Nr. 2499. It is the set U-534 was using on 1 May 1945, and it is every table the
scan contains — the booklet's cover says the edition held nine, "A bis J ohne I",
but Tafel J and the two Tauschtafelplan sheets are not reproduced in it.

A transcription of a table is worth more than a transcription of a wiring, because it
can be checked. All 676 entries are present, every one pairs with its mirror, and no
bigram enciphers to itself — a single mistyped cell would break a pair and show up.
Eighteen cells did, across the eight tables, and each time the rest of the table said
what the cell had to be — then the glyph at magnification agreed. `B`/`E` is the
commonest slip and `F`/`P` the next. Eight of the eighteen are in Tafel G alone, whose
two pages are the poorest scan in the booklet. Six further cells of Tafel E are
covered by later hand-written ink and could not be read at all; they were rebuilt from
their mirrors, which on an involution is the same fact written twice. On top of all
that, the four cells the U-534 message needed were published separately with the
working of that message, and they agree.

The whole message now runs end to end in the test suite: the transmitted indicator
`FNHC GVET`, through the real table, to the message key `ODFF` the operator wrote on
his sheet — and back again.

**The calendar ships too.** `Tauschtafelplan.BrunoQuelle` is the *Tauschtafelplan
"Bruno"* for this set, transcribed from Michael Hörenberg's photograph of the
original: thirty-one days by six Kennziffer columns, the column chosen by cipher net.
It agrees with the traffic at the one point it can be checked — U-534 sent on Tafel A
on 1 May 1945, and the column pencilled "Mai 45" reads `A` on the first.

```csharp
var tafel = Tauschtafelplan.BrunoQuelle.Tafel(kennziffer: 6, dayOfMonth: 1);  // 'A'
```

Tafel J is in neither source and is not shipped. Supply a table and it will be used:

```csharp
var table = BigramTable.Parse("AK=BD HQ=BJ LK=EM GZ=EJ");
var sent = navalProcedure.Send(dailyKey, table, "HLG", "KQK", 'A', 'Z');
```

One detail worth knowing, and one this library got wrong until a real message caught
it: the Kenngruppenbuch lists trigrams, and three letters cannot key four wheels.
What sets an M4's fourth wheel is the **filler** — the letter that padded the
trigram out to fill its bigram column. Both stations have it, the sender by choosing
it and the receiver by reading it out of the indicator, so the group typed at the
ground setting is the trigram and its filler, taken to as many letters as the
machine has wheels.

That is pinned by traffic rather than by reasoning. Message P1030690 from U-534,
1 May 1945: the indicator `FNHC GVET` decodes through "Quelle" Tafel A to the
Schlüsselkenngruppe `DUZ` and the Verfahrenkenngruppe `YMU`, and `YMUZ` typed at the
Grundstellung `IBFK` on that day's key gives the message key `ODFF` — which is what
the operator's own sheet says it was.

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

## The Zählwerk Enigma (G-31)

The Abwehr's Enigma G is the one machine here that differs in *mechanism* rather
than in wiring. Name it as the model:

```json
{
  "Model": "G-31",
  "Reflector": "G",
  "Rotors": "G-I G-II G-III",
  "RingSettings": "AAA",
  "Positions": "AAA",
  "ReflectorPosition": "A",
  "ReflectorRingSetting": "A"
}
```

Four things set it apart:

- **Cogwheels rather than pawls.** The wheels turn as a plain odometer, so there is
  **no double step** — the middle wheel advances only when the wheel to its right
  passes a notch, never twice in succession.
- **The reflector turns.** It is set to a starting position like a wheel, and is
  driven round by the leftmost wheel during encipherment, so it takes an active part
  in the cipher.
- **Many notches.** Seventeen, fifteen and eleven on wheels I, II and III, numbers
  chosen because they are relatively prime to twenty six, which stretches the
  machine's period enormously.
- **No plugboard.** The Steckerbrett was reserved for the Army, and a key sheet that
  gives cables to a G-31 is refused.

The entry wheel is the keyboard-wired one, and a key sheet need not say so: leave
`EntryWheel` unset and the model supplies it. The commercial A28/G31 wheels are
`G-I`…`G-III` with reflector `G`; the Abwehr set from the Bletchley Park machine is
`G312-I`…`G312-III` with reflector `G312`.

Because of all this a Zählwerk machine cannot exchange messages with an Enigma I,
which is exactly what its makers intended.

## The commercial machines: Enigma D and K

The Enigma D of 1926 and the Enigma K that followed it were sold to whoever would
buy them, and are the ancestors of everything else here. Three wheels driven by
pawls, a keyboard-order entry wheel, a reflector set to a position but never driven,
and no plugboard — the Steckerbrett was an Army fitting.

```json
{
  "Model": "Commercial",
  "Reflector": "G",
  "Rotors": "K-III K-II K-I",
  "RingSettings": "AAA",
  "Positions": "AAA",
  "ReflectorPosition": "A"
}
```

The D, the K and the Zählwerk A28/G31 were fitted with **the same three wheel
wirings and the same reflector**, which is why they share them here too. What
separates them is entirely in the notches:

| | Notches | Cut into | Driven by |
|---|---|---|---|
| Enigma D | Z, Z, Z | the rotor body | pawls, double stepping |
| Enigma K | Y, E, N | the letter ring | pawls, double stepping |
| A28 / G31 | 17, 15, 11 | the letter ring | cogwheels, no double step |

The D's notch placement has a consequence its makers may not have intended: since
the notch keeps its place against the wiring, moving the ring moves the turnover
with it, and a Ringstellung is only ever a different starting position. As the
Crypto Museum puts it, "the cryptographic effect of the Ringstellung is null. It
does not enhance the machine's key space." A test proves it by setting ring and
position together and getting the same ciphertext back.

### The same pattern, other wheels

Three more machines are the commercial arrangement with different parts, and need
no new mechanism at all:

| | Wheels | Reflector | Stator | Notes |
|---|---|---|---|---|
| Swiss K | `SK-I`…`SK-III` | `G` | QWERTZ | the Swiss rewired the wheels and left the rest |
| Railway | `R-I`…`R-III` | `R` | QWERTZ | the wiring found in machine K438 |
| Tirpitz | `T-I`…`T-VIII` | `T` | its own | eight wheels, five notches each |

The Railway Enigma has two published wirings: Bletchley Park's wartime
reconstruction and the original, recovered from machine K438 in 2023. They are
equivalent up to ring settings, but BP's has the turnovers of wheels I and III
swapped through a misidentification. The original is what ships.

The Enigma T is the one machine here whose entry wheel is neither straight through
nor the keyboard order, and forgetting it would encipher perfectly well and
wrongly — so its model supplies it, and a key sheet need not say so. Its eight
wheels carry five notches apiece, which is what stretches its period.

### Two machines that are other machines rewired

The **Norenigma** is an Enigma I. The Norwegian police security service kept theirs
after the war and gave it new wheels (`N-I`…`N-V`) and a new reflector (`N`),
leaving the plugboard, the straight-through stator and the pawls exactly as they
were — so it is a service machine and needs no model named at all. Its notches sit
where the service wheels of the same numbers had them, wheel I still carrying at Q.

The **Enigma KD** is a commercial K with a rewirable UKW-D in place of its
reflector. That is the one thing that separates it: a UKW-D has no position to set,
so it needs its own model, `KD`, which is the commercial one with that requirement
relaxed. Its three wheels carry nine notches apiece.

```json
{
  "Model": "KD",
  "Reflector": "D",
  "ReflectorPairs": "AK BO CT DV EP FN GL HM IJ QW RY SX UZ",
  "Rotors": "KD-III KD-II KD-I"
}
```

Those pairs are the wiring of the KD machine held by the FRA in Sweden, written as
the plain letter pairs this library takes rather than in the printed UKW-D notation,
which uses the wheel's own contact lettering and is not applied here.

## The Enigma Z, a machine of figures

The Z30 has no letters at all: ten contacts per wheel, a keyboard of a single row
of figures, and no plugboard. It was built for traffic that was numeric to begin
with, such as weather reports.

```json
{
  "Model": "Z30",
  "CharacterMap": "Digits",
  "Reflector": "Z",
  "Rotors": "Z-III Z-II Z-I",
  "RingSettings": "000",
  "Positions": "000",
  "ReflectorPosition": "0"
}
```

Its reflector is driven, as the Zählwerk machines' is, but by pawls rather than
cogs: there is one pawl more than there are wheels, and the extra one rides the
leftmost wheel's notch ring. That gives the leftmost wheel a double step of its
own, for the same reason the middle wheel has one on an Enigma I — and it is
exactly the pawl a service machine does not have.

The wheel wirings are published by the Crypto Museum indexed 1 to 0 and by Daniel
Palloks' simulator indexed 0 to 9; converting between the two makes them identical.
The vectors in the test suite come from that simulator.

The machine reached the Spanish Foreign Ministry in 1931 as a 600-Reichsmark offer
alongside the commercial A27 and the printing H29, and the papers that survive there
are most of what is known about it. Its key had two parts and no plugboard: an
*inner* key naming the wheels and their order, and an *outer* key of four figures —
three wheels and the reflector — written like `III I II 5 2 8 1`.

Those papers also describe a machine that counts. The wheels and the reflector are
"coupled to each other in the manner of a normal decade counter", so the windows
tell the operator how many figures have been enciphered, and the brochure claims a
period of 10,000: every wheel back where it started after ten thousand keystrokes.

That is an odometer, and it is a claim worth testing. This library's `GearDrive`
has a period of exactly 10,000 on four ten-position wheels. The pawl drive of the
Mk I shipped here never returns to its starting window at all — a double step makes
the stepping non-injective, so some windows can never be reached and all zeros is
one of them. So the brochure describes the geared Mk II rather than this machine,
and both tests are in the suite.

Whether that Mk II was ever built is itself uncertain, and only the Mk I survives —
one machine, Z103, held by the FRA in Sweden. Its wheels are what this library
carries; the geared drive is here because the Zählwerk machines need it anyway.

Its notch sits somewhere unusual, and it matters. On an Enigma I or K the notch is
cut into the index ring, so a wheel carries its neighbour at a fixed letter in the
window whatever the Ringstellung — rotor I always at Q. On the Z30, as on the older
Enigma D, it is cut into the rotor body instead, so it keeps its place against the
wiring and setting the ring carries the turnover with it. `RotorBase` models both,
and every wheel is of the first kind unless it says otherwise.

That is not a detail one can reason out from first principles: the same corpus that
settles it for the service machines says nothing about this one. It is settled here
because the Crypto Museum states it outright, and because the simulator's vectors
with ring settings only match once the notch is in the right place.

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

To work a machine rather than build one, `Enigma.App` has a session: a machine
keyed for the day, pressed a key at a time. A key sheet it cannot be built from
comes back as a message rather than an exception, which is what a front end needs.

```csharp
var keyed = EnigmaSession.Open(factory, keySheet);

if (!keyed.Succeeded)
{
    Console.Error.WriteLine(keyed.Error);
    return;
}

var session = keyed.Session!;

session.Press('A');           // the lamp that lit, or null for a key it has not got
session.Type("ATTACKATDAWN"); // the same, for a run of text
session.WindowText;           // where the wheels stand
session.Patch('A', 'V');      // run a cable, as the board itself allows it
session.Reset();              // back to the key sheet, rebuilt rather than wound back
```

## Watching it work

`--verbose` traces each character through every component, with the rotor window
alongside. Primed names are the return leg, after the reflector.

```
A -> B   window AAB   plug A>A | Standard A>A | III A>C | II C>D | I D>F | B F>S | I' S>S | II' S>E | III' E>B | Standard' B>B | plug B>B
```

The same path is available as data rather than text. Subscribe to a machine's
`Translated` event and each keypress arrives as a `TranslationTrace`, carrying the
key, the lamp, the rotor window, and every component the current passed through.
The log line above is formatted from exactly that structure, so a trace and a
diagnostic cannot disagree. A machine nobody is watching builds nothing.

```csharp
machine.Translated += trace =>
{
    // Contacts are numbers in the machine's own alphabet, so map them to read them.
    var window = string.Concat(trace.Window.Select(machine.CharacterMap.GetCharacter));

    Console.WriteLine($"window {window}, {trace.Steps.Count} components");
};
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

## Sources

The wirings, key sheets and test vectors here are taken from published sources
rather than reconstructed, and each is pinned by a test.

- **[Crypto Museum](https://www.cryptomuseum.com/crypto/enigma/uhr/index.htm)** —
  the Enigma Uhr, including the photograph of the plates inside the lid from which
  the two letter setting notation is read, and the cable ordering `1a-1b` through
  `10a-10b`.
- **[John Savard, quadibloc](http://www.quadibloc.com/crypto/ro020402.htm)** — the
  Uhr's construction: eighty contacts in two rings, the `a` wires in order and the
  `b` wires scrambled, and the reason every fourth position is reciprocal. Savard
  credits Frode Weierud for the facts behind it, and cites a paper in *Cryptologia*,
  July 1999, for the authoritative account.
- **[Arduino Enigma](https://arduinoenigma.blogspot.com/2020/03/enigma-uhr-switch-test-vectors.html)**
  — test vectors for all forty dial positions, generated with Daniel Palloks'
  Universal Enigma and partly hand verified. The Uhr table in this repository is
  derived from those vectors, and every one of them is a test. A
  [companion post](https://arduinoenigma.blogspot.com/2020/05/designing-uhr-switch-for-arduino-based.html)
  gives an independent vector with a different plug set, which the same table
  reproduces.

- **[Palloks, *Universal Enigma*](https://palloks.2ix.de/enigma/index_en.html)** —
  the geared drive. Its `engage_gear` gives the carry chain the Zählwerk machines
  use and where it ends, and its `etq` table independently confirms the direction of
  the keyboard-wired entry wheel used here.
- **[Crypto Museum, *Enigma Z*](https://www.cryptomuseum.com/crypto/enigma/z/index.htm)**
  — the Z30's wheel and reflector wirings, indexed 1 to 0, and where its notch is
  cut: "The notch is attached to the rotor body, which means that altering the
  Ringstellung does not alter its position with respect to the wiring... different
  from the rotors of later machines like Enigma K and Enigma I where the notch is
  attached to the index ring."
- **Arturo Quirantes, *Model Z: a numbers-only Enigma version*** — the paper that
  found the machine in the Spanish Foreign Affairs Ministry archives (reference
  007459-4-R), with the 1931 offer, the two-part key, and the brochure's claim of "a
  period of 10,000... thereby suggesting a non-Enigma (odometer) stepping". Its
  worked example, 25183 91467 enciphering to 38760 15924, cannot be used as a vector
  here: as the paper says, no machine settings are given with it.
- **The Z30 instruction manual (Spanish) and brochure (German)**, both from those
  archives — the decade-counter description of the drive, and the key count of
  1,200,000,000 with six cylinders fitted three at a time, which is exactly 120 wheel
  orders times ten thousand window positions times a thousand ring settings.
- **[Palloks, *Enigma Z*](https://palloks.2ix.de/enigma/index_en.html)** — the same
  wirings indexed 0 to 9, which agree exactly once converted; the pawl chain that
  reaches the reflector; and the vectors the Z30 tests are built on, taken from it
  before any of this was written.
- **[Crypto Museum, *Enigma D*](https://www.cryptomuseum.com/crypto/enigma/d/index.htm)**
  and **[*Enigma K*](https://www.cryptomuseum.com/crypto/enigma/k/index.htm)** — the
  commercial wirings the D, the K and the A28/G31 share, their notch positions, and
  the difference that matters between them: the D's "notch ring is attached to the
  body of the rotor (rather than to the letter ring)", while the K's is "attached to
  the letter ring rather than rotor body".
- **Crypto Museum, [*Swiss K*](https://www.cryptomuseum.com/crypto/enigma/k/swiss.htm),
  [*Railway Enigma*](https://www.cryptomuseum.com/crypto/enigma/k/railway.htm) and
  [*Enigma T*](https://www.cryptomuseum.com/crypto/enigma/t/index.htm)** — their
  wheels, notches, reflectors and, for the T, its own entry wheel. The Railway page
  also records that its two published wirings differ by a misidentification, "with
  turnover positions of rotors I and III swapped" in Bletchley Park's version, and
  that Friedman's report of a moving reflector on that machine was mistaken.
- **Palloks, *Universal Enigma*, model tables** — the Norenigma's five wheels and
  reflector, the KD's nine-notch wheels, and the UKW-D wiring of the KD machine
  held by the FRA in Sweden, which is the one the packaged key sheet carries.
- **[Dirk Rijmenants, *Enigma Procedures*](https://ciphermachinesandcryptology.com/en/enigmaproc.htm)**
  — the Kriegsmarine indicator procedure step by step, and the worked example the
  tests reproduce: groups HLG and KQK with fillers A and Z becoming BDBJ EMEJ under
  bigram table B. Four entries of that table are published with it, and those four
  are all this library claims to know.
- **[Michael Hörenberg, *The Kenngruppen System*](https://enigma.hoerenberg.com/index.php?cat=The+U534+messages&page=The+Kenngruppen+System)**
  — message P1030690 from U-534 worked right through: the transmitted indicator, the
  four entries of "Quelle" Tafel A it needs, the day's key, and the message key
  `ODFF` that comes out. The naval procedure here is pinned by that, and it is what
  showed the M4's fourth wheel is set by the filler.
- **[Crypto Museum, *Bigram tables*](https://www.cryptomuseum.com/crypto/codebook/bigram.htm)**
  — what the tables were and how they were used. The recovered ones are reproduced
  there as photographs of the originals, which is why none is transcribed here.
- **Reuvers & Simons, *Enigma G-111: A rare version of Zählwerk Enigma G31***
  (Crypto Museum, 2013) — the mechanism in the machine's own terms: cogwheels rather
  than pawls and levers, no double stepping anomaly, notches attached to the letter
  ring, and the statement that the UKW "can be moved by wheel 3", which is what puts
  the reflector at the end of the carry chain.

The historical messages — Barbarossa, Scharnhorst, U-264 and the 1930 instruction
manual — are widely published intercepts, and each is decrypted in the test suite
with its own key sheet.
