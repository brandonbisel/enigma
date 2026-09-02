# N-gram scoring, and the steckered break

**Status: blocked on a source.** Not on design, and not on work.

## What it would close

Two things, and they are the same thing seen twice.

**The plugboard.** The wheel search recovers the machines with no board and cannot
touch a service Enigma. The reason is not that the index of coincidence is weak in
general but that the board sits inside the rotor sandwich: running the machine with
the cables out does not relabel the plaintext, it shreds it, and with eight cables
only about one letter in seven comes through. At the true setting of the Graf Spee
signal the unsteckered decipherment scores 0.040 against a random 0.038, while the
best *wrong* setting in the same sweep reaches 0.049. The signal is real and it is
smaller than the noise.

**The fast wheel's ring.** The first phase leaves every ring at A, which gets the
turnovers wrong; a fast wheel whose ring really sits far from A puts the true setting
at the rate of text with no structure at all, so it is not ranked low but invisible.
That is most of the failures in the measured rate.

A measure that knows what German *looks like*, rather than merely that it has
structure, sees through both: a wrong turnover part-way through a message still leaves
readable German in front of it, and a shredded decipherment does not look like German
at any setting. Both are in
[breaking a message](../cryptanalysis.md) with the numbers.

## What is already in place

`IScore` exists for exactly this. It is an interface because the measure is the part
of an attack most likely to be replaced, and `RotorSearch` takes one, so a second
measure is a new class and not a rewrite of the search. `IndexOfCoincidence` is what
ships, and it ships because it needs no corpus.

So the code side of this is small. What is missing is the data.

## What is missing

A table of German n-gram statistics: bigrams, trigrams, or the hexagram scoring
Ostwald and Weierud used. Under
[the one rule](../sources.md#the-standard-everything-here-is-held-to) it may arrive in
one of two ways and no other:

- **published** — a table given in a paper or a reference implementation, cited the
  way every wiring here is cited; or
- **derived** — computed from a *named* corpus, with the derivation script shipped in
  the repository the way `tools/transcribe` is, so that anyone can run it and get the
  same table.

What may not happen is a table generated from a model's own idea of German. That is
the same failure as the hand-written Uhr table, which was deleted rather than patched
into agreement, and it would be worse here: a wrong scoring table does not fail a
test, it quietly reports the wrong break as the right one.

## What the work would be, once there is a table

1. A second `IScore` over the table. Stateless per call, because it is called from
   every thread of a search.
2. A test that the new measure ranks the true setting of an already-broken message
   above every wrong one — the historical intercepts are the material, since their
   plaintexts are published.
3. The plugboard phase, which is what the measure was for: Gillogly's hill-climb, one
   cable at a time, keeping a cable that improves the score. This is the part of his
   paper that is deliberately not implemented today.
4. The measured rate, redone. The existing figures — 15 of 24 unsteckered settings
   recovered — are the baseline the new measure has to beat, and the comparison is
   worth writing down whichever way it falls.

## What must not change

`TheIndexOfCoincidenceCannotBreakASteckeredServiceMachine` and
`ARingSettingFarFromAHidesTheTrueSettingFromTheFirstPhase` pin the *current* measure's
limits. They are statements about the index of coincidence, not about the library's
ambitions, so they stay true and stay green after this work lands. A change that makes
either of them fail has changed what the index of coincidence does, which is a bug.

The new measure gets its own tests, and its own honest statement of where it stops.
