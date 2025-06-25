# 🎬SPT SatisfyingOverlay mod

**Because nothing calms you down like a rug being pressure-washed...**  
**...while you're bleeding out in Interchange.**

This mod adds configurable **video overlays** directly into your **SPT-AKI raids**.  
Perfect for tactical vibing mid-fight — pure visual dopamine.

---

## 🧠 Features

- 🎥 **Plays videos during raids**
  - Supports **up to 10 videos** at the same time!
- ⚙️ **Fully configurable settings per video-player:**
  - Position (X/Y)
  - Scale (Width/Height)
  - Transparency
  - Audio toggle
  - Custom video from mod folder

---

## 🔧 Installation

1. Download and install **[KmyTarkovApi v1.4.0 or later](https://hub.sp-tarkov.com/files/file/1215-kmy-tarkov-api/)** – **this is required**!
2. Extract the `BepInEx` folder from the `.7z` archive into your **SPT root directory**.
3. Launch the game — **done!**

---

## ▶️ How to Use

📺 Video guide: [https://youtu.be/ZoWvqE2H6d4](https://youtu.be/ZoWvqE2H6d4)

### 🔹 How to add my own videos?

1. Drop your `.mp4` files into:  
   `BepInEx/plugins/SatisfyingOverlay/Videos`
2. Start SPT.
3. Press **F12** to open the overlay configurator.
4. Select your video from the dropdown list.

### 🔹 Does it work outside of raids?

- ❌ No. Videos play **only during raids.**

### 🔹 Why is the archive size large?

- Comes with **5 example videos**:
  - Carpet cleaning
  - Kinetic sand cutting
  - Slime
  - Soap crushing
  - Subway Surfers gameplay

### 🔹 Can videos play sound?

- ✅ Yes!
  - Enable **“AudioEnable”** in the video slot settings.
  - Volume is linked to **in-game music volume** slider.

### 🔹 My video doesn’t show up!

- Make sure the file is a `.mp4`
- Avoid strange characters in the filename.
- Re-encode the video using `ffmpeg`:
  ```bash
  ffmpeg -i broken.mp4 -c:v libx264 -c:a aac fixed.mp4
