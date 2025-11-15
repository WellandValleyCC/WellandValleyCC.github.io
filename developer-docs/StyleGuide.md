# Font Size Style Guide

This document defines the explicit font sizes used across the site.  
All major elements have a defined size for both **desktop** and **mobile** breakpoints to avoid reliance on browser defaults.

---

## 📐 Font Size Table

| Element / Selector        | Desktop Font Size | Mobile Font Size |
|---------------------------|------------------|------------------|
| `html, body`              | 1rem (baseline)  | 1rem (reset baseline) |
| `table.results`           | 1.4rem           | 0.8rem |
| `table.results th, td`    | inherits (1.4rem) | inherits (0.8rem) |
| `.legend` (container)     | 1rem             | 0.9rem |
| `.legend span` (pill)     | 1rem             | 0.8rem |
| `header` (container)      | 1.25rem          | 0.9rem |
| `.event-number`           | 1.5rem           | 0.9rem |
| `header .event-date`      | 1.25rem          | 0.9rem |
| `header .event-distance`  | 1.25rem          | 0.9rem |
| `.event-nav` (container)  | 1rem             | 0.9rem |
| `.event-nav a` (links)    | 1rem             | 0.9rem |
| `footer`                  | 0.75rem          | 0.8rem |

---

## 🔑 Notes

- **Baseline**: `html, body` set to 1rem ensures consistency across all elements.
- **Overrides**: Each major container (`header`, `.legend`, `.event-nav`, `footer`) has explicit font sizes for both desktop and mobile.
- **Table cells**: Inherit from `table.results`, so they scale automatically with the table’s font size.
- **Mobile reset**: At ≤600px, everything is explicitly resized to avoid browser defaults.

---

## 🧩 Contributor Guidance

- Always define font sizes explicitly when adding new components.
- Use `rem` units for scalability and accessibility.
- Keep desktop and mobile sizes documented here so future maintainers can see intent at a glance.
