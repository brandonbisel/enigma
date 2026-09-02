# The bigram tables

The Doppelbuchstabentauschtafeln the naval indicator procedure runs on, and the
calendars issued with them. Three of the Crypto Museum's booklets are transcribed and
ship. A set is a set of tables plus the Tauschtafelplan — the calendar that says which
table a given Kennziffer column uses on a given day of the month.

| Set | Booklet | Tables | Calendar |
|---|---|---|---|
| Quelle | Prüf-Nr. 2499 | 8 of 9 — A to H | front only: Kennziffer one to six |
| Meer | Prüf-Nr. 3733 | 9 — A to J without I | both sides: one to twelve |
| Flußlauf | Prüf-Nr. 3633 | 15 — A to P without I | both sides: one to twelve |

Thirty-two tables in all. What they are *for* is in
[the indicator procedures](indicator-procedures.md#the-naval-procedure).

## What ships

`BigramTables.Quelle` holds Tafels A to H of the set "Quelle", booklet Prüf-Nr. 2499 —
the set U-534 was using on 1 May 1945, and every table its scan contains. The cover
says the edition held nine, "A bis J ohne I", but Tafel J is not reproduced.

`BigramTables.Meer` holds all nine of the set "Meer", booklet Prüf-Nr. 3733, A to J
without I. It is the complete edition: nine tables and both sides of its
Tauschtafelplan, so every day of its calendar leads to a table that is actually
here.

`BigramTables.Flusslauf` holds all fifteen of the set "Flußlauf", booklet Prüf-Nr. 3633,
which is a *fifteen* table edition — A to P without I — and not a nine table one.
Its calendar has twelve Kennziffer columns and is transcribed in full, and so are
Tafeln A to P, so every day of it leads to a table that is actually here. It is the
second complete set. Its scan is the poorest of the three at 150 ppi, half the
linear resolution of Meer's, which is why the pages are cut on their own printed
rules and each cell magnified from native pixels rather than read out of an enlarged
render. `tools/transcribe` holds that pipeline.

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

## How a reading is trusted

A transcription of a table is worth more than a transcription of a wiring, because it
can be checked. All 676 entries are present, every one pairs with its mirror, and no
bigram enciphers to itself — a single mistyped cell would break a pair and show up.
Eighteen cells did, across Quelle's eight tables, and each time the rest of the table
said what the cell had to be — then the glyph at magnification agreed. `B`/`E` is the
commonest slip and `F`/`P` the next. Eight of the eighteen are in Tafel G alone,
whose two pages are the poorest scan in the booklet. Meer's nine tables needed four
such cells and Flußlauf's fifteen needed five, which is twenty-seven over the three
sets and not one of them guessed at: the mirror said what each had to be, and only
then was the glyph looked at again. Six further cells of Quelle's Tafel E are covered
by later hand-written ink and could not be read at all; they were rebuilt from their
mirrors, which on an involution is the same fact written twice. On top of all that,
the four cells the U-534 message needed were published separately with the working of
that message, and they agree.

That is what makes reading one by hand worth doing, and where a reading has been
wrong the mirror has also supplied the answer. Every adjudicated cell is recorded,
with the argument that settled it, in its file under `Enigma.Tests/Data`; the pipeline
that puts native pixels in front of a reader, and the eleven letter pairs this
typeface confuses, are in [tools/transcribe](../tools/transcribe/README.md).

The whole message now runs end to end in the test suite: the transmitted indicator
`FNHC GVET`, through the real table, to the message key `ODFF` the operator wrote on
his sheet — and back again.

Note what the check does and does not do: it catches transcription error, not
fabrication, so a table still only ships against a scan.

## The calendars

Each set ships with its own. `Tauschtafelplan.BrunoQuelle` is the *Tauschtafelplan
"Bruno"* for the Quelle booklet, transcribed from Michael Hörenberg's photograph of
the original: thirty-one days by six Kennziffer columns, the column chosen by cipher
net.
It agrees with the traffic at the one point it can be checked — U-534 sent on Tafel A
on 1 May 1945, and the column pencilled "Mai 45" reads `A` on the first.

```csharp
var tafel = Tauschtafelplan.BrunoQuelle.Tafel(kennziffer: 6, dayOfMonth: 1);  // 'A'
```

The other two are better sheets. `BrunoMeer` and `BrunoFlusslauf` are clean print
with no pen corrections, and both sides of the Meer one are reproduced — which is
what shows that a full plan has **twelve** Kennziffer columns, not six. The Quelle
photograph stops at six and is footed "Fortsetzung Rückseite!"; `BrunoQuelle` is half
a plan, and remains the weakest evidence here.

**A calendar cannot be checked the way a table can**, because it has no redundancy:
it is 372 independent letters. The check that every column uses all nine letters is
weak, and a mutation test confirmed it — changing one cell from `D` to `A` left the
suite green. So the Meer calendar was checked against a **second Meer booklet,
Prüf-Nr. 4002**, separately photographed: all 372 cells were read again from it and
**all 372 agreed**. That is the only cipher data in this repository with a check of
that kind. It verifies the reading and not the edition — an error in the printing
would appear in both copies, and would anyway be what the operators worked from.

The Flußlauf calendar carries a check of its own. Fifteen tables over thirty-one days
is fourteen letters twice and one three times, and eleven of its twelve columns are set
exactly so — a much tighter constraint than nine letters per column, and it earned its
keep by flagging two misreadings before anything shipped. Kennziffer four does not
conform: `K` appears once there, `D` and `O` three times each. That column was read
cell by cell a second time and the irregularity is in the print. It is recorded as a
test rather than tidied away, so that a later pass cannot quietly "correct" the
document into agreeing with a pattern.

**Why `BrunoQuelle` is the weakest evidence here.** There is only one photograph of
that sheet, half of it was never taken, and its pen corrections add a second layer to
read: every printed `C` and `H` in columns one, four and six is struck out and replaced,
consistently per column. The natural reading is that Tafeln C and H were withdrawn —
but the sheet does not say so, and the library records the two layers rather than the
inference.

## The two gaps

- **Quelle's Tafel J.** In no source found so far. It is the only table any of the
  three calendars names that is not here. Days its calendar sends there have no table
  to offer, and say so rather than substituting one.
- **The reverse of the Quelle Tauschtafelplan.** Kennziffer seven to twelve, on a side
  that was never photographed. Meer's plan shows what those columns look like;
  Quelle's are not recoverable from it.

Both are sourcing problems rather than work, and neither will be filled by inference.
They are the two open items in [the roadmap](../ROADMAP.md).

## See also

- [The indicator procedures](indicator-procedures.md) for the procedure these tables
  serve, at the command line and in the panel.
- [tools/transcribe](../tools/transcribe/README.md) for the pipeline that turns a
  scanned booklet into a data file, and the eleven letter pairs its typeface confuses.
- [Sources](sources.md) for where the scans and the worked messages come from.
