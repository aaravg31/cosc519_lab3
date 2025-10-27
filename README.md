# COSC 519 (J) – Lab 3  
**Project Title:** AR Fruit Slice Playground  

---

## 🎮 Project Description  
This project is an **Augmented Reality (AR)** experience developed for **COSC 519 (J) Lab 3** using **Unity 6000.2.7f2 (URP)** and **AR Foundation (ARCore)** for Android devices.  

The goal was to create an **interactive AR environment** featuring at least three engaging interactions. Our team built a **fruit-slicing mini-game** where apples spawn and fall in AR space, and players can **swipe to throw shurikens** to slice them. Each action is paired with responsive **sound effects**, and users can also **tap on a plane to place a 3D tree**.  

A simple **UI Mode Switcher** allows players to toggle between the *Apple Slice* and *Tree Place* modes, ensuring only one mode is active at a time.  


---

## ✨ Interactive Elements  

1. **🍎 Apple Spawning (AR Plane Detection)**  
   - Detects horizontal AR planes and periodically spawns apples that fall naturally through the player’s AR view.  
   - Uses `ARPlaneManager` and physics for realistic gravity and motion.  
   - Apples are randomly positioned within the detected plane’s bounds.  

2. **⚔️ Swipe-to-Throw Shuriken (Gesture Detection)**  
   - The user swipes upward from the bottom of the screen to throw a blade (shuriken).  
   - The shuriken travels forward from the camera, spins, and can hit falling apples.  
   - Collision detection causes apples to split into two halves with satisfying physics.  

3. **💥 Apple Splitting & Physics**  
   - When hit by a shuriken, each apple splits into left and right halves that peel apart and fall.  
   - This is handled using Unity’s `Rigidbody` physics system and custom collision logic.  

4. **🔊 Sound Effects System** 
   - Plays sound cues for:  
     - Apple spawn  
     - Shuriken throw  
     - Apple hit/slice  
   - Managed by a global **`SoundManager`** script using an `AudioSource` component.   

5. **🌳 Tap-to-Place Tree (AR Interaction)**  
   - Allows users to tap a detected AR plane to instantiate a tree prefab at that location.  
   - Utilizes AR raycasting for accurate placement and offsets the tree slightly above ground level.  
   - Managed by `TapToPlaceTree.cs`.

6. **🟩 Mode Switcher UI (Mode Toggle System)**  
   - Two floating buttons (`Apple Slice` and `Tree Place`) appear at the top-left of the screen.  
   - Selecting a mode activates its corresponding interaction and visually highlights the selected button (green).  
   - Only one mode can be active at a time.  
   - Managed by `ModeSwitcher.cs`.

---

## 🔹 Development Details  
- **Unity Version:** 6000.2.7f2  
- **Render Pipeline:** Universal Render Pipeline (URP)  
- **Platform Target:** Android (ARCore Compatible Devices)  
- **Packages Used:**  
  - AR Foundation  
  - ARCore XR Plugin  
  - Input System (for swipe gestures)
  - Tree_Packs

---

## 👩‍💻 Team Contributions  

| Team Member | Contribution | Description |
|--------------|---------------|--------------|
| **Zhehao Sun** | 🍎 Apple Spawner + ⚔️ Swipe Interaction | Implemented the `AppleLauncher.cs`, `ThrowDarts.cs`, `DartProjectile.cs`, and `AppleTarget.cs` scripts. Handles apple spawning, gesture detection, projectile launching, and apple-splitting physics. |
| **Aarav Gosalia** | 🔊 Sound System + UI Mode Switcher + Project Setup | Implemented `SoundManager.cs` and integrated audio triggers. Designed and coded the `ModeSwitcher.cs` UI system for toggling between Apple and Tree modes. Responsible for organizing the repository, Android build setup, and final submission/demo. |
| **Sadia Ahmmed** | 🌳 Tap-to-Place Tree Interaction | Created `TapToPlaceTree.cs` to allow plane tapping and tree instantiation in AR space. Focused on natural placement and AR raycasting logic. |


---

## 📂 Repository Structure  

```
Assets/
├── Sounds/               # Contains apple spawn, shurikenthrow, and hit sound clips
├── Scripts/              # All custom scripts used in the project
│   ├── AppleLauncher.cs       # Handles apple spawning on detected planes
│   ├── ThrowDarts.cs          # Detects swipe gestures and launches shurikens
│   ├── DartProjectile.cs      # Controls shuriken behavior and aim assist
│   ├── AppleTarget.cs         # Detects hits and splits apples into halves
│   ├── SoundManager.cs        # Centralized sound playback system
│   ├── ModeSwitcher.cs        # UI toggle system for switching between modes
│   └── TapToPlaceTree.cs      # Handles tree placement via screen tap
├── Prefabs/              # Contains apple, shuriken, sliced apple, and tree prefabs
└── Scenes/
    └── Lab3Scene.unity   # Main AR scene for the project
```

---

## 🎥 Demo Video  
<a href="https://youtube.com/shorts/Mb5bMKOWy5Y?feature=share" target="_blank">
  <img src="https://img.youtube.com/vi/Mb5bMKOWy5Y/hqdefault.jpg" alt="Demo video thumbnail" />
</a>  

**YouTube Video – Demonstration of all AR interactions (Apple Slice, Shuriken Throw, Tree Placement, and Mode Switcher UI).**

Note: Not the best demo but didn't have time to make a nice one...

---

## 👤 Authors  
**Zhehao Sun**, **Aarav Gosalia**, and **Sadia Ahmmed**  
University of British Columbia – Okanagan  
