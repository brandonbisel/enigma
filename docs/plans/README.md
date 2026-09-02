# Plans

A plan document is the detail behind a line in [the roadmap](../../ROADMAP.md). The
roadmap says what is intended in a sentence or two; a plan here says what the thing
would actually be, what it is waiting on, and how it would be pinned once it existed.

Plans are working documents and are allowed to be wrong. What they may not be is
vague about evidence: a plan that would ship cipher data has to name the source it is
waiting for, because [the one rule](../sources.md#the-standard-everything-here-is-held-to)
applies to a plan exactly as it applies to a commit.

**When the work lands, the plan goes.** It is replaced by a feature document in
[docs](../README.md) and a short description in the README, and the roadmap line
disappears rather than moving to a list of things already done. A completed item is
documented, not commemorated.

| Plan | Status |
|---|---|
| [N-gram scoring, and the steckered break](ngram-scoring.md) | Blocked on a source |
| [Bombe simulation](bombe.md) | Not started; not blocked |
