# Documentation

The README is the tour: what this is, and enough of each feature to use it. These are
the long forms — one document per feature, with the detail, the reasoning and the
evidence that would otherwise crowd the tour out.

## Working the machine

| Document | What is in it |
|---|---|
| [Key sheets](key-sheets.md) | The settings file in the notation a real sheet used, the M4's fitment rules, and the packaged historical sheets |
| [The machines](machines.md) | Every model the library builds: the service Enigmas, the commercial D and K, the Zählwerk G-31, the Z30 and the rest |
| [The plugboard, the Uhr and UKW-D](plugboard.md) | The three things a key sheet could change about the wiring itself |
| [Parts and alphabets](parts-and-alphabets.md) | The entry wheel, machines that do not work in letters, and defining wheels of your own |
| [The indicator procedures](indicator-procedures.md) | How each service told the receiver where its rotors started, worked both ways |
| [Writing a message out](message-text.md) | Fitting German to a keyboard with no space bar, and the five-letter groups it was sent in |
| [The bigram tables](bigram-tables.md) | The three sets that ship, how each was read off its scan, and how far each is to be trusted |

## Front ends

| Document | What is in it |
|---|---|
| [The command line](command-line.md) | Every option, grouped by what it is for |
| [The panel](panel.md) | The browser front end, part by part |
| [Using the library](library.md) | Building a machine in code, the session, the trace, and how the suite is put together |

## Attacking it

| Document | What is in it |
|---|---|
| [Breaking a message](cryptanalysis.md) | The wheel search: what it recovers, what it costs, and the two places it stops |

## Behind all of it

| Document | What is in it |
|---|---|
| [Sources](sources.md) | Where every wiring, vector, table and intercept came from |
| [tools/transcribe](../tools/transcribe/README.md) | Reading a bigram booklet off its scan |
| [Plans](plans/) | The detail behind what the roadmap has not built yet |

## The convention

A feature is documented in three places, and each has one job:

- **[README.md](../README.md)** describes it the way a user meets it: what it does,
  what it looks like, one example. A few paragraphs at most, and a link to here.
- **A document in `docs/`** carries the rest — the reasoning, the evidence, the
  measured numbers, the edge that surprised somebody.
- **[ROADMAP.md](../ROADMAP.md)** does not mention it at all, because it is finished.
  The roadmap is for what is *not* built; detailed plans for those live in
  [docs/plans](plans/), and a plan is deleted when its feature is documented here.

The point of the last one is that a roadmap accumulating completed items stops being
a roadmap and becomes a changelog nobody reads. Git already keeps that history.
