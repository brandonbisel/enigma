# Bombe simulation

**Status: not started. Not blocked.**

The bombe is crib-driven rather than statistical, so unlike
[n-gram scoring](ngram-scoring.md) it waits on no table of data at all. Everything it
needs — the machine, the wheels, the wirings — is already here and already pinned by
traffic.

## The intention

**The machine rather than the result.** A program that recovers a key by any means is
not what this would be; a program that recovers it *the way the bombe did* is. That
means the parts are modelled as parts:

- **The menu.** A crib laid against ciphertext, the letter pairs it implies, and the
  loops among them that make a menu worth running. Building a menu by hand from a
  known intercept comes first, because it is the thing the machine consumes.
- **The twelve Enigma equivalents.** Each a scrambler with no plugboard, wired in the
  order the menu dictates. These are the library's own machines, driven by the same
  factory as everything else — the rule that
  [an attack drives the real machine](../../AGENTS.md#cryptanalysis) applies here as
  it does to the wheel search.
- **The diagonal board.** Welchman's addition, and the thing that makes the difference
  between a machine that stops constantly and one that is useful.
- **Stops, and checking them.** A stop is a hypothesis, not an answer. The operator
  checked it, most of them failed, and a simulation that hides that has simulated the
  wrong thing.

## How it would be pinned

Against **published worked menus** — a crib, a menu drawn from it, and the stops that
menu produces, all given in a source. The standard is the same as everywhere else
here: the simulation is not done when it runs, it is done when something published
says it runs correctly.

The historical intercepts already in the suite are the natural material for the
end-to-end test, because their plaintexts are published and so a crib can be taken
from one honestly.

## What is not decided

- **Where it lives.** `Enigma.Analysis` holds the wheel search and depends only on
  `Enigma`. Whether the bombe belongs beside it or in a project of its own is an open
  question, and worth deciding by what it needs rather than in advance.
- **Which source pins the menus.** Nothing has been chosen. That choice is the first
  piece of work, not the last.
- **How far the operator's side is modelled.** Stops have to be checkable; whether the
  checking machine is also modelled is a question about scope rather than about
  correctness.

## Why it was deferred, and whether that reasoning held

The original roadmap entry placed an attack after the simulator, on the grounds that
"an attack is only worth writing against a machine already known to be right, and a
wrong machine would make a broken attack look successful."

That judgement held up. Every failure found while writing the wheel search turned out
to be in the search or in the measure, and the machine underneath was never in
question. The same protection carries over to this.
