# Site Preview Guide

This guide explains how the static site is previewed safely during development before going live.

---

## Preview Mode

- The site generator outputs `preview.html` instead of `index.html`
- This prevents accidental overwrite of the legacy homepage (`docs/index.htm`)
- GitHub Pages publishes `docs/preview.html`, accessible at:
  https://wellandvalley.github.io/preview.html

---

## Local Testing

To preview the site locally:

```bash
dotnet run --project processor/ClubSiteGenerator
dotnet serve -d SiteOutput -o
```

This opens `http://localhost:5000/preview.html` in your browser.

## CI Workflow Safety
The GitHub Actions workflow:

Publishes `preview.html` to `gh-pages`

Skips `index.html` to avoid exposing the new homepage prematurely

Optionally fails if `index.html` is present during preview

## Go-Live Plan
When ready to launch:

Rename `preview.html` to `index.html`

Add seasonal links (e.g. 2026, 2027) to the homepage

Link to legacy archive at legacy/index.htm

## Contributor Notes
Do not commit `index.html` during preview phase

Use `preview.html` for layout and content validation

See `README.md` for full repo structure and publishing flow