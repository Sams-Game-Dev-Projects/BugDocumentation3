Sam Garcia

Bug Documentation 3



##### Section 1 - (1 Credit Point)

The game's code does not compile. No matter what the developers do it just won't let them compile anything. They suspect it has something to do with different scripts not communicating but they cannot find the issue.

Notes, remove later: adding asmdef to make scripts communicate.

 

##### Section 2 - (1 Credit)

The player must be able to move between Game Mode and Menu Mode. This includes the player being able to move, shoot, and look around while in Game Mode. The inventory cannot be seen or interacted with when in Game Mode. The cursor can also not be seen when in Game Mode. When in Menu Mode however, the player cannot perform the actions stated in Game Mode, instead they are able to see their inventory and their cursor is visible for clicking on objects such as buttons.

TODO, remove notes: Unity Event OnMove was assigned the mouse movement function

 

##### Section 3 - (3 Credit)

The game must include a minimum of 3 weapons. A firearm that does not fire automatically, a firearm that does fire automatically, and a weapon that fires multiple projectiles at once such as a shotgun. All 3 weapons must be usable by the player, this means they can be selected in the menu in some way.

 

##### Section 4 - (3 Credit)

When in Menu Mode, the player can move items around their inventory by clicking on it then clicking on a new location. If the item fits, it will be placed in that spot. In the event that the item is stackable and you click on another stackable object of that type, or one can be found within the required spaces the items are merged instead. For example, if the slot has 5 bullets and you try to place 6 onto that slot, the slot will now have 11 bullets.

 

##### Section 5 - (2 Credit)

Elements are added to the game world that are damageable. When attacking these objects they will receive some damage. The game must include at least 2 of these elements. These can be as simple as a table that takes damage and is instantly destroyed, or as complex as an enemy. Regardless of what is added, 1 of the elements at least must have a health value that is reduced when taking damage.

 

##### Section 6 - (1 Credit)

A key item must be added into the game along with a door that can be opened when the player has the key. The team would like to suggest you look into interfaces to get this done.

 

##### Section 7 - (1 Credit)

Items in the inventory that are stackable accurately display how many items are stacked. In the event a stackable item reaches 0 or less in the stack, the item is completely removed from the inventory.

 

##### Section 8 - (1 Credit)

The player cannot move through objects they are not meant to, such as walls.

Added RigidBody to player and froze rotation to prevent tumbling. 

##### Section 9 - (1 Credit)

When shooting a weapon, the trail renderer does not go beyond the bounds of a collider. For example, if shooting at the edge of a wall, the bullet does not pass through the wall.

 

##### Section 10 - (1 Credit)

The game builds without any issues both in the build process and when trying to run it. The game does not need to be 100% correct, just that it can build and run. A usable version of this build must also be submitted in order to get this credit point.

 

**—----------------------------------------------------------------------------------------------------------------------------**

**EXTRA CREDIT**

 

##### Section 11 - (1 Credit)

When an item in the inventory is being held/selected an image of its icon moves with the mouse so the player knows what item they are currently trying to place. If the player clicks outside of the inventory, or changes from Menu Mode back to Game Mode the item is dropped on the floor.

 

##### Section 12 - (1 Credit)

Make half of the weapons in your game have an “alternative” attack. This attack can be triggered by right clicking instead of left clicking. These attacks must be visually different from the regular attack. For example, you could use a for loop to shoot in a spread as an alternative to shooting 1 shot at a time.

 

##### Section 13 - (1 Credit)

Make some weapons and/or alternate attacks using projectiles instead of raycasting. For example, a grenade launcher. These projectiles must hurt targets.

 

##### Section 14 - (1 Credit)

In the version of the game presented to you, the player must always manually load ammo by pressing R. Change this so that if the player attempts to attack with an empty weapon, and they have ammo, reload the weapon. In addition to this, make the reloading process take some time. Have some means to know how long the process is taking such as a loading bar, an animation, or audio; just some way to know that the weapon is loading and therefore cannot yet be used.

 

##### Section 15 - (1 Credit)

When a weapon is equipped, have some kind of display that shows how much ammo is within the weapon. This must update when used. In addition to this, if the player fills a weapon with ammo, then drops it, only to pick it up again, the same amount of ammo should be in the firearm.