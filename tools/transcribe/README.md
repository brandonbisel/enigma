# Transcribing a Doppelbuchstabentauschtafel

These scripts turn a page of one of the Crypto Museum's scanned bigram booklets
into the two forms this repository keeps a table in: the record under
`Enigma.Tests/Data`, and the constant in `Enigma/BigramTables.cs`.

They do not read the letters. A person does that, off magnified sheets of cells.
Everything here is about putting native pixels in front of that person and about
checking what comes back.

## Why it works this way

The Flußlauf booklet is scanned at 150 ppi, half the linear resolution of the Meer
one. Rendering a page larger and reading off that invents detail — two cells were
once read confidently and wrongly from a four-fold interpolation, `B` for `E` in
both cases, and were caught only by going back to the native image. So no step
here resamples, and pages are never rotated. Where a leaf is out of true the grid
is fitted to *where it actually runs* and a cell comes back as a plain crop.

Nothing global is fitted, because a leaf pressed into a scanner is not a plane.
Tafel L's reverse bows: its rules lean left down one side of the page and right
down the other, and its top rule arcs ten pixels over the width while its bottom
rule lies flat. So each rule is followed band by band and fitted its own line, and
each column of cells gets its own row comb.

Pages are read from the green channel whatever colour the stock is. The booklet
alternates white and pink, and red ink on pink barely separates in luminance.

What tells a rule from a column of letters is not how much ink it carries but how
continuously: a rule has ink on every scanline it crosses, and letters have gaps
between the rows. Height alone cannot separate the two — down the faded edge of a
leaf a rule projects no darker than the letters beside it, which is what left
Tafel L's reverse three rules short of a grid.

## The run

Needs `pdfimages` (poppler), Pillow and NumPy.

```bash
cd tools/transcribe

# One leaf of the booklet, upright, at the size the scanner made it.
python3 pages.py DoppelTafeln_Flusslauf_3633_992.pdf 22 front.png
python3 pages.py DoppelTafeln_Flusslauf_3633_992.pdf 23 back.png

# Check the grid before trusting it: 13 columns and a row pitch near 30.5. It also
# reports how far the leaf is out of true, which is context for the cells to come.
python3 cut.py front.png back.png

# Sheets of magnified cells to read from. The last argument says where the leaf
# starts in the alphabet: 0 for a front, 13 for a reverse.
python3 sheet.py front.png K 0
python3 sheet.py back.png K 13

# Write the readings into a file -- any layout, "AA=TD" pairs are all that is
# read out of it -- and then hold it to account.
python3 check.py reading.txt

# Both output forms.
python3 emit.py reading.txt data
python3 emit.py reading.txt constant
```

`emit.py data` prints the body of the data file; put the provenance note above it
by hand, naming the booklet and the two page numbers, as the existing files do.

## What is actually verified

**Verify the cutter before using it on a page nobody has read.** Cut a table that
is already transcribed, read a column or two off the sheets, and check them
against its data file — the extreme columns especially, because that is where the
cutter has failed before. On Tafel G a rigid comb drifted twelve pixels across the
page and cut the last letter off every cell in column Z; on Tafel L the whole grid
was fitted as a plane and the bowed leaf would not sit in one.

```bash
python3 check.py fresh-reading.txt ../../Enigma.Tests/Data/flusslauf_A.txt
```

**The involution is what makes a table trustworthy.** It is reciprocal and has no
fixed point, so 676 independently read cells all have to pair up; one misread
letter breaks a pair and `check.py` names the cell. Where a reading has been wrong
the mirror has also supplied the answer — `AT` read `WO` on Tafel C, but `WO` was
spoken for by `XG` and `WD` read `AT`, so the value had to be `WD`, and at
magnification an ink blot had closed a `D` into an `O`.

A Tauschtafelplan has no such redundancy — it is 372 independent letters — which is
why the Meer calendar had to be checked against a second copy of the booklet
instead, and why `BrunoQuelle` remains the weakest evidence here.

## Letters this typeface confuses

`B`/`E`, `F`/`P`, `G`/`C`, `K`/`X`, `I`/`L`, `M`/`H`, `S`/`B`, `B`/`D`, `Y`/`V`,
`J`/`T` and `D`/`O`. Note also the dotted **İ**, which the face uses to keep I
apart from J.
