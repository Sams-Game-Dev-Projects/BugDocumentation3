Sam Garcia

Bug Documentation 3



##### Section 1 - (1 Credit Point)

The game's code does not compile. No matter what the developers do it just won't let them compile anything. They suspect it has something to do with different scripts not communicating but they cannot find the issue.

- Problem: `ItemScripts.asmdef` was missing a reference to `Configurators.asmdef`, so weapons/items could not see the attack/trail configurators and the project failed to compile when those types were used (`Assets/Scripts/ItemScripts/ItemScripts.asmdef:4-9`).
- Action: Added `Configurators.asmdef` as a reference in `ItemScripts.asmdef`.

 

##### Section 2 - (1 Credit)

The player must be able to move between Game Mode and Menu Mode. This includes the player being able to move, shoot, and look around while in Game Mode. The inventory cannot be seen or interacted with when in Game Mode. The cursor can also not be seen when in Game Mode. When in Menu Mode however, the player cannot perform the actions stated in Game Mode, instead they are able to see their inventory and their cursor is visible for clicking on objects such as buttons.

- Problem: Gameplay input events were not wired—`EnableGamePlay` unsubscribed instead of subscribed, so Fire/Reload/Interact never fired; `OnOpenInventory` was invoked every frame in `Update`; and there was no input binding to open the menu (`Assets/Scripts/PlayerInput/InputListener.cs:64-69,130-147,165-169` plus `Assets/Scripts/PlayerInput/PlayerInput.inputactions`).
- Plan: Add a dedicated toggle action/binding (e.g., Tab/Escape), subscribe in `OnEnable`/unsubscribe in `OnDisable`, and invoke state changes only on the action callbacks.
- Action: Added `OpenInventory` binding, subscribed/unsubscribed in `OnEnable`/`OnDisable`, and used `OpenInventory` to toggle GamePlay/Menu. Cursor lock/visibility and InventoryPanel activation are driven by `GameStateController.OnStateChange`.
- Problem: Inventory UI bootstrap failed because the static dictionary was never instantiated, `InventoryUIController.Awake` returned early when it didn’t find an existing entry, and `ItemPickup.Interact` could hit `KeyNotFoundException` when the player picked up an item before the inventory registered (`Assets/Scripts/InventoryScripts/InventoryUIController.cs:22-67`, `Assets/Scripts/InteractionScripts/ItemPickup.cs:40-73`).
- Plan: Initialize the dictionary, register inventories on Awake (and lazily if needed), expose a singleton for late access, and ensure pickups auto-register before adding.
- Action: Instantiated the dictionary up front, register inventories in `Awake`/`EnsureInventoryRegistered`, added `InventoryUIController.Instance`, and made `ItemPickup` auto-register before adding so pickups succeed.

 

##### Section 3 - (3 Credit)

The game must include a minimum of 3 weapons. A firearm that does not fire automatically, a firearm that does fire automatically, and a weapon that fires multiple projectiles at once such as a shotgun. All 3 weapons must be usable by the player, this means they can be selected in the menu in some way.

- Problem: Only pistol and shotgun assets exist; no third weapon exists/usable to satisfy the required trio (`Assets/WeaponObjects/*`). `WeaponController.Start` assumes an equipped weapon and calls `Unequip` on a null reference, and `OnAttack` ignores the `firstAttack` flag so semi-auto weapons can’t fire on the initial press (`Assets/Scripts/CombatScripts/WeaponController.cs:18-78`).
- Plan: Add a third weapon (e.g., SMG/rifle), guard null before `Unequip`, and pass through `firstAttack` to `WeaponItem.Attack`.
- Action: 
- Problem (new): Reload input requests `GetAllItemsOfType("ammo")`, but `Inventory.GetAllItemsOfType` is inverted and case-mismatched (`"Ammo"` in assets) so the filter removes matching ammo and weapons never find reload sources (`Assets/Scripts/InventoryScripts/Inventory.cs:53-65`, `Assets/Scripts/PlayerInput/InputHandler.cs:33-44`).
- Plan: Fix the string comparison to include matching types (case-insensitive) and return only matching items.
- Action: 
- Problem (new): `DatabaseElement.SetItemDatabaseIndex` assigns `value = _index` instead of `_index = value`, leaving all ScriptableObject elements with index 0 and breaking token lookups (weapons/ammo resolve to the wrong base item) (`Assets/Scripts/DatabaseScripts/DatabaseElement.cs:13-16`).
- Plan: Flip the assignment so `_index` stores the passed-in value; repopulate indices via the inspector button.
- Action: Corrected the setter to assign `_index = value` so elements store their database index (`Assets/Scripts/DatabaseScripts/DatabaseElement.cs:13-16`). Repopulate indices via the Database inspector button.


##### Section 4 - (3 Credit)

When in Menu Mode, the player can move items around their inventory by clicking on it then clicking on a new location. If the item fits, it will be placed in that spot. In the event that the item is stackable and you click on another stackable object of that type, or one can be found within the required spaces the items are merged instead. For example, if the slot has 5 bullets and you try to place 6 onto that slot, the slot will now have 11 bullets.

- Problem: Grid math/fit logic are incorrect—`CalculateIndex` used the wrong formula and should be `y * width + x`; `IsOutOfBounds` should use `>=` to block `x==width`/`y==height`; `CheckNeighboursAreEmpty` iterates incorrect ranges; and `AddItem` only marks diagonally adjacent cells (`+1,+1` start), leaving the rest of the footprint unmarked (`Assets/Scripts/InventoryScripts/Inventory.cs:33-224`). This causes false positives/negatives for placement and lets overlaps occur.
- Plan: Correct index math, bounds checks, neighbor loops (iterate full footprint), and fill all covered slots including the root so occupancy reflects reality.
- Action: Corrected the index formula/bounds checks, filled the entire footprint starting at the root, updated the root slot UI on add, and return true when an item is placed (`Assets/Scripts/InventoryScripts/Inventory.cs:91-179`).
- Problem: Slot occupancy/UI wiring—`Slot.IsOccupied` is inverted (returns true when `_itemInSlot == null`), causing empty slots to be treated as occupied and NREs when dereferencing `GetItemInSlot` (`Assets/Scripts/InventoryScripts/Slot.cs:22-27`). `Slot.AddItem` never subscribes to `itemToken.onAmountChanged`, so stack text never updates; `ChangeSlotUIData` sizes the icon/text to raw item grid size instead of slot-size-scaled; `InventoryUIController.InitSlot` swaps x/y when initializing slots; merge/overflow handling is missing, so partial stacks aren’t split and zero-count items aren’t removed.
- Plan: Flip `IsOccupied` logic, subscribe/unsubscribe to `onAmountChanged`, scale UI by `_slotSize * itemSize`, correct x/y ordering, ensure dictionary init, and implement merge/overflow/zero cleanup.
- Action: Fixed `IsOccupied`, added stack change subscription/unsubscription, scaled visuals by slot pixel size, cleared lingering stack text on empty, removed inventory tokens on clear, and corrected slot initialization order (`Assets/Scripts/InventoryScripts/Slot.cs:22-105,142-156`; `Assets/Scripts/InventoryScripts/InventoryUIController.cs:22-67`; `Assets/Scripts/InventoryScripts/Inventory.cs:209-225`). Set `_slotSize` to `32x32` in the scene `InventoryUIController` to match the grid cell size.

 

##### Section 5 - (2 Credit)

Elements are added to the game world that are damageable. When attacking these objects they will receive some damage. The game must include at least 2 of these elements. These can be as simple as a table that takes damage and is instantly destroyed, or as complex as an enemy. Regardless of what is added, 1 of the elements at least must have a health value that is reduced when taking damage.

- Problem: `IDamageable` exists but no components implement it, so nothing takes damage.
- Plan: Add at least two `IDamageable` components (e.g., destructible prop and health-based enemy) and hook weapon hits to reduce health/destroy.
- Action: 

 

##### Section 6 - (1 Credit)

A key item must be added into the game along with a door that can be opened when the player has the key. The team would like to suggest you look into interfaces to get this done.

- Problem: No key item or door logic exists; `IInteract/IInteractor` are present but unused for this purpose.
- Plan: Create a `KeyItem` (ItemBase) and a door implementing `IInteract` that checks the interactor’s inventory for the key before opening.
- Action: 

 

##### Section 7 - (1 Credit)

Items in the inventory that are stackable accurately display how many items are stacked. In the event a stackable item reaches 0 or less in the stack, the item is completely removed from the inventory.

- Problem: Stack text never updates because slots don’t subscribe to `onAmountChanged`; zero/removal isn’t handled, so empty stacks persist in data/UI.
- Plan: Subscribe in `Slot.AddItem`, update text in the callback, and on zero remove the token from `_itemsInInventory`, clear covered slots, and refresh UI.
- Action: 

 

##### Section 8 - (1 Credit)

The player cannot move through objects they are not meant to, such as walls.

- Problem: Movement uses `transform.position += ...` so it bypasses physics/colliders; adding a Rigidbody alone doesn’t prevent ghosting.
- Plan: Move via `CharacterController.Move` or `Rigidbody.MovePosition` in `FixedUpdate` and ensure colliders are non-trigger.
- Action: Added Rigidbody to the Player and froze rotation to prevent tumbling. (Collision-respecting movement still pending—needs CharacterController/Rigidbody.MovePosition.)

##### Section 9 - (1 Credit)

When shooting a weapon, the trail renderer does not go beyond the bounds of a collider. For example, if shooting at the edge of a wall, the bullet does not pass through the wall.

- Problem: `WeaponItem.HitscanAttack` spherecasts to `float.MaxValue` and always draws to `hit.point`, so trails can extend past thin colliders/edges and ignore configured range; `attackConfiguration.maxDistance` isn’t applied (`Assets/Scripts/ItemScripts/EquipItems/WeaponItems/WeaponItem.cs:140-160`).
- Plan: Use the configured max distance and clamp trail endpoints to the collider surface/trace distance.
- Action: 

 

##### Section 10 - (1 Credit)

The game builds without any issues both in the build process and when trying to run it. The game does not need to be 100% correct, just that it can build and run. A usable version of this build must also be submitted in order to get this credit point.

- Problem: Runtime assembly references UnityEditor (`DatabaseEditor` in `Assets/Scripts/DatabaseScripts/Database.cs`), which will break player builds.
- Action: Wrapped the custom editor in `#if UNITY_EDITOR` and ensured assembly definitions are scoped so editor-only refs stay in editor.

 

**—----------------------------------------------------------------------------------------------------------------------------**

**EXTRA CREDIT**

 

##### Section 11 - (1 Credit)

When an item in the inventory is being held/selected an image of its icon moves with the mouse so the player knows what item they are currently trying to place. If the player clicks outside of the inventory, or changes from Menu Mode back to Game Mode the item is dropped on the floor.

What I found and did
- Not implemented. Plan: add a UI `Image` that follows the mouse when `InventoryUIController.currentlySelectedItem != null`. On click outside inventory or `GameState` change to GamePlay, spawn `ItemPickup` with `SetItemToken` to drop the item.

 

##### Section 12 - (1 Credit)

Make half of the weapons in your game have an “alternative” attack. This attack can be triggered by right clicking instead of left clicking. These attacks must be visually different from the regular attack. For example, you could use a for loop to shoot in a spread as an alternative to shooting 1 shot at a time.

What I found and did
- Input has `AltFire`, but nothing subscribes to it (`Assets/Scripts/PlayerInput/InputListener.cs`). `WeaponItem` has a single attack path. Plan: wire `AltFire` to a second method and add an alternate pattern (e.g., burst/spread) with distinct VFX.

 

##### Section 13 - (1 Credit)

Make some weapons and/or alternate attacks using projectiles instead of raycasting. For example, a grenade launcher. These projectiles must hurt targets.

What I found and did
- Not implemented. Plan: add a projectile prefab with forward motion and collision damage (invokes `IDamageable.TakeDamage`), and create a weapon that fires it.

 

##### Section 14 - (1 Credit)

In the version of the game presented to you, the player must always manually load ammo by pressing R. Change this so that if the player attempts to attack with an empty weapon, and they have ammo, reload the weapon. In addition to this, make the reloading process take some time. Have some means to know how long the process is taking such as a loading bar, an animation, or audio; just some way to know that the weapon is loading and therefore cannot yet be used.

What I found and did
- Not implemented. Plan: on empty clip with matching ammo in inventory, auto‑trigger reload with a timed coroutine; block attacks during reload and show progress in UI/audio.

 

##### Section 15 - (1 Credit)

When a weapon is equipped, have some kind of display that shows how much ammo is within the weapon. This must update when used. In addition to this, if the player fills a weapon with ammo, then drops it, only to pick it up again, the same amount of ammo should be in the firearm.

What I found and did
- Not implemented. Plan: add HUD text bound to the equipped weapon’s `_ammoInClip/clipSize`, update on `Attack`/`ReloadWeapon`. Preserve clip count on dropped weapons by keeping it on the cloned instance and persisting via token when dropped/picked up.
