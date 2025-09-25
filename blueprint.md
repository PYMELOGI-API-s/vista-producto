# Blueprint

## Overview

This project is a modern Admin Panel application built with Blazor, inspired by the "Stephub" design. It features a clean, professional layout with a side navigation menu and a main content area for managing application data.

## Style and Design

*   **Theme:** A professional two-tone design with a dark sidebar and a light content area.
*   **Layout:** A two-column layout featuring a fixed sidebar for navigation and a flexible main content area.
*   **Typography:** Clean, sans-serif font for readability. Hierarchy is established through font size and weight.
*   **Iconography:** Font Awesome icons are used throughout the application to provide clear visual cues for navigation and actions.
*   **Color Palette:**
    *   **Sidebar Background:** Dark grey (`#1a1d21`)
    *   **Sidebar Text:** Light grey (`#adb5bd`)
    *   **Sidebar Active/Hover:** White text with a subtle indicator.
    *   **Content Background:** Off-white (`#f8f9fa`)
    *   **Text/Headings:** Dark grey (`#343a40`)
*   **Components:**
    *   **Sidebar:** Contains navigation links grouped into categories ("Navigation", "Access Controls") and a user profile section.
    *   **Product Table:** A clean, organized table for displaying product data with columns for Image, Name, Price, and Old Price.

## Current Request: Implement Stephub Admin Panel

The user has requested to transform the application into an admin panel that matches the provided image.

### Plan:

1.  **Overhaul Layout (`MainLayout.razor`):** Implement a two-column structure with a fixed sidebar and a main content area.
2.  **Rebuild Navigation (`NavMenu.razor`):** Create the dark-themed sidebar with all the navigation links, icons, and user profile section as seen in the image.
3.  **Create Products Page (`Home.razor`):** Set the main page to be a "Products" view, displaying a table of products with mock data.
4.  **Apply Global Styles:** Update CSS to match the new color palette, typography, and layout.
5.  **Data Models:** Create a `Product` class to model the data for the products table.
 