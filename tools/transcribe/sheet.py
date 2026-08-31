"""Rendering cells as sheets to be read from.

Each cell is magnified from its own native pixels and normalised on its own
contrast, so a faint corner of a page is as readable as a dark one. Cells are laid
out a few columns at a time because a whole column at this magnification does not
fit in one image, and because reading down a column keeps the eye on one leaf.
"""
import sys

import numpy as np
from PIL import Image

import cut

SCALE = 4
LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"


def cells(path):
    """Every cell of one leaf, as a normalised greyscale image."""
    page = cut.grid(path)
    comb = cut.rows(page)
    green = np.asarray(Image.open(path).convert("RGB")).astype(float)[:, :, 1]

    out = {}
    for column in range(cut.COLUMNS_PER_LEAF):
        for row in range(cut.ROWS):
            x0, y0, x1, y1 = cut.cell(page, column, row, comb)
            patch = green[y0:y1, x0:x1]
            low, high = np.percentile(patch, 2), np.percentile(patch, 98)
            scaled = np.clip((patch - low) / max(high - low, 1) * 255, 0, 255)
            out[(column, row)] = Image.fromarray(scaled.astype(np.uint8), "L")
    return out


def render(cells, columns, rows, destination, scale=SCALE, gutter=6):
    widths = [max(cells[(c, r)].width for r in rows) for c in columns]
    heights = [max(cells[(c, r)].height for c in columns) for r in rows]
    sheet = Image.new(
        "L",
        (sum(w * scale for w in widths) + gutter * (len(columns) + 1),
         sum(h * scale for h in heights) + gutter * (len(rows) + 1)),
        128)

    y = gutter
    for ri, row in enumerate(rows):
        x = gutter
        for ci, column in enumerate(columns):
            tile = cells[(column, row)]
            sheet.paste(
                tile.resize((tile.width * scale, tile.height * scale), Image.LANCZOS),
                (x, y))
            x += widths[ci] * scale + gutter
        y += heights[ri] * scale + gutter

    sheet.save(destination)
    return sheet.size


def sheets(path, prefix, first_column=0, per_sheet=3):
    """Every cell of a leaf, as sheets of `per_sheet` columns by half a column."""
    grid = cells(path)
    written = []
    for start in range(0, cut.COLUMNS_PER_LEAF, per_sheet):
        columns = list(range(start, min(start + per_sheet, cut.COLUMNS_PER_LEAF)))
        named = "".join(LETTERS[first_column + c] for c in columns)
        for half, rows in enumerate((range(0, 13), range(13, 26))):
            out = f"{prefix}_{named}_{'AM' if half == 0 else 'NZ'}.png"
            render(grid, columns, list(rows), out)
            written.append(out)
    return written


if __name__ == "__main__":
    if len(sys.argv) not in (3, 4):
        raise SystemExit("usage: sheet.py LEAF.png PREFIX [FIRST_COLUMN]")
    first = int(sys.argv[3]) if len(sys.argv) == 4 else 0
    for name in sheets(sys.argv[1], sys.argv[2], first):
        print(name)
