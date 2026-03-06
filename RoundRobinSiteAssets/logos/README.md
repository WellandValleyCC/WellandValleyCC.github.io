# Logos

This directory contains the **brand identity assets** used by the Round Robin TT Series and the participating clubs. These files are stable, global identity elements and are reused across multiple pages of the site.

They are **not** decorative or content images. Only identity assets belong here.

---

## Current Folder Structure

```
/logos
    hcrc.png
    kcc.png
    lfcc.png
    ratae.png
    rfw.png
    wvcc.png
    /round-robin
        rr-header-2026.png
```

### What these files represent

- **Club logos (top level)**  
  Logos for each club participating in the Round Robin TT Series:
  - HCRC  
  - KCC  
  - LFCC  
  - Ratae  
  - RFW  
  - WVCC  

- **Round Robin branding (`/round-robin`)**  
  Assets representing the Round Robin TT Series itself.  
  Currently:
  - `rr-header-2026.png` — the 2026 Round Robin header/branding graphic

---

## Purpose

This folder provides a single, predictable location for all identity assets used by the Round Robin site.  
It keeps branding separate from:

- CSS  
- content images  
- page‑specific assets  
- decorative graphics  

Those belong in `/assets`.

---

## Workflow Notes

The logos exist in **two places** during development:

1. **Authoritative copy (published site)**  
   `RoundRobinTT.github.io/docs/logos`  
   This is the version served by GitHub Pages.

2. **Local development copy (generated)**  
   `WellandValleyCC.github.io/RoundRobinSiteOutput/logos`  
   This folder is created during local builds and is **ignored by git**.

When adding new logos:

1. Add them here in the RoundRobin repo (this is the source of truth).  
2. Copy the updated folder into `RoundRobinSiteOutput/logos` so local dev renders correctly.

Because logo changes are rare and typically additive, maintaining the two copies manually is a simple and reliable workflow.

---

## Naming Conventions

- Use short, lowercase filenames for club logos (e.g., `wvcc.png`, `hcrc.png`).  
- Use versioned filenames for Round Robin branding (e.g., `rr-header-2026.png`) to avoid browser cache issues.  
- Keep names descriptive and consistent.

---

## Guidelines

- Only store **identity** assets here.  
  If the image represents a club or the Round Robin brand, it belongs in this folder.

- Do **not** store photos, decorative images, or content graphics here.  
  Those belong in `/assets`.

- Keep the structure tidy and intention‑revealing for future maintainers.