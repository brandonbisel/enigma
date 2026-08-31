"""Pulling one leaf of a booklet out of the scan, upright and at native size.

`pdfimages` gives back the embedded JPEG rather than a render of the page, which
is the whole point: the pipeline reads the pixels the scanner produced.
"""
import subprocess
import sys
import tempfile
from pathlib import Path

from PIL import Image

import cut


def _right_heavy(image, workdir):
    """How much more ink a cell carries on its right than on its left."""
    probe = Path(workdir) / "probe.png"
    image.save(probe)
    page = cut.grid(str(probe))
    comb = cut.rows(page)
    pixels = page["ink"]

    left = right = 0.0
    for column in range(cut.COLUMNS_PER_LEAF):
        for row in range(1, cut.ROWS - 1):
            x0, y0, x1, y1 = cut.cell(page, column, row, comb)
            middle = (x0 + x1) // 2
            left += pixels[y0:y1, x0:middle].sum()
            right += pixels[y0:y1, middle:x1].sum()
    return right - left


def upright(image, workdir):
    """Turn a leaf the right way up.

    The leaves are bound alternately, so half the scans come out inverted, and the
    header block is no guide: a reverse prints its Kennwort *below* the table
    rather than above, so a test on which side of the table the text sits picks the
    wrong rotation on exactly the pages that matter. What settles it is inside the
    table. Every row names one letter and gives two, so a cell carries more ink on
    the right than on the left, and a page upside down reverses that.
    """
    best = None
    for angle in (90, 270):
        turned = image.rotate(angle, expand=True)
        score = _right_heavy(turned, workdir)
        if best is None or score > best[0]:
            best = (score, turned)
    return best[1]


def extract(pdf, page, destination):
    """Write page `page` of `pdf` (1-based, as pdfimages counts) to `destination`."""
    with tempfile.TemporaryDirectory() as workdir:
        stem = Path(workdir) / "leaf"
        subprocess.run(
            ["pdfimages", "-f", str(page), "-l", str(page), "-png", pdf, str(stem)],
            check=True)
        scanned = sorted(Path(workdir).glob("leaf-*.png"))
        if len(scanned) != 1:
            raise SystemExit(f"page {page} of {pdf} holds {len(scanned)} images, not one")
        upright(Image.open(scanned[0]), workdir).save(destination)
    return destination


if __name__ == "__main__":
    if len(sys.argv) != 4:
        raise SystemExit("usage: pages.py BOOKLET.pdf PAGE OUT.png")
    print(extract(sys.argv[1], int(sys.argv[2]), sys.argv[3]))
