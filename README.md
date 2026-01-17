# 🏛️ Scape - 3D Puzzle & Parkour

A third-person 3D adventure game combining demanding parkour mechanics and logical puzzles set deep within a dark and mysterious Aztec pyramid.

**Goal:** Escape the dungeon as quickly as possible (Speedrun focus).

---

## 🛠️ Tech Stack

* **Engine:** Unity 6 (6000.2.14f1)
* **Pipeline:** Universal Render Pipeline (URP)
* **Language:** C#
* **Key Systems:**
    * New Input System
    * Cinemachine (3rd Person Camera)
    * Modular Architecture (Interfaces, Managers)

---

## 🎮 Controls (Keyboard/Mouse)

The game natively supports both QWERTY and AZERTY layouts.

| Action | Key (Keyboard) |
| :--- | :--- |
| **Move** | `W` `A` `S` `D` (or ZQSD) |
| **Camera** | `Mouse` |
| **Jump** | `Space` |
| **Run** | `Left Shift` |
| **Interact** | `E` |
| **Inventory** | `I` |
| **Menu / Pause** | `Esc` |

---

## 🗺️ Roadmap & Progress

Development follows an atomic, step-by-step approach.

### 🟢 Phase 1: Foundations (3C)
- [x] Project Setup & Git Configuration
- [x] Player Controller (Move, Jump, Run)
- [x] 3rd Person Camera (Cinemachine)
- [x] Unified Input System

### 🟡 Phase 2: Interactions & Core Gameplay
- [ ] **Interaction System (Raycast & Interface)** *(In Progress)*
- [ ] Inventory System (UI & Data)
- [ ] Physics: Pushing objects
- [ ] Items: Pistol, Pickaxe, Flashlight

### 🔴 Phase 3: Level Design (Stages)
- [ ] **Stage 0:** Tutorial & Introduction
- [ ] **Stage 1:** Parkour (Falling blocks)
- [ ] **Stage 2:** Puzzle (Logic gates)
- [ ] **Stage 3:** Shoot & Platforming (Moving targets)
- [ ] **Stage 4:** Item Puzzle (Stele)
- [ ] **Stage 5:** Destructible Maze (Pickaxe)
- [ ] **Stage 6:** Blackout (Flashlight navigation)
- [ ] **Stage 7:** Final & Amulet

### 🟣 Phase 4: Polish & UI
- [ ] Main Menu & Pause
- [ ] Timer & Leaderboard
- [ ] Save System (JSON/PlayerPrefs)
- [ ] Sound Design & Visual Atmosphere

---

## 🏗️ Code Architecture

The project prioritizes decoupled and maintainable code:
* **`IInteractable`**: Generic interface for all interactive objects (Doors, Items, Levers).
* **`GameManager`**: Handles global game state (Timer, Stage transitions).
* **`InputManager`**: Singleton handling user inputs.

---

*Project developed by Kessel DIAROUMEYE.*