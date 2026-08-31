"""What makes a transcription trustworthy.

A bigram table is an involution with no fixed point, so a table checks itself: 676
cells are read independently and every one of them has to agree with the cell that
mirrors it. A single misread letter breaks a pair and is named. This is the whole
basis on which the tables here are trusted, and it is why a Tauschtafelplan -- 372
independent letters with no such redundancy -- had to be checked against a second
copy of the booklet instead.

`compare` is the other half: it holds a fresh reading against one already shipped,
which is how the cutter is verified before it is used on a page nobody has read.
"""
import sys
from collections import defaultdict

from emit import BIGRAMS, load


def faults(table):
    """Everything wrong with a reading, as lines of prose."""
    found = []

    missing = [b for b in BIGRAMS if b not in table]
    if missing:
        found.append(f"{len(missing)} bigrams are not given: {' '.join(missing[:12])}"
                     + (" ..." if len(missing) > 12 else ""))

    for bigram in sorted(b for b in table if table[b] == b):
        found.append(f"{bigram} enciphers to itself")

    claimed = defaultdict(list)
    for key, value in table.items():
        claimed[value].append(key)
    for value, keys in sorted(claimed.items()):
        if len(keys) > 1:
            found.append(f"{value} is given as the value of both "
                         + " and ".join(sorted(keys)))

    for key in sorted(table):
        mirror = table.get(table[key])
        if mirror is not None and mirror != key:
            found.append(f"{key} = {table[key]} but {table[key]} = {mirror}")

    return found


def compare(one, other):
    """Where two readings of the same table disagree."""
    return [f"{b}: {one.get(b, '--')} against {other.get(b, '--')}"
            for b in BIGRAMS if one.get(b) != other.get(b)]


if __name__ == "__main__":
    if len(sys.argv) not in (2, 3):
        raise SystemExit("usage: check.py READING.txt [ALREADY-SHIPPED.txt]")

    table = load(sys.argv[1])
    problems = faults(table)

    if len(sys.argv) == 3:
        problems += compare(table, load(sys.argv[2]))

    if problems:
        print(f"{len(problems)} to settle:")
        for line in problems:
            print(f"  {line}")
        raise SystemExit(1)

    print(f"{len(table)} entries, every mirror agreeing, no fixed point.")
