# Nations Wars

A Red Alert-style RTS in Unity. Skirmish mode comes first: you against AI opponents, each side a unique nation.
The design plan with concept art is in [`docs/game-plan.html`](docs/game-plan.html).

## Run it

1. Open this folder in **Unity 6 (6000.0 LTS)** with Unity Hub. Unity will offer to switch the editor version if yours differs; accept.
2. Wait for the package import. If it asks to enable the new input backend and restart, choose **Yes**. Either input setting works.
3. Menu **Nations Wars > Setup Skirmish Prototype**. This creates the nation, unit and weapon assets and builds `Assets/_Game/Scenes/Battle.unity`.
4. Open the Battle scene and press Play.

| Input | Action |
|---|---|
| Left click / drag | Select unit / box select (Shift adds) |
| Double click | Select all of that unit type on screen |
| Right click | Move selected units (A* around the grey wall) |
| Ctrl+1-9, 1-9 | Save / recall control group |
| WASD or arrows, mouse wheel | Pan, zoom |

## Layout

```
Assets/_Game/
  Scripts/
    Simulation/   Engine-free logic (damage table, A*, economy). Runs on a fixed tick later.
    Game/         Unity layer: data assets, input, camera, units, selection, skirmish setup, HUD
    Editor/       Nations Wars menu (sample content + scene builder)
  Tests/EditMode/ NUnit tests for the simulation (Window > General > Test Runner)
```

## What works now

- Data-driven nations, units, weapons and buildings as ScriptableObjects (Atlantic Alliance and Red Star Union).
- Damage-vs-armor table with veterancy, deterministic A* pathfinding, credits and power with oil income.
- RTS camera, unit selection, control groups, move orders, placeholder models in team colors.
- 1v1 skirmish start: each player's starting army is placed at opposite corners.

## Next (see the roadmap in the plan)

Base building and the build sidebar, harvesters and refineries, combat (targeting, projectiles, health), fog of war, skirmish lobby, AI.
