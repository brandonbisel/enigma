# Roadmap

Where this simulator has got to, what is planned, and what is deliberately left
alone. For how to work in the repository see [AGENTS.md](AGENTS.md); for how to use
it, [README.md](README.md).

## The standard everything here is held to

One rule governs what may be added: **no cipher data ships unless it is pinned to a
published source or a published vector.**

This is not a slogan. A hand-written Uhr table was produced during development,
failed its first test, and was deleted rather than patched into agreement — the
feature waited until published forty-position vectors turned up. The G-31's wheels
were withheld until its stepping rule was confirmed by two independent sources.
When a mechanism is understood but its wirings are not, the right move is to ship
the mechanism and say so.

Everything below inherits that rule. A machine on the "next" list is not done when
it runs; it is done when something published says it runs correctly.

## Where things stand

| Area | State |
|---|---|
| Enigma I, M3 | Complete, with the pawl drive and its double-step anomaly |
| Naval M4 | Complete, including the thin rotors that no pawl can drive |
| Abwehr G-31 (Zählwerk) | Complete: gear drive, no double step, turning reflector |
| Plugboard | Complete, including the Uhr and its dial notation |
| UKW-D | Complete — the reflector rewired in the field |
| Entry wheel | Complete, straight-through and QWERTZ |
| Custom alphabets | Complete — the alphabet is a first-class part of the machine |
| Custom parts files | Complete — rotors, reflectors and alphabets from user files |
| Indicator procedure | Army and naval, both in both front ends |
| Bigram tables | Three sets: "Meer" and "Flußlauf" complete, "Quelle" less its Tafel J |
| Message formatting | Complete — preparation and five-letter groups |
| Key sheets | Complete, in the notation a real key sheet used |
| Enigma D and K | Complete — the commercial pattern, shared wheels, notches apart |
| Swiss K, Railway, Tirpitz | Complete — the same pattern with other wheels |
| Norenigma and KD | Complete — an Enigma I rewired, and a K with a UKW-D |
| Enigma Z30 | Complete — ten contacts, figures, a pawl-driven reflector, a rotor-body notch |
| Front ends | A command line tool and a browser panel, over a shared session layer |
| The panel | Windows, lamps, keyboard, signal path, message, key sheet, plugboard, both indicator procedures |

714 tests, including seven historical messages — Barbarossa, Scharnhorst, Graf Spee,
U-264, U-106, the signal naming Dönitz Hitler's successor and the 1930 instruction
manual — each verified against its published ciphertext, and a differential suite
comparing against an independently written oracle. The Dönitz signal is what pins the
thin UKW-C, every other message here running on B; the Graf Spee signal is the
earliest, and the only one cabled with eight plugboard pairs rather than ten.

The web front end is complete. It is a Blazor WebAssembly page with the verified
library running unchanged in the browser: no server, so nothing typed into it is
transmitted anywhere.

### The naval indicator procedure

`NavalIndicatorProcedure` and `BigramTable` carry the Kenngruppenbuch trigrams, the
fillers, the column pairing and the Doppelbuchstabentauschtafel, pinned first by the
published worked example and then by a real message: U-534's P1030690 of 1 May 1945,
read from its transmitted indicator through to the message key its operator wrote
down.

**The panel offers it.** Choosing *Kriegsmarine* gives the two Kenngruppen and their
fillers when sending, eight letters when receiving, and a table picker that works the
way an operator did: give it a Kennziffer column and a day of the month and the
Tauschtafelplan names the table, rather than the table being chosen by hand. A day
that falls on a table no source reproduces says so instead of failing quietly. It is
checked at the panel by the message it is pinned by in the library — U-534's `FNHC
GVET`, on the day the calendar puts on Tafel A, has to leave the rotors reading
`ODFF`.

**The command line offers it as well.** `--tafel`, or `--kennziffer` with
`--monatstag`, picks the table; `--kenngruppen` sends and `--indicator` receives;
`--set` chooses a set and `--list-sets` says what is on offer and which of them is
incomplete. The presence of a table is what selects the procedure, so nothing has to
be told which service is meant.

The rule for *which* table — the calendar outranks a named letter, and a letter that
was never recovered is an answer rather than an exception — is `BigramTableChoice` in
`Enigma.App`, so both front ends decide it once rather than twice. `BigramTableSet`
pairs a set of tables with the calendar issued alongside it, because the two are
useless apart, and neither front end can offer a mismatched pair.

### The bigram tables

Three of the Crypto Museum's booklets are transcribed and ship. A set is a set of
tables plus the Tauschtafelplan — the calendar that says which table a given
Kennziffer column uses on a given day of the month.

| Set | Booklet | Tables | Calendar |
|---|---|---|---|
| Quelle | Prüf-Nr. 2499 | 8 of 9 — A to H | front only: Kennziffer one to six |
| Meer | Prüf-Nr. 3733 | 9 — A to J without I | both sides: one to twelve |
| Flußlauf | Prüf-Nr. 3633 | 15 — A to P without I | both sides: one to twelve |

Quelle is the set U-534 was using and stays the default. Meer and Flußlauf are
complete, and completeness is a test rather than a claim: **every one of the 372 cells
of their calendars leads to a table that is actually here**, and dropping any single
table fails it. Quelle is the set that cannot say that, and it is where the refusal
stays under test.

**A set is not always nine tables, and nothing may assume it is.** `TableChoice.Missing`
used to tell an operator that "the set ran to nine tables, A to J without I" — true of
the two sets that then existed and false of Flußlauf. The count and the range are read
off the set's own calendar, which is the only thing that knows them. A full plan has
twelve Kennziffer columns, not six; that is what the Quelle photograph's
*"Fortsetzung Rückseite!"* was pointing at.

**Why so few of these survive at all.** The Kriegsmarine printed its cipher documents
in water-soluble ink on paper that dissolved: a table aboard a sinking boat wiped
itself the moment the water reached it, which is exactly what the design intended. It
is why the daily key sheets are gone entirely — U-534's is reconstructed rather than
transcribed — and why a complete bigram set is a rarer thing than a rotor wiring. The
Flußlauf Tauschtafelplan says so on its own face: above the title it is headed
**"Vorsicht! Wasserlöslicher Druck!"**. That is not an inference from the survival
rate; it is printed on the document.

#### How a reading is trusted

A table is an involution with no fixed point, so 676 independently read cells all have
to pair up: a mistyped letter breaks its pair and is named rather than shipped. That
is what makes reading one by hand worth doing, and where a reading has been wrong the
mirror has also supplied the answer. Every adjudicated cell is recorded, with the
argument that settled it, in its file under `Enigma.Tests/Data`; the pipeline that
puts native pixels in front of a reader, and the eleven letter pairs this typeface
confuses, are in [tools/transcribe/README.md](tools/transcribe/README.md).

Six cells of Quelle's Tafel E are covered by later hand-written ink and could not be
read; they were reconstructed from their mirrors instead. On an involution that is not
a guess — the mirror is the same fact written twice — but it is worth recording that
those six were derived rather than seen.

Note what the check does and does not do: it catches transcription error, not
fabrication, so a table still only ships against a scan.

**A calendar cannot be checked that way**, because it has no redundancy: it is 372
independent letters. The check that every column uses all nine letters is weak, and a
mutation test confirmed it — changing one cell from `D` to `A` left the suite green.
So the Meer calendar was checked against a **second Meer booklet, Prüf-Nr. 4002**,
separately photographed: all 372 cells were read again from it and **all 372 agreed**.
That is the only cipher data in this repository with a check of that kind. It verifies
the reading and not the edition — an error in the printing would appear in both copies,
and would anyway be what the operators worked from.

The Flußlauf calendar carries a check of its own. Fifteen tables over thirty-one days
is fourteen letters twice and one three times, and eleven of its twelve columns are set
exactly so — a much tighter constraint than nine letters per column, and it earned its
keep by flagging two misreadings before anything shipped. Kennziffer four does not
conform: `K` appears once there, `D` and `O` three times each. That column was read
cell by cell a second time and the irregularity is in the print. It is recorded as a
test rather than tidied away, so that a later pass cannot quietly "correct" the
document into agreeing with a pattern.

**`BrunoQuelle` remains the weakest evidence here.** There is only one photograph of
that sheet, half of it was never taken, and its pen corrections add a second layer to
read: every printed `C` and `H` in columns one, four and six is struck out and replaced,
consistently per column. The natural reading is that Tafeln C and H were withdrawn —
but the sheet does not say so, and the library records the two layers rather than the
inference.

## Next

### The two gaps in the naval data

- **Quelle's Tafel J.** In no source found so far. It is the only table any of the
  three calendars names that is not here.
- **The reverse of the Quelle Tauschtafelplan.** Kennziffer seven to twelve, on a side
  that was never photographed. Meer's plan shows what those columns look like; Quelle's
  are not recoverable from it.

Both are sourcing problems rather than work, and neither will be filled by inference.

### More machines

`IMachineLayout` and `IStepping` were introduced for the G-31, and they reduced a new
model to *parts plus a layout* — the Z30 needed a character map, a set of wheels and
one new drive, and nothing else. What remains is therefore a matter of sourcing
wirings rather than writing mechanism, and the standard above decides which of them
can ship.

## Future

**Cryptanalysis** — bombe simulation, index-of-coincidence rotor search, and
ciphertext-only attack. This is deliberately placed after the simulator is
complete rather than alongside it: an attack is only worth writing against a
machine already known to be right, and a wrong machine would make a broken attack
look successful.

## Not scheduled

- **Enigma Z Mk II.** Probably not possible. The gear-driven version of the Z30 may
  never have been built: "it is uncertain whether this machine was actually built",
  though a brochure photograph shows a serial number in the range its production would
  have used. Only one Z30 of any mark survives, the Mk I Z103 held by the FRA in
  Sweden. No wiring is published and there is no machine to recover one from, so this
  is recorded as considered rather than as work waiting to be done. Its mechanism is
  nonetheless here already and tested: `GearDrive` has the period of 10,000 that the
  1931 brochure advertised, which the Mk I's pawls cannot give.
- **Packaging and continuous integration.** The tool builds and installs; nothing
  publishes it to NuGet and no CI runs the suite. Worth doing, not yet planned.
- **Mechanical detail with no effect on the cipher.** The G-31's hand crank and
  backward stepping, its four-digit letter counter, the 52-tooth cogwheel. These
  are properties of the object rather than of the cipher, and a simulator that
  reproduced them would not encipher a single letter differently.
