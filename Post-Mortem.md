# Post-Mortem
---

## 📝 Phase 1 

**Topic:** Initial Setup & Controls

### The Git & Unity Struggle

Starting out, I hit a frustrating wall just trying to commit the empty Unity project. Even though I thought I had my `.gitignore` set up, I kept getting hit with "Filename too long" errors because of Windows path limits, and "Permission denied" errors because Unity was locking files in the background.

At first, I thought the only solution was to close the Unity Editor every single time I wanted to commit. I quickly realized this wasn't sustainable—reopening the project takes way too long, especially as the project grows.

It turned out my hypothesis about the `.gitignore` was half-right. The file was there, but it was in the parent folder instead of inside the actual Unity project folder (`Scape`). Git wasn't filtering the deep `Library` and `Temp` folders correctly. The fix was a mix of things: allowing long file paths in the Git config, but most importantly, moving the `.gitignore` into the correct subfolder. Once I did that, I could commit without closing Unity.

### The QWERTY vs. AZERTY Mix-up

When I moved on to the controls, I ran into a weird issue where the game ignored my code. I programmed it for **ZQSD** (AZERTY), but Unity kept behaving like I was using a QWERTY keyboard (I had to press W to move forward).

I realized that Unity’s Input System defaults to the physical location of keys on a US keyboard, regardless of what my script said. I stopped trying to manually select keys from the dropdown list and instead used the **"Listen"** feature in the Input Action window. By simply pressing my physical 'Z' key, Unity mapped the correct Key Code instantly. That sorted out the movement logic for good.

## 📝 Phase 2


