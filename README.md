# COSC 519 (J) – Lab 3  
**Project Title:** AR Fruit Slice Playground  

---

## 🎮 Project Description  
This project is an **Augmented Reality (AR)** experience developed for **COSC 519 (J) Lab 3** using **Unity 6000.2.7f2 (URP)** and **AR Foundation (ARCore)** for Android devices.  

The goal was to create an **interactive AR environment** featuring at least three engaging interactions. Our team built a fruit-slicing mini-game where apples spawn and fall in AR space, and players can **swipe to throw shurikens** to slice them. Each action is paired with responsive **sound effects**, and an additional interaction allows users to **tap on a plane to spawn a tree** in the environment.  

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
   - Allows users to tap on a detected plane to place a small 3D tree prefab.  
   - Demonstrates direct plane raycasting and object instantiation in AR.
   - Ensures natural placement of trees by offsetting it's height 

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
| **Zhehao Sun** | 🍎 Apple Spawner + ⚔️ Swipe Interaction | Implemented the `AppleLauncher.cs`, `ThrowDarts.cs`, `DartProjectile.cs`, and `AppleTarget.cs` scripts. These handle apple spawning, gesture detection for throwing shurikens, collision detection, and apple-splitting physics. |
| **Aarav Gosalia** | 🔊 Sound System + Project Setup | Implemented the `SoundManager.cs` script, integrated audio triggers into the game logic (apple spawn, throw, and hit events). Also responsible for organizing the repository structure, managing the final project build, and submitting the demo and GitHub repository. |
| **Sadia Ahmmed** | 🌳 Tap-to-Place Tree Interaction | Created the `ARPlaceTree.cs` script, which allows users to tap detected AR planes to place a tree prefab in the environment. |

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
│   └── ARPlaceTree.cs      # Handles tree placement on plane tap (Placeholder)
├── Prefabs/              # Contains apple, shuriken, and sliced apple prefabs
└── Scenes/
    └── Lab3Scene.unity   # Main AR scene for the project
```

---

## 🎥 Demo Video  
<a href="https://www.youtube.com/watch?v=dummy-arfruit-demo" target="_blank">
  <img src="https://img.youtube.com/vi/dummy-arfruit-demo/hqdefault.jpg" alt="Demo video thumbnail" />
</a>  

**YouTube Video – Demo showcasing all AR interactions.**

---

## 👤 Authors  
**Zhehao Sun**, **Aarav Gosalia**, and **Sadia Ahmmed**  
University of British Columbia – Okanagan  
