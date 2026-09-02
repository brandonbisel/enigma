# Breaking a message

`Enigma.Analysis` attacks the machine instead of running it. What it recovers, what it
costs, what it cannot touch and why — all of it measured, and all of it pinned by
tests rather than asserted here. The short version is in
[the README](../README.md#breaking-a-message).

## The wheel search

Given ciphertext and nothing else, `RotorSearch` sweeps every arrangement of the
wheels at every starting position with the board empty, scores each decipherment by
its **index of coincidence**, and lets the right setting rise; then it sweeps the
ring settings that decide where in the message the wheels carry each other over, and
the alignment again against those, until neither improves. It is Gillogly's method,
and it runs the real machine — the same factory, the same key sheets, no faster
private copy of the cipher.

```bash
dotnet run --project Enigma.Cmd -- --key-sheet machine.json --recover \
    --wheels "K-I K-II K-III" --candidates 3 < cipher.txt
```

The key sheet says what is already known — the model, the alphabet, the reflector —
and the search finds the rest. Four hundred letters of German enciphered on an
Enigma K at `K-II K-III K-I`, Ringstellung QRS, Grundstellung QMT:

```
Searching 6 arrangements, 105,456 settings, over 400 letters.
 1  0.05529  rotors K-II K-III K-I, reflector G, Ringstellung ABS, Grundstellung BXT
 2  0.04397  rotors K-III K-I K-II, reflector G, Ringstellung AER, Grundstellung XKZ
 3  0.04248  rotors K-I K-II K-III, reflector G, Ringstellung ACZ, Grundstellung ETV
```

The wheel order is right and the gap to the runner-up is not close. The setting is
not the one written on the sheet, and it does not need to be — set the machine to it
and the message comes out:

```
XKTZWOABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZXSEBEZXUAFFLIEGERSTRASZE...
AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZXSEBEZXUAFFLIEGERSTRASZE...
```

Six letters of garble and then the signal, because the recovered setting sits one
letter from the true one and the two machines converge at the first turnover. That is
what a break looks like, and it is why `SettingsEquivalence` asks three different
questions rather than comparing strings: `ReadTheSame` for one message, `AgreeOn` for
how much of it, and `AreOneMachine` for whether any message of any length could tell
two settings apart.

## What it costs

Measured, on 272 letters and 32 cores:

| Search | Arrangements | Settings | Time |
|---|---|---|---|
| Enigma I, the five Wehrmacht wheels | 60 | 1,054,560 | 17s |
| M3, all eight naval wheels | 336 | 5,905,536 | 75s |

That is a hundred million and more passes through a real Enigma, which is where the
time goes. It is also why there is no faster private copy of the cipher inside the
search: the machine everything else is checked against is the machine being swept, so
where speed was needed it was the machine itself that got faster — the wirings each
wheel reads are flattened from the dictionary they are checked as, once, and the
existing suite is what proves that changed nothing.

## What it breaks

**The machines with no plugboard.** The Enigma D and K, the Swiss K, the Railway and
Tirpitz machines, the Zählwerk G-31, the Z30 — which is most of what this library has
grown, and the machines for which no intercept survives to pin a break against.

How often, measured rather than asserted: over **24 random settings** of an
unsteckered Enigma I, 400 letters of German each, searched across all 60 orderings of
the five Wehrmacht wheels, it recovered the wheel order **15 times**. In every one of
those 15 the setting it returned read at least 378 of the 400 letters, and in 8 it
read all 400. The other 9 read fewer than 30. There is nothing in between: it either
breaks the message or it does not come close, so a result needs no judging — you can
see which happened by reading it.

**Not a service Enigma.** Not the Wehrmacht's ten cables, and not the Kriegsmarine's
eight. The reason is worth stating because it is easy to get backwards: the index of
coincidence is untouched by *relabelling* the alphabet, which is why one might expect
it to see through a plugboard — but the board sits **inside** the rotor sandwich, not
outside it. Running the machine with the cables pulled out does not relabel the
plaintext, it shreds it. Only the letters that were unsteckered at both ends come
through, and with eight cables that is about one character in seven.

The numbers, on the Graf Spee signal — 272 letters, the longest three-wheel message
here and the one on the fewest cables:

| | index of coincidence |
|---|---|
| Text with no structure | 0.0385 |
| The true setting, cables pulled out | 0.0401 |
| The best *wrong* setting the sweep finds | 0.0490 |
| The true setting, cables in | 0.0618 |

The signal is real and it is smaller than the noise. A test pins this, so that a
later change cannot quietly claim a break that has not happened. Closing the gap
needs a measure that knows what German looks like rather than merely that it has
structure — bigrams or trigrams — and that is a table of statistics, which is data,
which under this repository's one rule needs a published source. See
[the plan for it](plans/ngram-scoring.md).

## The other blind spot

The first phase leaves every ring at A and sweeps the positions, which covers every
alignment of the wiring but gets the turnovers wrong. How wrong depends on the
Ringstellung it is looking for: a fast wheel whose ring really sits at Q has its
turnovers sixteen keypresses out of phase, and the true setting then scores 0.0385 —
the rate of text with no structure at all. It is not ranked low, it is invisible.

What survives is the wheel *order*, which still ranks at or near the top because the
wiring is right even when the stepping is not. So the second phase is handed the best
of each wheel order rather than the best few settings overall, and climbs from a
mediocre starting point to the setting itself. That is also a test.

It is also most of the nine failures above. Sweeping the fast wheel's ring in the
first phase as well would fix it and would cost twenty-six times the work, which on a
full eight-wheel sweep is hours rather than seconds. The cheaper fix is a measure that
can see the true setting through a wrong turnover — the same n-gram scoring that the
plugboard needs. Both roads out of here lead to the same missing table.

## Where it goes next

Both blind spots above lead to the same missing thing: a measure that knows what
German looks like rather than merely that it has structure. That is a table of
statistics, which is data, which under this repository's one rule needs a published
source or a named corpus with the derivation shipped. It waits on one, exactly as the
Uhr table did — [the plan is written down](plans/ngram-scoring.md).

A [bombe simulation](plans/bombe.md) is crib-driven rather than statistical, and so is
not blocked on that sourcing question at all.

## See also

- [The command line](command-line.md) for every `--recover` option.
- [The machines](machines.md) for the unsteckered models this does break.
- [Sources](sources.md#cryptanalysis) for Gillogly's paper, which is the method.
