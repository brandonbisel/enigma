# The indicator procedures

A key sheet fixed the wheel order, the rings and the plugboard for the day. Where the
rotors *started* was the sender's own choice, and how he told the receiver what he had
chosen is the indicator procedure. The two services did it in quite different ways,
and both are here, in both front ends. The short version is in
[the README](../README.md#the-indicator-procedures).

## The Army and Luftwaffe procedure

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

## The naval procedure

The Army sent its ground setting in clear. The Kriegsmarine sent nothing in clear at
all.

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

**The command line offers the procedure too.** A table is what makes it naval — the
Navy's indicator cannot be worked without one, and the Army's never wants one — so
`--tafel` or `--kennziffer` is the whole switch. Reading a message:

```bash
$ echo "..." | dotnet run --project Enigma.Cmd -- \
      --preset u534 --kennziffer 6 --monatstag 1 --indicator "FNHC GVET"
Tauschtafelplan Bruno, Kennziffer 6, Monatstag 1: Tafel A
Ground setting IBFK, Schlüsselkenngruppe DUZ, Verfahrenkenngruppe YMU, indicator FNHCGVET, rotors ODFF
```

`--set` picks the booklet and `--list-sets` says what is on offer; `--kennziffer` with
`--monatstag` reads the table off that set's Tauschtafelplan, as an operator did, and
`--tafel A` names one outright for anyone working without a calendar.
Sending takes the two trigrams and the two padding letters, and produces the eight
letters that went out:

```bash
$ echo "..." | dotnet run --project Enigma.Cmd -- \
      --preset u534 --tafel A --kenngruppen "DUZ YMU" --fillers KZ
Ground setting IBFK, Schlüsselkenngruppe DUZ, Verfahrenkenngruppe YMU, indicator FNHCGVET, rotors ODFF
```

A day the calendar sends to a table the set does not have stops and says so rather than
substituting one that would decipher to plausible nonsense.

Quelle's Tafel J is in neither source and is not shipped, so some days of its calendar
have no table to offer and say so. Meer and Flußlauf have no such day. Supply your own
table and it will be used:

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

## Where it lives

`NavalIndicatorProcedure` and `BigramTable` carry the Kenngruppenbuch trigrams, the
fillers, the column pairing and the Doppelbuchstabentauschtafel, pinned first by the
published worked example and then by a real message: U-534's P1030690 of 1 May 1945,
read from its transmitted indicator through to the message key its operator wrote
down. The panel is checked by that same message — `FNHC GVET`, on the day the calendar
puts on Tafel A, has to leave the rotors reading `ODFF`.

The rule for *which* table — the calendar outranks a named letter, and a letter that
was never recovered is an answer rather than an exception — is `BigramTableChoice` in
`Enigma.App`, so both front ends decide it once rather than twice. `BigramTableSet`
pairs a set of tables with the calendar issued alongside it, because the two are
useless apart, and neither front end can offer a mismatched pair.

The presence of a table is what selects the naval procedure at the command line: the
Navy's indicator cannot be worked without one and the Army's never wants one, so
nothing has to be told which service is meant.

## See also

- [Bigram tables](bigram-tables.md) for the tables the naval procedure runs on: what
  ships, how each was read, and how far each is to be trusted.
- [The command line](command-line.md) for every option named above.
- [The panel](panel.md) for the same two procedures under *Spruchschlüssel*.
