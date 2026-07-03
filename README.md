# 🎥 Orbital Camera

*A smooth orbital camera system built from scratch in C# on top of Aiv.Fast3D*

![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?style=flat&logo=dotnet&logoColor=white)
![OpenGL](https://img.shields.io/badge/OpenGL-Aiv.Fast3D-5586A4?style=flat&logo=opengl&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green?style=flat)

---

## 🔎 overview/

A small 3D scene where you steer an Ivysaur across a grass field with **WASD**, look around with the **mouse**, and zoom with the **mouse wheel**. The actual point of the project isn't the Pokémon — it's the camera: a third-person orbital rig that stays locked onto the character while letting the player freely rotate and zoom around it.

Built as the first-year final exam for the *Video Game Programming* course at **Accademia Italiana Videogiochi (AIV)**, on top of Aiv.Fast3D — the school's lightweight C# wrapper around OpenGL/OpenTK. No engine, no editor: camera math, character-facing logic, and rendering setup all written by hand.

**What I learned:**
- Applying trigonometry directly to solve a real gameplay problem (polar-coordinate camera orbiting)
- Programming 3D movement, rotation, and camera control from scratch, without an engine underneath

**Status:** complete.

---

## 🕹️ functional/

**Tech stack:** C#, [Aiv.Fast3D](https://github.com/) / Aiv.Fast2D (AIV's teaching wrapper around OpenTK/OpenGL), AssimpNet (model import), OpenTK 3.2 — targeting .NET Framework 4.8.

**Dependencies:** Aiv.Fast2D 1.0.3, Aiv.Fast3D 1.0.1, AssimpNet 4.1.0, OpenTK 3.2, all restored via NuGet (`packages.config`).

**Setup:**
1. Clone the repo and open `OrbitalCamera/OrbitalCamera.sln` in Visual Studio.
2. Restore NuGet packages (Aiv.Fast2D, Aiv.Fast3D, AssimpNet, OpenTK).
3. Build and run — it's a self-contained Windows executable, no external assets to configure.

**Controls:**
| Input | Action |
|---|---|
| `W / A / S / D` | Move Ivysaur, relative to camera orientation |
| `Mouse movement` | Orbit the camera around the character |
| `Mouse wheel` | Zoom in/out |

**Configuration:** camera distance, min/max zoom, mouse sensitivity, movement/rotation speed, and character turn-smoothing are all exposed as tunable constants in `Player.cs`.

![Orbital camera in action](https://github.com/user-attachments/assets/471a9e50-0695-400d-a49e-4da08fcc87c7)

No playable build is currently available — source only.

---

## ⚙️ technical/

**Architecture:** three classes, no framework abstractions in between:
- `Program` — window/game loop, ground plane generation, lighting setup.
- `Player` — owns the `PerspectiveCamera`, reads input, drives orbit math and movement.
- `Ivysaur` — wraps the imported mesh (loaded via `SceneImporter`/AssimpNet), handles positioning, scaling, and smooth turn-to-face rotation.

**Orbit math:** the camera doesn't parent to the character — every frame its position is recomputed in polar coordinates around the Ivysaur:

```csharp
camera.Position3 = new Vector3(
    Mathf.Cos(accumulator) * distance * scrollMultiplied,
    offset.Y,
    Mathf.Sin(accumulator) * distance * scrollMultiplied
) + Ivysaur.GetPosition();
```

`accumulator` is an angle driven by raw mouse-delta on X, `distance * scrollMultiplied` is the radius (scroll-clamped between a min and max), and the whole thing is re-centered on the character's current position each frame. Movement direction (WASD) is derived from the camera's own forward/right vectors, so strafing and moving always feel camera-relative.

**Hardest problem — Euler rotation wrap-around:** rotations are done with Euler angles instead of quaternions (Aiv.Fast3D's `EulerRotation3` API leaves you with Euler angles by default), which caused the Ivysaur model's Y-rotation to visibly snap back whenever it crossed the ±180° boundary while turning to face the camera direction. The fix was writing explicit angle-wrapping helpers — `NormalizeAngle` (keeps camera and character heading in the -180°/180° range) and a custom `LerpAngle` that computes the shortest signed delta between current and target rotation before interpolating — so the turn-to-face logic in `Ivysaur.SetForwardPokemon` always takes the short way around instead of spinning the long way or popping.

**Known limits:** the whole rotation system is Euler-angle-based by necessity of the wrapper's API, which is workable here (single Y-axis rotation, no roll/pitch on the character) but wouldn't scale cleanly to free 3D rotation on multiple axes — that's the case where quaternions would earn their complexity. Camera offset, zoom range, and speeds are hardcoded constants rather than exposed as runtime-configurable parameters.

**Lighting/rendering:** ground plane uses a single directional light with diffuse + specular maps, tiled via scaled UVs; the Ivysaur model uses six directional lights facing each axis to approximate ambient fill without a real ambient/IBL setup, plus a diffuse map and a glossiness map fed into `DrawPhong`.
