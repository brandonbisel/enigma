# Roadmap

What is planned, what is waiting on evidence, and what is deliberately left alone.

**Finished work is not listed here.** It is described in [README.md](README.md) and
documented in full in [docs](docs/); the roadmap entry for a thing disappears when the
thing exists. A roadmap that accumulates completed items stops being a roadmap and
becomes a changelog nobody reads, and git already keeps that history. For how to work
in the repository see [AGENTS.md](AGENTS.md).

## What "done" means here

Everything below inherits
[the sourcing rule](docs/sources.md#the-standard-everything-here-is-held-to): no cipher
data ships unless it is pinned to a published source or a published vector. That is
what puts several of these items in "waiting on evidence" rather than in "waiting to be
written".

An item is not done when it runs. It is done when something published says it runs
correctly.

## Next

### The two gaps in the naval data

- **Quelle's Tafel J.** In no source found so far. It is the only table any of the
  three calendars names that is not here.
- **The reverse of the Quelle Tauschtafelplan.** Kennziffer seven to twelve, on a side
  that was never photographed. Meer's plan shows what those columns look like;
  Quelle's are not recoverable from it.

Both are sourcing problems rather than work, and neither will be filled by inference.
What is already read, and how far each reading is trusted, is in
[docs/bigram-tables.md](docs/bigram-tables.md).

### More machines

`IMachineLayout` and `IStepping` reduced a new model to *parts plus a layout* — the
Z30 needed a character map, a set of wheels and one new drive, and nothing else. What
remains is therefore a matter of sourcing wirings rather than writing mechanism, and
the rule above decides which of them can ship. The models built so far are in
[docs/machines.md](docs/machines.md).

## Future

### Better scoring, and with it the steckered break

The wheel search breaks the machines with no plugboard and cannot touch a service
Enigma. Closing that gap needs a German n-gram table — published, or derived from a
named corpus with the derivation shipped the way `tools/transcribe` is. That is data,
so it waits on a source exactly as the Uhr table did.

**Plan: [docs/plans/ngram-scoring.md](docs/plans/ngram-scoring.md).** Where the search
stops today, and by how much, is in [docs/cryptanalysis.md](docs/cryptanalysis.md).

### Bombe simulation

Crib-driven rather than statistical, and so not blocked on that sourcing question at
all. The intention is the machine rather than the result: menus, the twelve Enigma
equivalents, the diagonal board, and stops an operator checks.

**Plan: [docs/plans/bombe.md](docs/plans/bombe.md).**

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
  backward stepping, its four-digit letter counter, the 52-tooth cogwheel. These are
  properties of the object rather than of the cipher, and a simulator that reproduced
  them would not encipher a single letter differently.
