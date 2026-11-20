# DishMeAPP

A cross-platform food app built with **.NET MAUI**, developed as a master's project at the University of Newcastle.

Order dishes, save your own recipes, and switch between Chef and Diner — all from one app, running on Android and iOS from a single C# codebase.

> **Status:** Work in progress. This is the original master's project, published as a portfolio piece. It is being polished step by step — see [Roadmap](#roadmap) for what is next.

## Features

### Order

- Browse dishes by category: Chinese, Western, Desserts, Soups, Vegetarian, Others
- 14 sample dishes with photos, e.g. Kung Pao Chicken, Mapo Tofu, Grilled Steak, Tiramisu
- Filter dishes by category
- Add dishes to the cart, change quantities, remove items
- View a cart summary and place an order (currently simulated — see below)

### Recipes

- Create a recipe with a name, cooking steps, ingredients and a category
- Take a photo with the camera or pick one from the gallery (MAUI `MediaPicker`)
- Photos are saved locally on the device
- Recipes are saved locally and appear as dishes on the Order tab

### Mine

- Switch roles between **Chef** and **Diner** — the role is saved on the device
- View app version, platform and device info in About

## Tech stack

| Layer        | Technology                                                      |
| ------------ | --------------------------------------------------------------- |
| Language     | C#                                                              |
| UI framework | .NET MAUI 8.0, XAML                                             |
| Platforms    | Android (API 21+) and iOS (11.0+)                               |
| IDE          | Visual Studio 2022                                              |
| Architecture | MVVM — Models / ViewModels / Views / Services                   |
| Libraries    | `Microsoft.Maui.Controls` 8.0.80, `CommunityToolkit.Maui` 7.0.1 |

## Data — local only, no backend

There is no backend server, database server or external API in this project. Everything is stored on the device:

- **Recipes:** saved as JSON files in the app's local `Recipes/` folder (`recipe_{id}.json` + `recipes_index.json`), using `System.Text.Json` via `Services/RecipeStorageService.cs`.
- **Cart:** held in memory only, via the `CartService` singleton. It is not persisted across restarts.
- **Settings / role:** stored with MAUI `Preferences` via `Services/UserSettingsService.cs`.

## Project structure

DishMeAPP.sln
DishMeAPP/
App.xaml / App.xaml.cs # App entry, loads AppShell
AppShell.xaml / .cs # Shell navigation: Order / Recipes / Mine
MauiProgram.cs # MAUI + CommunityToolkit setup, OpenSans font
DishMeAPP.csproj # .NET 8, Android + iOS targets
Models/ # Dish, Category, Recipe, UserRole
ViewModels/ # OrderViewModel, RecipeViewModel, MineViewModel
Views/ # OrderPage, RecipesPage, MinePage
Services/ # RecipeStorageService, CartService, UserSettingsService
Resources/ # Styles, icons, images, fonts
Platforms/ # Android / iOS / Windows / MacCatalyst / Tizen

## How to run

1. Install **Visual Studio 2022** with the **.NET Multi-platform App UI development** workload.
2. Open `DishMeAPP.sln`.
3. Select an Android emulator or an iOS simulator / device and run in Debug.

Notes:

- The camera permission is requested on Android (`AndroidManifest.xml`) and iOS (`Info.plist`). The iOS simulator does not support the camera — use a physical device to test taking photos.
- Building for iOS requires a Mac. On Windows you can develop and run the Android version; pair Visual Studio with a Mac to build and debug iOS.

## Roadmap

What is being worked on next, in order:

- [ ] Add prices to dishes and show a cart total
- [ ] Make placing an order save a real order, and add an order history page
- [ ] Add a recipe list page with view, edit and delete
- [ ] Persist the cart across restarts
- [ ] Finish category management in Mine (currently a placeholder)
- [ ] Fix missing assets (`recipe_placeholder.png`, `camera.svg`) and the font folder path
- [ ] Clean up duplicated code (shared alert helper, shared `INotifyPropertyChanged` base class)
- [ ] Add unit tests

## Author

Gabbi — Master of Information Technology, University of Newcastle (Jan 2024 – Dec 2025).

Built solo as a master's project, now being polished as a portfolio piece for AI Engineer / Full-Stack / Software Engineer roles.
