# CI Workflows Guide

This document explains how the CI pipeline builds, publishes, and maintains the Welland Valley CC time trial results site.

---

## Workflow Overview

- Checkout `master` with full history (`fetch-depth: 0`)
- Install runtime tools and dependencies (dotnet, Python packages, etc.)
- Run processor to produce `site-out/`
- Publish `site-out/` to `gh-pages` branch in one atomic commit
- Use workflow `concurrency` to avoid overlapping generator runs

---

## Preventing Loops

- Trigger workflows only on input/source paths:

```yaml
on:
  push:
    branches: [ master ]
    paths:
      - 'data/**'
      - 'results/**'
      - 'scripts/**'
```

- Ensure processor workflows do not trigger on `gh-pages/**`.

- Mark generated commits with a tag in the message, for example:

``` text
chore: publish site from master <SHA> [generated]
```

## CI commit and push snippet

``` Bash
git config user.name "github-actions[bot]"
git config user.email "41898282+github-actions[bot]@users.noreply.github.com"

# prepare or update gh-pages worktree
git worktree add /tmp/gh-pages gh-pages || (git checkout --orphan gh-pages && git rm -rf .)

# sync generated site into worktree and commit
rsync -a --delete site-out/ /tmp/gh-pages/
cd /tmp/gh-pages

git add -A
if git diff --quiet --exit-code; then
  echo "No changes to publish"
  exit 0
fi

git commit -m "chore: publish site from master ${GITHUB_SHA} [generated]"
git push origin HEAD:gh-pages
```

Note: the snippet above uses only `GITHUB_TOKEN` (no PAT required) when run inside GitHub Actions with `permissions: contents: write`.

## Example Workflow (`generate-site.yml`)

``` yaml
name: Generate site to gh-pages

on:
  push:
    branches: [ master ]
    paths:
      - 'data/**'
      - 'results/**'
      - 'scripts/**'
      - '.github/workflows/generate-site.yml'

permissions:
  contents: write

jobs:
  build-and-publish:
    runs-on: ubuntu-latest
    concurrency:
      group: generate-site-${{ github.ref }}
      cancel-in-progress: true
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Install runtime and deps
        run: |
          # example for dotnet and python helpers
          sudo apt-get update && sudo apt-get install -y python3 python3-pip
          dotnet --info
          python3 -m pip install --upgrade pip
          pip install pandas openpyxl

      - name: Build and run processor
        run: |
          mkdir -p site-out
          # adjust command to your processor invocation
          dotnet build processor/ClubProcessor.sln -c Release
          dotnet run --project processor/ClubProcessor -- build --input data --db club.db --output site-out

      - name: Publish to gh-pages
        env:
          GH_PAGES_BRANCH: gh-pages
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "41898282+github-actions[bot]@users.noreply.github.com"
          git worktree add /tmp/gh-pages gh-pages || (git checkout --orphan gh-pages && git rm -rf .)
          rsync -a --delete site-out/ /tmp/gh-pages/
          cd /tmp/gh-pages
          git add -A
          if git diff --quiet --exit-code; then
            echo "No changes to publish"
            exit 0
          fi
          git commit -m "chore: publish site from master ${GITHUB_SHA} [generated]"
          git push origin HEAD:gh-pages
```

## Maintenance Notes

- To snapshot before removing `docs/` from `master`:

```Bash
git tag -a pre-ghpages-site -m "Snapshot before moving site to gh-pages"
git push origin pre-ghpages-site
```

- For emergency edits, you can push directly to `gh-pages`, but prefer fixing source on master so the generator remains authoritative.

- Keep `README.md` and contributor notes updated when processor arguments, template locations, or publishing steps change.