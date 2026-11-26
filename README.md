# Pussy Island — AutoMerge & AutoCollect Mod

MelonLoader mod for **Pussy Island** that automates merging items on the board and collecting max-level resources.

## Features
- **Auto-Merge (`F5`)**: Automatically merges matching available items on the board.
- **Auto-Collect (`F6`)**: Automatically collects max-level resources (wood, screws, bricks, concrete, etc.) once they reach maximum level.
- **Animation & Inventory Safe**: Pauses actions while dragging items, spawning items from inventory/generators, or clicking on the board to prevent visual ghost items.

## Requirements
- **MelonLoader** (v0.6.0 or newer, .NET 6.0 / IL2CPP)

## Installation
1. Download the latest `AutoMerge.dll` from the [Releases](../../releases) page.
2. Place `AutoMerge.dll` into your game's `Mods` folder (e.g. `...\Steam\steamapps\common\Pussy Island\Mods\`).
3. Launch the game. Press **`F5`** to toggle Auto-Merge and **`F6`** to toggle Auto-Collect.

## Building from Source
1. Install **.NET 6.0 SDK** (or newer).
2. Run in the project directory (pass your game path if it differs from default Steam folder):
   ```powershell
   dotnet build -c Release -p:GameDir="D:\Programs\steam\steamapps\common\Pussy Island"
   ```
3. The compiled DLL will be located at `bin\Release\net6.0\AutoMerge.dll`.

## License
MIT License
