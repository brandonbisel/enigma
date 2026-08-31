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
| Message formatting | Complete — preparation and five-letter groups |
| Key sheets | Complete, in the notation a real key sheet used |
| Enigma D and K | Complete — the commercial pattern, shared wheels, notches apart |
| Swiss K, Railway, Tirpitz | Complete — the same pattern with other wheels |
| Norenigma and KD | Complete — an Enigma I rewired, and a K with a UKW-D |
| Enigma Z30 | Complete — ten contacts, figures, a pawl-driven reflector, a rotor-body notch |
| Front ends | A command line tool and a browser panel, over a shared session layer |
| The panel | Windows, lamps, keyboard, signal path, message, key sheet, plugboard, both indicator procedures |

701 tests, including five historical messages — Barbarossa, Scharnhorst, U-264,
U-106 and the 1930 instruction manual — each verified against its published
ciphertext, and a differential suite comparing against an independently written
oracle.

The web front end is complete. It is a Blazor WebAssembly page with the verified
library running unchanged in the browser: no server, so nothing typed into it is
transmitted anywhere.

## Next

### More machines

`IMachineLayout` and `IStepping` were introduced for the G-31, and they reduced a
new model to *parts plus a layout*. That makes the remaining machines mostly a
matter of sourcing wirings rather than writing mechanism:

- **Enigma Z Mk II** — *not planned, and probably not possible.* The gear-driven
  version of the Z30 may never have been built: "it is uncertain whether this
  machine was actually built", though a brochure photograph shows a serial number in
  the range its production would have used. Only one Z30 of any mark survives, the
  Mk I Z103 held by the FRA in Sweden. No wiring is published, and there is no
  machine to recover one from, so this is listed to record that it was considered
  rather than as work waiting to be done.

  Its mechanism is nonetheless here already, and tested: `GearDrive` has the period
  of 10,000 that the 1931 brochure advertised, which the Mk I's pawls cannot give.

  The Mk I is done. It needed a character map, a set of wheels and one new drive:
  making the alphabet first-class did carry it, and what it did *not* carry was the
  stepping, because the Z30 has a pawl the service machines do not.

### The naval indicator procedure

The procedure itself is done: `NavalIndicatorProcedure` and `BigramTable` carry the
Kenngruppenbuch trigrams, the fillers, the column pairing and the
Doppelbuchstabentauschtafel, pinned by the published worked example.

It is pinned by a real message: U-534's P1030690 of 1 May 1945, read from its
transmitted indicator through to the message key its operator wrote down.

**Why so few of these survive at all.** The Kriegsmarine printed its cipher documents
in water-soluble ink on paper that dissolved: a table aboard a sinking boat wiped
itself the moment the water reached it, which is exactly what the design intended. It
is why the daily key sheets are gone entirely — U-534's is reconstructed rather than
transcribed — and why a complete bigram set is a rarer thing than a rotor wiring. What
Bletchley mostly had was not captured tables but tables rebuilt from traffic.

The Flußlauf Tauschtafelplan says so on its own face. Above the title it is headed
**"Vorsicht! Wasserlöslicher Druck!"** — caution, water-soluble print — and below that
"Tritt erst auf besonderen Befehl in Kraft!". This is not an inference from the
survival rate; it is printed on the document.

**The source has been found and checked.** The Crypto Museum publishes a scan of the
"Quelle" booklet — *Doppelbuchstabentauschtafeln für Kenngruppen*, serial 2499 — the
same set U-534 was using. All four of the entries that message needed read off the
scan exactly:

| | from the message | from the booklet |
|---|---|---|
| `FN` | `KY` | `KY` |
| `HC` | `DM` | `DM` |
| `GV` | `UU` | `UU` |
| `ET` | `ZZ` | `ZZ` |

**The scan is not the whole booklet, and that took until Tafel H to notice.** Its
printed cover states the edition's contents plainly: *"Zu dieser Ausgabe gehören 9
Einzeltafeln, bezeichnet mit Tafeln A bis J ohne I"*, and *"2 Abdrucke
»Tauschtafelplan«"*. But the PDF is 18 pages — a cover, its blank verso, and eight
tables at two pages each. **Tafel J is not in it, and neither is the Tauschtafelplan.**
An earlier version of this document claimed the scan held all nine tables plus the
plan; that was wrong, and the arithmetic alone (2 + 9×2 = 20 pages, not 18) would have
caught it sooner.

So the set is now complete *as far as the source goes*:

**Tafels A to H are transcribed and shipped** — all eight the scan contains. 676
entries each, every mirror agreeing, no bigram enciphering to itself, and — for Tafel
A — the four cells known from real traffic correct. U-534's message runs end to end
through the real table.

**The Tauschtafelplan has since been found in a different source, and ships.** Michael
Hörenberg publishes a photograph of *Tauschtafelplan "Bruno" zu den
Doppelbuchstabentauschtafeln für Kenngruppen, Kennwort: Quelle*, Prüfnr. 1772a — the
calendar for exactly this set. It is `Tauschtafelplan.BrunoQuelle`: thirty-one days by
six Kennziffer columns, the column chosen by cipher net from the Zuteilungsliste.

It is checkable at one point and it holds. U-534's P1030690 of 1 May 1945 was sent on
Tafel A; column six, the one pencilled "Mai 45" on the sheet, reads A on the first of
the month. The tables and the calendar were transcribed from different documents in
different archives and agree.

The sheet carries pen corrections as well as print, and both layers ship. Every
printed `C` and `H` in columns one, four and six is struck out and a replacement
written over it, consistently per column, so that after correction those three columns
use only `A B D E F G J` while the uncorrected columns still contain both letters. The
natural reading is that Tafeln C and H were withdrawn — but the sheet does not say so,
and the library records the two layers rather than the inference.

Two limits stand. Only the six columns printed on the front are in the photograph; the
sheet is footed *"Fortsetzung Rückseite!"* and the reverse is not. And **Tafel J is
still missing** — it is in no source found so far.

That self-checking property is what makes this worth doing by hand. The tables are
involutions, so a mistyped cell breaks its pair and is caught; eighteen cells of this
booklet have needed adjudicating and each time the table itself said what the answer was:

| Tafel | cell | misread as | what settled it |
|---|---|---|---|
| A | `DS` | `QN` | a speck under an O reads as a Q's tail; `ON=DS`, `QN=HA`, `HA=QN` |
| B | `KD` | `BP` | `BP` is taken by `JL`; `EP=KD` in column E |
| C | `MA` | `HB` | `HB` is taken by `QO`; `HE=MA` in column H |
| C | `DM` | `YB` | `YB` is taken by `ST`; `YE=DM` in column Y |
| D | `BP` | `YK` | `YK` pairs with `LJ` both ways; `YX=BP` in column Y |
| D | `JN` | `PI` | `PI` pairs with `ZY` both ways; `FI=JN` in column F |
| F | `JA` | `QP` | `QP` pairs with `MS` both ways; `QF=JA` in column Q |
| G | eight cells | — | see below |
| H | `SI` | `LB` | `LB` pairs with `UH` both ways; `LE=SI` in column L |
| H | `ZD` | `NM` | `NM` and `NN` are both taken; `NH=ZD` in column N |
| H | `RF` | `CX` | `CX` pairs with `UW` both ways; `GX=RF` in column G |

Two kinds of evidence settle them: an exclusion argument from the rest of the table,
and then the glyph itself at magnification, which has agreed every time. `B`/`E` is
the commonest slip, `F`/`P` the next — an `F` whose crossbar has filled in reads as a
`P` — with single cases of `K`/`X`, `I`/`L`, `Y`/`V`, `S`/`B`, `B`/`D` and a speck of
dirt.

**Tafel G is the poorest scan in the booklet**, and it shows: eight of its cells
needed adjudicating where no other table needed more than two. Pages 15 and 16 are
blurred enough that `B`, `E` and `S` are not reliably separable by eye at 600 dpi, so
those eight were settled by the rest of the table first and only then checked against
the glyph. That is the right order — but it is worth being plain that Tafel G rests
more heavily on the involution and less on the reading than its neighbours do.

`G` and `C` are the standing hazard rather than a one-off: at scan resolution they
are near identical in this typeface, and 100 of Tafel D's 676 values contain one or
the other. None of them was decided by eye. The involution check settles every one,
which is the point — the reading that survives is the one the rest of the table
agrees with.

Tafel E needed no adjudication at all, but six of its cells — `LA`, `LB`, `MA`, `MB`,
`NA` and `NB`, all in the first two printed rows — are covered by later hand-written
ink and could not be read. They were **reconstructed from their mirrors** instead
(`MC=LA`, `CM=LB`, `SV=MA`, `FR=MB`, `OZ=NA`, `IQ=NB`). On an involution that is not a
guess: the mirror is the same fact written twice, and a misread mirror would have
surfaced as a break somewhere else in the table. It is still worth recording that
those six were derived rather than seen.

Note what the check does and does not do: it catches transcription error, not
fabrication, so a table still only ships against a scan.

**The panel now offers it.** Choosing *Kriegsmarine* gives the two Kenngruppen and
their fillers when sending, eight letters when receiving, and a table picker that
works the way an operator did: give it a Kennziffer column and a day of the month and
the Tauschtafelplan names the table, rather than the table being chosen by hand. A day
that falls on Tafel J says so instead of failing quietly.

It is checked at the panel by the message it is pinned by in the library. U-534's
`FNHC GVET`, on the day the calendar puts on Tafel A, has to leave the rotors reading
`ODFF`; if the panel wires anything up wrongly, it does not.

**The command line offers it as well.** `--tafel`, or `--kennziffer` with
`--monatstag`, picks the table; `--kenngruppen` sends and `--indicator` receives. The
presence of a table is what selects the procedure, so nothing has to be told which
service is meant.

The rule for *which* table — the calendar outranks a named letter, and a letter that
was never recovered is an answer rather than an exception — is `BigramTableChoice` in
`Enigma.App`, so both front ends decide it once rather than twice.

The console app has a test project of its own now, which it had never had. It covers
the two halves that matter: `NavalArguments`, where every refusal is a combination
that would otherwise be half-obeyed, and `EnigmaConsole` driven over real files, where
U-534's indicator has to key the machine at the same place a machine started at `ODFF`
outright would be. That closes the gap the previous commit had to record as open.

### A second set: "Meer"

The Crypto Museum publishes other booklets of the same kind, and **"Meer",
Prüf-Nr. 3733, is a complete one**: cover, two copies of the Tauschtafelplan front and
back, and **all nine tables, A to J**. It is the first set here to include a Tafel J
at all. A third, "Flußlauf" Prüf-Nr. 3633, runs to fifteen tables, A to P; its
calendar is transcribed and all fifteen of its tables with it, so it is complete too.

Its scan is markedly better than Quelle's: four passes a table rather than six, and
almost every cell agrees with its own mirror on a first reading, where Quelle's poorer
pages needed the involution to arbitrate.

**All nine tables are transcribed and shipped — the set is complete.** Six of the nine
came through clean on the first pass, needing no adjudication at all. Four cells across
the other three were settled the usual way, by a pair the table already agreed on:
`A/SF` read `ZK` where `ZK` pairs with `CR` and column Z reads `ZX=SF`; `B/HR` read
`BB` where `BB` pairs with `KM` and `BE=HR`; `B/YZ` read `ME` where `ME` pairs with
`LP` and `HE=YZ`; and `H/AS` read `VJ`, where an ink flaw at the foot of a T's stem
mimics a J's hook and `VT=AS` in column V settles it. `B`/`E` again, a first `M`/`H`,
and a first `J`/`T`.

**This is the first complete set in the library, and the first Tafel J of any set.**
It changes what the naval data is good for. "Quelle" is eight tables and half a
calendar: pick a day at random and there is a real chance the plan names a table that
does not survive. "Meer" is nine tables and both sides of its plan, so **every one of
the 372 cells of its calendar leads to a table that is actually here** — which is a
test, not a claim, and dropping any single table from the set fails it.

**The Meer Tauschtafelplan ships too, and it settles a question.** Both sides are
reproduced, and the reverse carries Kennziffer *sieben* to *zwölf*: a full plan has
**twelve columns, not six**. That is what the Quelle photograph's "Fortsetzung
Rückseite!" was pointing at, and why `BrunoQuelle` holds only half a plan.

**Both front ends now choose between the two sets.** `BigramTableSet` pairs a set of
tables with the calendar issued alongside it, because the two are useless apart, and
the panel offers *Satz* as one choice rather than two that could be mismatched. The
command line takes `--set`, with `--list-sets` to say what is on offer and which of
them is incomplete.

Changing set changes what the other controls can mean, and the wiring honours that:
Quelle's plan has six Kennziffer columns where Meer's has twelve, and the two sets do
not hold the same letters, so a column or a Tafel the new set cannot honour falls back
instead of pointing at nothing. Quelle stays the default — it is the set the pinned
message was sent on.

**A calendar cannot be checked the way a table can — so the Meer one was checked
against a second copy instead.** A bigram table is an involution, so a mistyped cell
breaks a pair and is caught; that is the whole basis on which the tables here are
trusted. A Tauschtafelplan has no such redundancy — it is 372 independent letters. The
check that every column uses all nine letters is weak, and a mutation test confirmed
it: changing one cell from `D` to `A` left the suite green, because the column still
used nine distinct letters.

The Crypto Museum publishes a **second Meer booklet, Prüf-Nr. 4002**, separately
photographed. Every one of the 372 cells was read again from it and compared against
the grid transcribed from 3733. **All 372 agreed.** That is a real check on the
transcription, and it is the only cipher data in this repository to have one of this
kind.

Two limits on what it proves. It verifies the *reading*, not the *edition*: an error
in the printing would appear in both copies — and would anyway be what the operators
worked from. And **`BrunoQuelle` has had no such check**: there is only one photograph
of that sheet, its pen corrections add a second layer to read, and half of it was
never photographed at all. It remains the weakest evidence here.

### A third set: "Flußlauf"

**"Flußlauf", Prüf-Nr. 3633, is a set of fifteen tables**, A to P without I, where
Quelle and Meer are nine. It ships complete: the Tauschtafelplan and every table that
plan names. The calendar was transcribed first and the tables followed, which was
deliberate — the plan is one sheet, it stands on its own, and it turned out to be the
piece that corrected an assumption in the code.

**A set is not always nine tables, and nothing may assume it is.** `TableChoice.Missing`
used to tell an operator that "the set ran to nine tables, A to J without I" — true of
both sets that then existed and false of this one. The count and the range are now read
off the set's own calendar, which is the only thing that knows them.

**Its scan is 150 ppi, half the linear resolution of the Meer booklet**, and that
changed how it had to be read. The pipeline that worked before — render the page at 600
dpi, separate the green channel, stretch the contrast — is a four-fold interpolation of
a 150 ppi original, and it invented detail: two cells of the calendar read confidently
and wrongly from it, `B` for `E` in both cases, and were caught only by going back to
the native image. Every one of its 372 cells was read from the native scan, and so was
every cell of every table.

**The grid carries a check the Meer one did not.** Fifteen letters over thirty-one days
is fourteen letters twice and one three times, and eleven of the twelve columns are set
exactly so. That is a much tighter constraint than "every column uses all nine letters",
and it earned its keep: it flagged both misreadings before anything shipped.

Kennziffer four does not conform — `K` appears once there, `D` and `O` three times
each. That column was read cell by cell a second time and the irregularity is in the
print. It is recorded as a test rather than tidied away, so that a later pass cannot
quietly "correct" the document into agreeing with a pattern.

**All fifteen tables are transcribed and shipped** — A to P, the set skipping I as these
booklets do throughout. All 676 entries each, every mirror agreeing, no bigram
enciphering to itself. Eleven of the fifteen came through with nothing to adjudicate at
all — from the *worst* scan of the three sets — which is the pipeline rather than the
paper: the page is cut on its own printed rules and each cell magnified from native
pixels, instead of being read out of an upsampled render.

Five cells across the other four tables were settled the way they are always settled
here, by the rest of the table first and the glyph at magnification second:

| Tafel | cell | read as | what settled it |
|---|---|---|---|
| C | `AT` | `WO` | `WO` pairs with `XG` both ways; `WD` reads `AT`, so the value is `WD` |
| H | `EZ` | `BP` | `BP` pairs with `XH` both ways, which leaves `EP` unclaimed |
| O | `AT` | blotted | `QI` reads `AT`, and is the one value nothing else claims |
| P | `JV` | `IX` | `LX` is unclaimed and `LX` reads `JV` |
| P | `ZY` | `AB` | `AE` is unclaimed and `AE` reads `ZY` |

Tafel C's is a new confusion for this project — the eleventh distinct letter pair. At
magnification an ink blot where the D's stem meets its bowl closes the letter into an
O, so **`D`/`O`** joins `B`/`E`, `F`/`P`, `G`/`C`, `K`/`X`, `I`/`L`, `M`/`H`, `S`/`B`,
`B`/`D`, `Y`/`V` and `J`/`T`. The involution found it, named the cell, and supplied the
answer from the mirror — which is the whole reason a table can be trusted where a
calendar cannot.

Tafel H shows the involution doing the part a careful reading cannot. `BP` was already
`XH`'s partner and `XH` was `BP`'s, so a third claim on `BP` left `EP` with none, and
the checker named the cell without being told where to look. At magnification the
middle arm of the E is plain: `EZ` is `EP`, which is `B`/`E` again, the first pair on
the list. The mutation that restores the misreading is refused at construction with the
same words that found it — *Bigram 'BP' is given as both 'XH' and 'EP'*.

Tafel O's was the cheapest of the five to settle, because nothing was read there at
all: an ink blot in the cell at `AT` had closed the gap between its two letters, leaving
a legible `Q` and a stroke beneath the blot. Once the other 675 entries are laid down,
`QI` is the only one of the 676 values nothing claims. The stroke under the blot carries
the dot of this face's **İ**, which agrees. That is the redundancy doing what a second
reader would have done, and doing it from the page rather than from a guess.

Tafel P is the only table of the set to need two cells. `JV`'s `X` is blotted and the
same ink took the foot off the letter before it, leaving a bare stroke; `ZY`'s second
letter has had its arms ink together into a bowl. Neither was guessed at. Once the other
674 entries are laid down, exactly two of the 676 values are left unclaimed — `LX` and
`AE` — and both of their mirrors are legible on the page. `B`/`E` and `I`/`L` are the
first and the fifth of the eleven confusions listed, both of them in the reader rather
than in the crop.

Tafel F needed nothing adjudicated but is worth recording all the same: it is the
densest page in the booklet for the dotted **İ** the typeface uses to keep I apart from
J. Every one of them is confirmed by its own mirror rather than by the reading alone,
and a mutation proves the point — changing that İ to an L, `I`/`L` being one of the
eleven confusions on the list, is refused at construction.

**Cutting the pages is done by a script rather than by hand**, and two things had to be
right before it could be trusted at all. The booklet **alternates white and pink
stock** — Tafel B is red ink on pink, which barely separates in luminance but holds in
the green channel, so every page is read from green whatever colour it is. And the grid
is found by fitting a *comb* of evenly spaced teeth to the ink rather than by hunting
for individual lines: the rows carry no printed rule at all, and on some pages one row's
ink splits in two or two rows merge. The comb has to be anchored to the rule across the
top of the table, because a fit one row out of step — riding the second row down and
catching the edge of the rule beneath the last one — scores *higher* on ink than the
truth does.

Which way up a leaf is cannot be told from its header. A reverse prints its Kennwort
*below* the table rather than above, and half the scans come out inverted, so the
obvious test picks the wrong rotation on exactly the pages that matter. What settles it
is inside the table: every row names one letter and gives two, so a cell carries more
ink on its right than on its left, and a page upside down reverses that.

**Then the pages took the cutter's assumptions away one at a time.** Tafel G is the
faintest pair in the booklet, and it found a fault in the cutter rather than in the
paper. A comb is rigid — one pitch across the page — and the rules are not quite that
even. On G's reverse the fit had drifted twelve pixels by the far side, which is enough
to cut the last letter off every cell in column Z; `XO` read `ME` where it is `MB`. The
involution would have caught it (`ME` already pairs with `UU`), but the cause was
geometry, not reading. The cutter now snaps each column onto the rule it is nearest, and
carries the table's unruled right-hand edge out at the pitch the printed rules actually
measure. Pages already transcribed re-cut unchanged.

Tafel K's pages arrived with a skew of about half a degree, which over the width of a
table is that same twelve pixel drift. It is corrected without resampling anything: the
grid is fitted in *sheared* coordinates, so a cell is still a plain crop of original
pixels and the page is never rotated.

Tafel L's reverse is not skewed but *bowed*, and it retired the last assumption the
cutter was making — that a leaf lies flat enough to be one plane. Its rules lean left
down one side of the page and right down the other, and its top rule arcs ten pixels
over the width while its bottom rule lies flat. No single shear straightens both, and
under one the projection smears every rule until the faintest of them — the outer ones,
always — no longer stands above a column of letters. The page came back with ten
columns where a leaf has thirteen, and the reading would have been cut from the wrong
cells if the count had happened to come out right.

So nothing global is fitted any more. Each rule is followed band by band and given its
own line, and each column of cells is given its own row comb; on that page the first
row sits eleven pixels lower under the middle columns than under the outer ones, a
third of the row pitch. What separates a rule from a column of letters is no longer
how much ink it carries — down a faded edge a rule is no darker than the letters
beside it — but how continuously: a rule has ink on every scanline it crosses and
letters have gaps between the rows. On Tafel L's *front* leaf, which is skewed rather
than bowed and which the old fit could read, the two cutters agree to within three
pixels on all 338 cells; it is only the reverse that the plane could not hold.

None of this resamples anything, which is the whole point of reading this booklet
natively: a cell is still a plain crop of original pixels, taken where that column's
grid actually runs.

**Tafels M to P are the pages the pipeline read without being changed for them.** M's
leaves are both slightly out of true and neither badly — the rules lean six pixels over
the height of the table, the first row under nine pixels out of level across the page —
and the per-column cutter that L's bowed reverse forced took them without noticing. N is
the squarest pair the booklet offered, under five pixels of each, half of what M
carried. O and P are ordinary again: nine pixels of lean, and seven and eight out of
level. Nothing in `cut.py` moved for any of them.

**The pre-use check was run every time all the same**, and on an unchanged cutter it is
not redundant: what a clean Tafel A rules out is the *page*, not the code, and it is the
page that is different every time. It was the same check each time — re-cut a page
already read by hand and confirm it reproduces it cell for cell — on both leaves of
Tafel A, their extreme columns, and never fewer than fifty-two entries against the file
that ships.

**The set shipped incomplete for as long as it took to read**, which is the case
`BigramTableChoice` exists for. Asked for 6 May on Kennziffer one it answered "Tafel P
of "Flußlauf" is not published. Its Tauschtafelplan names 15 tables, A to P without I,
and the scan that survives holds 14 of them" — rather than quietly handing back a table
it did have. That day now reaches Tafel P, and every Kennziffer and every day of
Flußlauf leads to a table that is actually here. Quelle, whose Tafel J no source
reproduces, is the only set left with a day it cannot serve, and it is where that
refusal stays under test.

Adding a third set also caught a defect in the panel: the Tafel dropdown labelled
every option "Quelle" whatever set was chosen, so Meer's Tafel A had been offered
under Quelle's name since the set picker landed. The option now names its own set.

## Future

**Cryptanalysis** — bombe simulation, index-of-coincidence rotor search, and
ciphertext-only attack. This is deliberately placed after the simulator is
complete rather than alongside it: an attack is only worth writing against a
machine already known to be right, and a wrong machine would make a broken attack
look successful.

## Not scheduled

- **Packaging and continuous integration.** The tool builds and installs; nothing
  publishes it to NuGet and no CI runs the suite. Worth doing, not yet planned.
- **Mechanical detail with no effect on the cipher.** The G-31's hand crank and
  backward stepping, its four-digit letter counter, the 52-tooth cogwheel. These
  are properties of the object rather than of the cipher, and a simulator that
  reproduced them would not encipher a single letter differently.
