"""Turning a reading into the two forms the repository keeps it in.

The data file under Enigma.Tests/Data is the transcription written through in
full, one line per half-row, and is the human-readable record. The C# constant in
Enigma/BigramTables.cs is the same table written once per pair, because each entry
implies its own reverse; BigramTable.Parse expands it and refuses any disagreement.
"""
import re
import sys

LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
BIGRAMS = [a + b for a in LETTERS for b in LETTERS]
PER_DATA_LINE = 13
PER_CONSTANT_LINE = 14


def load(path):
    """Every substitution written in a file, comments and layout ignored."""
    table = {}
    for line in open(path, encoding="utf-8"):
        line = line.split("#", 1)[0]
        for key, value in re.findall(r"\b([A-Z]{2})=([A-Z]{2})\b", line):
            if key in table and table[key] != value:
                raise SystemExit(f"{path}: {key} is given as both {table[key]} and {value}")
            table[key] = value
    return table


def data_file(table, header):
    """The table written through in full, under its provenance note."""
    lines = list(header)
    for i in range(0, len(BIGRAMS), PER_DATA_LINE):
        lines.append(" ".join(f"{k}={table[k]}" for k in BIGRAMS[i:i + PER_DATA_LINE]))
    return "\n".join(lines) + "\n"


def constant(table, name, indent="    "):
    """The table written once per pair, as a C# constant.

    Note the trailing space inside every segment but the last: the compiler joins
    the string literals with nothing between them, so without it the last entry of
    one line and the first of the next run together into a single unreadable pair.
    """
    once = [f"{k}={table[k]}" for k in BIGRAMS if k <= table[k]]
    lines = [" ".join(once[i:i + PER_CONSTANT_LINE])
             for i in range(0, len(once), PER_CONSTANT_LINE)]

    out = [f"{indent}private const string {name} ="]
    for i, line in enumerate(lines):
        last = i == len(lines) - 1
        out.append(f'{indent}{indent}"{line}{"" if last else " "}"' + ("" if last else " +"))
    out[-1] += ";"
    return "\n".join(out) + "\n"


if __name__ == "__main__":
    if len(sys.argv) != 3 or sys.argv[2] not in ("data", "constant"):
        raise SystemExit("usage: emit.py READING.txt data|constant")
    table = load(sys.argv[1])
    if sys.argv[2] == "data":
        sys.stdout.write(data_file(table, []))
    else:
        sys.stdout.write(constant(table, "Entries"))
