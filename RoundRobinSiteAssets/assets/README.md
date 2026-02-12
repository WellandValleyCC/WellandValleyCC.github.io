# Assets

This directory contains **supporting assets for the Round Robin TT Series site**, with a current focus on **CSS stylesheets**. These files define the presentation and layout of the site but are **not** brand identity elements.

Identity assets (club logos, Round Robin branding) belong in `/logos`, not here.

---

## Current Contents

At present, this folder contains:

- **CSS files** used to style the Round Robin pages  
  (e.g., layout rules, typography, colours, responsive behaviour)

These stylesheets are part of the site’s implementation and may evolve as the site grows.

---

## Purpose

Use this folder for any assets that:

- define the **look and feel** of the site  
- support **page layout** or **component styling**  
- are part of the **site’s implementation**, not its identity  

Examples (current and future):

- CSS files  
- JavaScript (if ever added)  
- Page‑specific or feature‑specific images  
- Decorative or illustrative graphics  

---

## Guidelines

- Keep CSS files **modular and intention‑revealing**.  
  For example: `layout.css`, `round-robin.css`, `tables.css`.

- Only store **non‑identity** assets here.  
  Logos and branding must remain in `/logos`.

- If images or other assets are added later, group them logically  
  (e.g., `/assets/css`, `/assets/images`) — but only when needed.

---

## Local Development Note

The `/assets` folder is included in the generated output (`RoundRobinSiteOutput`) so that local development via:

```
dotnet serve -d RoundRobinSiteOutput -o
```

renders pages exactly as they will appear when published.

No duplication across repos is required for this folder.