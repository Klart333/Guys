# Guys
 A bare-bones copy of the game Worms with three dimensional destructible terrain. 

# How to play 
Setup/Controlls
- Select player count, supports however many players you want
- Move your guy with default movement for your device
	- Max 10 meters of movement each turn (calculated from start point)
- Either throw a punch (M1/Right Trigger) or a grenade (M2/Left Trigger)
 	- One action per turn

Goal
- Damage other guys by knocking them into water
- Last guy standing wins

# Features
General
 - O (G) Only play scene is required
 - X (VG, small) Add main menu (start) scene and game over scene
 - X (VG, medium) Implement Pause menu and settings menu

Turn based game
 - O (G) You can have two players using the same input device taking turns.
 - O (VG, large) Support up to 4 players (using the same input device taking turns)
 - X (VG, large) Implement a simple AI opponent.

Terrain
 - O (G) Basic Unity terrain or primitives will suffice for a level
 - O (VG, large) Destructible terrain (You can use Unity's built in terrain or your own custom solution)

Player
 - O (G) A player only controls one worm
 - O (G) Use the built in Character Controller. Add jumping.
 - O (G) Has hit points
 - X (VG, small) Implement a custom character controller to control the movement of the worm.
 - O (VG, small) A worm can only move a certain range 
 - X (VG, medium) A player controls a team of (multiple worms)

Camera
 - O (G) Focus camera on active player
 - X (VG, small) Camera movement

Weapon system
 - O (G) Minimum of two different weapons/attacks, can be of similar functionality, can be bound to an individual button, like weapon 1 is left mouse button and weapon 2 is right mouse button
 - X (VG, small) a weapon can have ammo and needs to reload
 - O (VG, medium) The two types of weapons/attacks must function differently, I.E a pistol and a hand grenade. The player can switch between the different weapons and using the active weapon on for example left mouse button
 - X (VG, medium) Pickups
	- Spawning randomly on the map during the play session
	- Gives something to the player picking it up, I.E health, extra ammo, new weapon, armour etc
 - X (VG, medium) Cheat functionalities
	- Two different cheats, I.E Invincible, all weapons on start etc

Miscellaneous
 - X (VG, medium) Battle royal, danger zones that move around on the map after a set amount of time
 - X (VG, medium) High score that is persistent across game sessions

# Target Grade
VG