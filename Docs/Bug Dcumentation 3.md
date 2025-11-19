Sam Garcia

Bug Documentation 3



##### Section 1 - (1 Credit Point)

The game's code does not compile. No matter what the developers do it just won't let them compile anything. They suspect it has something to do with different scripts not communicating but they cannot find the issue.

Notes, remove later: adding asmdef to make scripts communicate.

What I found and did
- Assemblies: The project uses asmdefs per feature area (Configurators, DatabaseScripts, GameManagers, Interfaces, InventoryScripts, ItemScripts, UtilityScripts). This resolves compile-order/dependency issues the team suspected.
- Database indices: `DatabaseElement.SetItemDatabaseIndex(int)` assigns `value = _index` instead of `_index = value` (`Assets/Scripts/DatabaseScripts/DatabaseElement.cs:14`), preventing elements from getting correct indices. Plan: fix the setter so item tokens resolve the right base item.
- Assembly references: Verify asmdefs don’t pull editor‑only references for player builds. Fix the database index setter so tokens don’t resolve to the wrong element.

 

##### Section 2 - (1 Credit)

The player must be able to move between Game Mode and Menu Mode. This includes the player being able to move, shoot, and look around while in Game Mode. The inventory cannot be seen or interacted with when in Game Mode. The cursor can also not be seen when in Game Mode. When in Menu Mode however, the player cannot perform the actions stated in Game Mode, instead they are able to see their inventory and their cursor is visible for clicking on objects such as buttons.

TODO, remove notes: Unity Event OnMove was assigned the mouse movement function

What I found and did
- State system: `GameStateController` publishes `OnStateChange` and tracks `GamePlay` vs `Menu` (`Assets/Scripts/GameManagers/GameStateController.cs`).
- Input wiring bugs: `EnableGamePlay` unsubscribes instead of subscribing to actions, so Fire/Reload/Interact never trigger. Also `OnOpenInventory` is invoked every frame in `Update` (`Assets/Scripts/PlayerInput/InputListener.cs:29,58-61,106-161`).
- Missing toggle binding: The input action asset has no Inventory/Menu toggle. Plan: add a binding (e.g., Tab/Escape) and call `GameStateController.ChangeStateRequest(...)` to swap states.
- Action: Added `OpenInventory` to Input Actions. During `InputListener.OnEnable` we now subscribe to the `OpenInventory` method and unsubscribe during `InputListener.OnDisable`. The newly created `OpenInventory` method now swaps between  game states whenever it is called.
- Cursor/UI: Drive `Cursor.lockState/visible` and the inventory panel’s active state from `OnStateChange` so movement/shoot/look are disabled in Menu mode and inventory is only visible there.
- Inventory UI: Inside the `InventoryUIController`the OnEnable method is supposed to check if the inventory has already been added to `inventoryDictionary` and only add an inventory if it wasn't already added. It's actually doing the exact opposite and only trying to add the inventory if it's already in `inventoryDictionary`. `inventoryDictionary` was also declared but not instantiated.
- Action: Instantiate `inventoryDictionary` during declaration in `InventoryUIController.cs`. 

 

##### Section 3 - (3 Credit)

The game must include a minimum of 3 weapons. A firearm that does not fire automatically, a firearm that does fire automatically, and a weapon that fires multiple projectiles at once such as a shotgun. All 3 weapons must be usable by the player, this means they can be selected in the menu in some way.

What I found and did
- Weapons present: Pistol and Shotgun are configured (`Assets/WeaponObjects/PistolWeapon.asset`, `Assets/WeaponObjects/ShotgunWeapon.asset`), with `SmallFirearm` and `ShotgunAttack` configs and matching trails.
- Third weapon missing: A distinct non/automatic firearm is not present. Plan: add a third (e.g., SMG or Rifle) using `AttackConfiguration` with appropriate `automaticAttacking` and spread.
- Equip pipeline: `WeaponController.Start()` auto-equips and `ChangeWeapon` `Unequip()`s without a null check, and `OnAttack` ignores `firstAttack` (`Assets/Scripts/CombatScripts/WeaponController.cs:20,30-47,71-79`). Plan: null‑check before unequip and pass through `firstAttack` to honor automatic vs. semi‑auto.

 

##### Section 4 - (3 Credit)

When in Menu Mode, the player can move items around their inventory by clicking on it then clicking on a new location. If the item fits, it will be placed in that spot. In the event that the item is stackable and you click on another stackable object of that type, or one can be found within the required spaces the items are merged instead. For example, if the slot has 5 bullets and you try to place 6 onto that slot, the slot will now have 11 bullets.

What I found and did
- Indexing bugs: `Inventory.CalculateIndex` uses `x * width - y`; it should be `y * width + x` (`Assets/Scripts/InventoryScripts/Inventory.cs:35,45`).
- Bounds check: `IsOutOfBounds` should use `>=` to reject `x==width`/`y==height` (`Assets/Scripts/InventoryScripts/Inventory.cs:74`).
- Neighbour checks: `CheckNeighboursAreEmpty` loops are incorrect; should iterate from `start` to `start+itemSize` and verify occupancy/bounds.
- Add coverage: `AddItem` only marks diagonally adjacent cells (starts at `+1,+1`); should mark all covered cells after the root.
- Slot occupancy: `Slot.IsOccupied` returns true when `_itemInSlot == null` (inverted) (`Assets/Scripts/InventoryScripts/Slot.cs:22-27`).
- UI binding: `Slot.AddItem` never subscribes to `itemToken.onAmountChanged`; subscribe and unsubscribe on clear so `_stackText` stays accurate.
- UI sizing: `ChangeSlotUIData` should size by `_slotSize * itemSize` for correct visuals (`Assets/Scripts/InventoryScripts/Slot.cs:94-116`).
- Dictionary/init/order: `InventoryUIController.inventoryDictionary` is never initialized and `OnEnable` early-returns when the key is missing (logic inverted). Also passes `(y,x)` into `InitSlot(int x,int y,...)` (`Assets/Scripts/InventoryScripts/InventoryUIController.cs:31-41,55-62`).
- Merging: `CanAddItem` merges but doesn’t handle overflow/leftovers or zero removal. Requirement expects partial merges and proper cleanup.

 

##### Section 5 - (2 Credit)

Elements are added to the game world that are damageable. When attacking these objects they will receive some damage. The game must include at least 2 of these elements. These can be as simple as a table that takes damage and is instantly destroyed, or as complex as an enemy. Regardless of what is added, 1 of the elements at least must have a health value that is reduced when taking damage.

What I found and did
- Interface exists (`Assets/Scripts/Interfaces/IDamageable.cs`) but no implementations are present. Plan: add a simple `Destructible` component with health and a one-shot variant.

 

##### Section 6 - (1 Credit)

A key item must be added into the game along with a door that can be opened when the player has the key. The team would like to suggest you look into interfaces to get this done.

What I found and did
- `IInteract/IInteractor` are in place, but no key/door scripts exist. Plan: add a `KeyItem` (`ItemBase`), and a `Door` that implements `IInteract` and checks the interactor’s inventory for the key before opening.

 

##### Section 7 - (1 Credit)

Items in the inventory that are stackable accurately display how many items are stacked. In the event a stackable item reaches 0 or less in the stack, the item is completely removed from the inventory.

What I found and did
- Counts don’t update because slots don’t subscribe to `ItemToken.onAmountChanged`. Plan: subscribe in `Slot.AddItem`, update text in the callback, and unsubscribe on clear.
- Removal at zero: When `AdjustAmount` clamps to 0, the token isn’t removed from `_itemsInInventory` or UI. Plan: remove token and clear covered slots when amount hits 0.

 

##### Section 8 - (1 Credit)

The player cannot move through objects they are not meant to, such as walls.

Added RigidBody to player and froze rotation to prevent tumbling. 

What I found and did
- Movement uses direct `transform.position += ...` which bypasses physics (`Assets/Scripts/PlayerInput/InputHandler.cs:33-44`). Plan: switch to `CharacterController.Move` or `Rigidbody.MovePosition` in `FixedUpdate` and ensure level colliders are non‑triggers.
- Action: Added component `RigidBody` to Player object and froze rotation to prevent tumbling.

##### Section 9 - (1 Credit)

When shooting a weapon, the trail renderer does not go beyond the bounds of a collider. For example, if shooting at the edge of a wall, the bullet does not pass through the wall.

What I found and did
- `WeaponItem.HitscanAttack` uses `SphereCast` and trails from muzzle to `hit.point` (`Assets/Scripts/ItemScripts/EquipItems/WeaponItems/WeaponItem.cs`). This should stop at collider bounds. Plan: also respect `attackConfiguration.maxDistance` and ensure origin projection avoids overshooting thin edges.

 

##### Section 10 - (1 Credit)

The game builds without any issues both in the build process and when trying to run it. The game does not need to be 100% correct, just that it can build and run. A usable version of this build must also be submitted in order to get this credit point.

What I found and did
- Editor code: `DatabaseEditor` lives in `Assets/Scripts/DatabaseScripts/Database.cs` and references UnityEditor in a runtime assembly. This compiles in-Editor but breaks player builds. Plan: move the custom editor into an `Editor` folder or wrap with `#if UNITY_EDITOR`.
- Action: Added missing assembly definition and wrapped the `Database : Editor` class in a preprocessor directive.

 

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
