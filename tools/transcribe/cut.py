"""Locating the grid on a page of a Doppelbuchstabentauschtafel.

The booklets are scanned at 150 ppi, half the linear resolution of the Meer one,
and rendering them larger invents detail: two cells were once read confidently and
wrongly from a four-fold interpolation. So nothing here resamples. A cell comes
back as a plain crop of original pixels, and a leaf that is out of true is followed
rather than straightened.

Followed, because a leaf pressed into a scanner is not a plane. Tafel L's reverse
bows: its rules lean left down one side of the page and right down the other, and
its top rule arcs ten pixels over the width while its bottom rule lies flat. No one
shear straightens that, and projecting the whole page under one smears every rule
until the faintest of them -- the outer ones, always -- no longer stands above a
column of letters. So nothing global is fitted. Each rule is followed down the page
and given its own line, and each column of cells is given its own row comb.

The page is read from the green channel whatever colour the stock is. The booklet
alternates white and pink, and red ink on pink barely separates in luminance --
the first attempt at this found 26 evenly spaced rows that were not the rows.

What tells a rule from a column of letters is not how much ink it carries but how
continuously: a rule has ink on every scanline it crosses, and letters have gaps
between the rows. Height alone cannot separate the two, because down the faded edge
of a leaf a rule projects no darker than the letters beside it. So the ink is
thresholded first and what is measured is the fraction of a run that is covered.
"""
import numpy as np
from PIL import Image

COLUMNS = 26          # the table is 26 wide, but a leaf carries half of it
COLUMNS_PER_LEAF = 13
ROWS = 26
BANDS = 6             # strips of the page a column rule is followed through
TOOTH = 13            # how wide a tooth of the row comb samples


class Unreadable(Exception):
    """No grid could be found on a page -- most often one that is the wrong way up."""


def ink(path):
    """Ink strength: how far below the paper each pixel sits in green."""
    green = np.asarray(Image.open(path).convert("RGB")).astype(float)[:, :, 1]
    return np.clip(np.median(green) - green, 0, None)


def inked(image):
    """The page as ink or no ink, at a level the page's own darkest ink sets."""
    return (image > np.percentile(image, 99) * 0.35).astype(float)


def row_projection(image, shear):
    """Sum along rows, lifting each column by `shear` per pixel of x."""
    height, width = image.shape
    pad = int(abs(shear) * width) + 2
    total = np.zeros(height + 2 * pad)
    for x in range(width):
        offset = int(round(shear * x))
        total[pad - offset:pad - offset + height] += image[:, x]
    return total, pad


def _bands(indices, gap=4):
    runs = []
    for i in indices:
        if runs and i - runs[-1][-1] <= gap:
            runs[-1].append(i)
        else:
            runs.append([i])
    return runs


def _peaks(profile, threshold, pad):
    found = []
    for band in _bands(list(np.flatnonzero(profile > threshold))):
        weight = profile[band]
        found.append((float(np.sum(np.array(band) * weight) / weight.sum()) - pad,
                      float(weight.max())))
    return found


def _at(line, y):
    """Where a line of the grid sits at height `y`."""
    offset, slope = line
    return offset + slope * y


def _bounding_rules(covered):
    """The rule above the table and the one below it, each at its own slope.

    They are the only ink that runs the width of the page, but they are not
    parallel: on a bowed leaf one arcs and the other lies flat. One slope fitted to
    both leaves the arced one smeared and barely detected, so each is taken at
    whatever slope makes it sharpest, and only where it sits is kept.
    """
    width = covered.shape[1]
    found = []
    for shear in np.arange(-0.020, 0.0201, 0.0005):
        total, pad = row_projection(covered, shear)
        found += [(height, centre)
                  for centre, height in _peaks(total / width, 0.45, pad)]
    found.sort(reverse=True)

    if not found:
        raise Unreadable("nothing runs the width of the page")
    _, first = found[0]
    apart = [centre for _, centre in found if abs(centre - first) > 400]
    if not apart:
        raise Unreadable("only one rule runs the width of the page")
    return sorted((first, apart[0]))


def _column_rules(covered, top, bottom):
    """Every rule between the columns, as a line x = offset + slope * y.

    A rule is followed band by band rather than projected down the whole page: it
    does not run straight, and on a leaf that bows a full-height projection smears
    it away. Within one band it hardly moves, and the only thing there with ink on
    nearly every scanline is a rule. A band or two may still lose one where the ink
    has faded, so a rule is taken on the majority of them rather than on all.
    """
    cuts = np.linspace(top + 6, bottom - 6, BANDS + 1)
    seen = []
    for i in range(BANDS):
        band = covered[int(cuts[i]):int(cuts[i + 1]), :]
        middle = (cuts[i] + cuts[i + 1]) / 2
        seen += [(x, i, middle) for x, _ in _peaks(band.mean(axis=0), 0.7, 0)]

    tracks = []
    for point in sorted(seen):
        if tracks and point[0] - tracks[-1][-1][0] <= 14:
            tracks[-1].append(point)
        else:
            tracks.append([point])

    rules = []
    for track in tracks:
        if len({band for _, band, _ in track}) < BANDS - 1:
            continue
        slope, offset = np.polyfit([y for _, _, y in track],
                                   [x for x, _, _ in track], 1)
        rules.append((float(offset), float(slope)))
    return rules


def grid(path):
    """Where the table sits: its two bounding rules and its column boundaries."""
    image = ink(path)
    covered = inked(image)

    top, bottom = _bounding_rules(covered)
    rules = _column_rules(covered, top, bottom)
    if len(rules) != COLUMNS_PER_LEAF - 1:
        raise Unreadable(f"{len(rules)} rules between columns, "
                         f"not the {COLUMNS_PER_LEAF - 1} a leaf carries")

    # The table's outer edges carry no rule that survives the scan. Carry them out
    # at the pitch the printed rules actually measure, which is not quite even.
    middle = (top + bottom) / 2
    pitch = float(np.median(np.diff([_at(rule, middle) for rule in rules])))
    edges = ([(rules[0][0] - pitch, rules[0][1])] + rules
             + [(rules[-1][0] + pitch, rules[-1][1])])

    return {"ink": image, "top": top, "bottom": bottom,
            "edges": edges, "column_pitch": pitch}


def _comb(profile, top, bottom, count):
    """The evenly spaced comb of `count` teeth that catches the most ink."""
    caught = np.convolve(profile, np.ones(TOOTH), mode="same")
    firsts = np.arange(top + 24, top + 56, 0.25)
    teeth = np.arange(count)

    best = None
    for pitch in np.arange(29.0, 32.01, 0.02):
        usable = firsts[firsts + pitch * (count - 1) <= bottom - 24]
        if not usable.size:
            continue
        caught_by = caught[(usable[:, None] + pitch * teeth).astype(int)]
        scores = list(zip(caught_by.min(axis=1), caught_by.sum(axis=1)))
        pick = max(range(len(scores)), key=scores.__getitem__)
        if best is None or scores[pick] > best[0]:
            best = (scores[pick], pitch, float(usable[pick]))

    _, pitch, first = best
    return [first + pitch * i for i in range(count)], pitch


def rows(page, count=ROWS):
    """Centres of the rows of entries, fitted afresh under every column.

    The rows carry no printed rule at all, and on some pages one row's ink splits
    in two or two rows merge, so a comb of evenly spaced teeth is fitted to the ink
    as a whole rather than individual lines being hunted for. It has to be anchored
    between the two bounding rules: a comb one row out of step rides the second row
    down and catches the edge of the rule beneath the last, which scores higher on
    ink alone -- by both total and weakest tooth -- than the truth does.

    One comb per column, because a leaf that bows does not hold its rows at the
    same height across the page. On Tafel L's reverse the first row sits eleven
    pixels lower under the middle columns than under the last, a third of the row
    pitch, and one comb averaged over the width fits neither end.
    """
    image = page["ink"]
    middle = (page["top"] + page["bottom"]) / 2

    combs, pitches = [], []
    for column in range(COLUMNS_PER_LEAF):
        left = int(round(_at(page["edges"][column], middle)))
        right = int(round(_at(page["edges"][column + 1], middle)))
        centres, pitch = _comb(image[:, max(left, 0):right].mean(axis=1),
                               page["top"], page["bottom"], count)
        combs.append(centres)
        pitches.append(pitch)
    return combs, float(np.median(pitches))


def cell(page, column, row, comb, margin=3, half_height=15):
    """The box of one entry, taken where that column's grid runs. Native pixels."""
    combs, _ = comb
    y = combs[column][row]
    x0 = _at(page["edges"][column], y)
    x1 = _at(page["edges"][column + 1], y)
    return (int(round(x0)) - margin, int(round(y)) - half_height,
            int(round(x1)) + margin, int(round(y)) + half_height)


if __name__ == "__main__":
    import sys
    for path in sys.argv[1:]:
        page = grid(path)
        combs, pitch = rows(page)
        height = page["bottom"] - page["top"]
        lean = max(abs(slope) for _, slope in page["edges"]) * height
        bow = max(c[0] for c in combs) - min(c[0] for c in combs)
        print(f"{path}: {len(page['edges']) - 1} columns, rules at "
              f"{page['top']:.1f} and {page['bottom']:.1f}, row pitch {pitch:.2f}, "
              f"column pitch {page['column_pitch']:.2f}, rules leaning up to "
              f"{lean:.1f}px, first row {bow:.1f}px out of level across the page")
