"""Locating the grid on a page of a Doppelbuchstabentauschtafel.

The booklets are scanned at 150 ppi, half the linear resolution of the Meer one,
and rendering them larger invents detail: two cells were once read confidently and
wrongly from a four-fold interpolation. So nothing here resamples. The grid is
found in *sheared* coordinates and a cell comes back as a plain crop of original
pixels, which keeps a page that is a little out of true from being rotated.

The page is read from the green channel whatever colour the stock is. The booklet
alternates white and pink, and red ink on pink barely separates in luminance --
the first attempt at this found 26 evenly spaced rows that were not the rows.
"""
import numpy as np
from PIL import Image

COLUMNS = 26          # the table is 26 wide, but a leaf carries half of it
COLUMNS_PER_LEAF = 13
ROWS = 26


def ink(path):
    """Ink strength: how far below the paper each pixel sits in green."""
    green = np.asarray(Image.open(path).convert("RGB")).astype(float)[:, :, 1]
    return np.clip(np.median(green) - green, 0, None)


def row_projection(image, shear):
    """Sum along rows, lifting each column by `shear` per pixel of x."""
    height, width = image.shape
    pad = int(abs(shear) * width) + 2
    total = np.zeros(height + 2 * pad)
    for x in range(width):
        offset = int(round(shear * x))
        total[pad - offset:pad - offset + height] += image[:, x]
    return total, pad


def column_projection(image, shear):
    """Sum down columns, sliding each row by `shear` per pixel of y."""
    height, width = image.shape
    pad = int(abs(shear) * height) + 2
    total = np.zeros(width + 2 * pad)
    for y in range(height):
        offset = int(round(shear * y))
        total[pad - offset:pad - offset + width] += image[y, :]
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


def _suppress(peaks, radius):
    """A column of letters can stack into a ridge as tall as a faint rule, but never
    a rule's width away from a real one. Keep the strongest of any close pair."""
    kept = []
    for centre, height in sorted(peaks, key=lambda p: -p[1]):
        if all(abs(centre - k) >= radius for k, _ in kept):
            kept.append((centre, height))
    return sorted(kept)


def _best_shear(score):
    return max(np.arange(-0.020, 0.0201, 0.0005), key=score)


def grid(path):
    """Where the table sits: its two bounding rules and its column boundaries."""
    image = ink(path)
    height, width = image.shape

    row_shear = _best_shear(lambda s: np.var(row_projection(image, s)[0]))
    total, pad = row_projection(image, row_shear)
    profile = total / width

    # The only full-width ink on the page is the rule above the table and the one
    # below it.
    rules = [centre for centre, _ in _peaks(profile, profile.mean() * 4.0, pad)]
    top, bottom = min(rules), max(rules)

    body = image[int(top) + 4:int(bottom) - 4, :]
    column_shear = _best_shear(lambda s: np.var(column_projection(body, s)[0]))
    total, pad = column_projection(body, column_shear)
    profile = total / body.shape[0]
    printed = [c for c, _ in _suppress(_peaks(profile, 20, pad), 90)]

    # The table's outer edges carry no rule that survives the scan. Carry them out
    # at the pitch the printed rules actually measure, which is not quite even.
    pitch = float(np.median(np.diff(printed)))
    edges = [printed[0] - pitch] + printed + [printed[-1] + pitch]

    return {
        "ink": image, "row_shear": row_shear, "column_shear": column_shear,
        "top": top, "bottom": bottom, "edges": edges, "column_pitch": pitch,
    }


def rows(page, count=ROWS):
    """Centres of the rows of entries.

    The rows carry no printed rule at all, and on some pages one row's ink splits
    in two or two rows merge, so a comb of evenly spaced teeth is fitted to the ink
    as a whole rather than individual lines being hunted for. It has to be anchored
    between the two bounding rules: a comb one row out of step rides the second row
    down and catches the edge of the rule beneath the last, which scores higher on
    ink alone -- by both total and weakest tooth -- than the truth does.
    """
    image = page["ink"]
    left, right = int(page["edges"][0]), int(page["edges"][-1])
    total, pad = row_projection(image[:, left:right], page["row_shear"])
    profile = total / (right - left)
    top, bottom = page["top"], page["bottom"]

    best = None
    for pitch in np.arange(29.0, 32.01, 0.02):
        for first in np.arange(top + 24, top + 56, 0.25):
            if first + pitch * (count - 1) > bottom - 24:
                continue
            teeth = [profile[int(first + pitch * i + pad) - 6:
                             int(first + pitch * i + pad) + 7].sum()
                     for i in range(count)]
            score = (min(teeth), sum(teeth))
            if best is None or score > best[0]:
                best = (score, pitch, first)

    _, pitch, first = best
    return [first + pitch * i for i in range(count)], pitch


def cell(page, column, row, comb, margin=3, half_height=15):
    """The box of one entry, following the page's shear. Native pixels throughout."""
    centres, _ = comb
    x0, x1 = page["edges"][column], page["edges"][column + 1]
    y0, y1 = centres[row] - half_height, centres[row] + half_height
    dy = page["row_shear"] * (x0 + x1) / 2
    dx = page["column_shear"] * (y0 + y1) / 2
    return (int(round(x0 + dx)) - margin, int(round(y0 + dy)),
            int(round(x1 + dx)) + margin, int(round(y1 + dy)))


if __name__ == "__main__":
    import sys
    for path in sys.argv[1:]:
        page = grid(path)
        centres, pitch = rows(page)
        print(f"{path}: {len(page['edges']) - 1} columns, rules at "
              f"{page['top']:.1f} and {page['bottom']:.1f}, row pitch {pitch:.2f}, "
              f"shear {page['row_shear']:+.4f}/{page['column_shear']:+.4f}")
