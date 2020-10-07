See https://github.com/alvariusgames/bears_on_atvs/releases for more info

# v0.0.1-alpha | May 6 2018 | e88f873

A simple example Godot project of a KinematicBody2D wheel moving around StaticBody2D. The simplest type of movement/collision system I'd want to target for this game.

# v0.0.2-alpha | Dec 5 2018 | 6634e9b

Two "Wheely" characters operate independently: but there is an ATV parent node that keeps both of them attached at a certain body length. Uses a spring model instead of a hinge model, with a VERY stiff spring. Still needs some fine tuning, but the general idea works.

# v0.0.3-alpha | Dec 8 2018 | d1481b7

Has a "bear" object on top of the "ATV". When the bear is hit, he will fall off the bike, rendering it useless for a few seconds. He will then get back on the bike to continue riding.Tries to use a FSM model for interacting with state and reacting to movement. Some refactoring done, needs more refactoring.

# v0.0.4-alpha | Jan 6 2019 | 5082800

- Adds camera that follows our hero
- Revamps FSM, adds time delayed FSM state update calls
- Adds animation modes for modes of the bear (Hit, Invincible, Normal, etc.)
- Overhauls the 'flip over ATV if upside down' logic to send the Bear to the last 'safety checkpoint' if the ATV can't figure out how to flip over without causing the bear to get hit again right away.
- Other bug fixes

# v0.0.5-alpha | Jan 19 2019 | 694771c

- Adds a new `IConsumeable` type of Food, which gives our player energy
- Adds an inherited scene model, with the UI being a separate scene that will take in as input any level scene.
- Adds a progress bar that represents our player's health, has it automatically dwindle
- For every food item consumed, it will display the rotating food item and name, as well as add to the total amount of calories
- Increased speed/acceleration of `Wheel` object to make it more fun ;)
- Prototypes loop functionality
- Various bug fixes & improvements, adds new IFSM component logic, etc.

# v0.0.7-alpha | Mar 28, 2019 | a598825

- Adds a "hit" mechanism of a short range attack in whatever direction the user wants
- Adds an `NPC` class
    - Different types of collisions with either `Wheel`/`Bear`/`AttackWindow` have the capability to produce different results (still need to implement the type of different behavior)
- Restructured whole layer/mask system, architected it in a (hopefully) sustainable way

# v0.0.8-alpha | Mar 21 2019 | edba677

- Adds a fancy "speed boost" that temporarily increases speed 
    - A bit sloppy implementation, but seems to handle edge cases well (see crashing on a speed boost)
- NPCs decrease speed when they're hit
- Adds more of a realistic level terrain with items/NPCs sort of spaced out

# v0.0.9-alpha | Apr 1 2019 | 7c1d10a

- Adds rotation when the player in is in midair
- Adds a super-rotation when a user mashes 'attack' when they are mid air
- Fixes the "springy bug" where wheels would shake (might have replaced it with a different "springy bug")
- Tweaked constants for more fun ;)

# v0.0.10-alpha | Apr 6 2019 | f0a887c

- Adds a new 4th possible state for input of "left and right pressing at same time". Makes input much more 'snappy' and responsive
     - For wheels, makes this state `WheelState.IDLE`
     - For rotation, makes this state `RotationManager.NOT_ROTATING`
- Adds in two types of rotation: slow rotation and fast rotation (both can go in both directions)
     - The transition between slow -> fast is that this point defined as when the ATV is in the air AND respective button is being pressed for 0.8 seconds.

# v0.0.11-alpha | Apr 9 2019 | 8647bd4 

This game will entail attacking enemies that are in front of you. The previous camera behavior would put the player on the right of the screen when moving right, on the left of the screen when moving left. This is known as a 'drag' effect. In this game this isn't ideal, as enemies 'sneak up' on you and appear basically right as they are hitting you. Example:

We want to be able to see them in advance so we can attack them.The solution is to implement a "Reverse Drag" system. A player moving right will be on the left of the screen, moving left will be on the right of screen. Vertical is also taken into account, but to a lesser degree. Here is the same path taken in the above gif, but with the "Reverse Drag" camera in place.

When a `Camera2D` is a child of a `Node2D`, it will follow that Node. The default "Drag" camera is a child of the `Player` in the first gif. For the "Reverse Drag", a new `Node2D` `CameraManager` child is added to the `Player`. and the camera is moved to a child of this new `CameraManager`. This `CameraManager` moves around relative to the `Player` depending on the `Player`'s velocity, as well as some defined tuning parameters (how much more horizontal to move, max movement, etc.). This gives the desired "Reverse Drag" effect.

For illustration purposes, the below gif has a camera icon sprite visible on the `Node2D` `CameraManager`. Since the `Camera2D` is following this node, you can clearly visualize how the camera is working in this scenario to give us our desired effect.

Also in this release:
- Adds a better 'run over' effect for running into enemies without eating them (bounces the colliding wheel at 1x magnitude, the other wheel at 0.5x magnitude. Looks and feels right)

# v0.0.12-alpha | July 14 2019 | 4e122f2

This release marks where this public (open source) repository splits with the private repository for this project. All code/scenes/etc will still be released publicly: graphics, animations, music, etc. will not be on this repository.

This release adds an initial draft "recognizable" ATV graphic with wheel. It also prototypes out Godot's 'cutout animation' for a bear character that can be animated .

# v0.0.13-alpha | July 30 2019 | 4b6bd75

- Have a standard black bear on a standard ATV. A few basic animations and rotations, a good starting point even if rough around the edges
- Implemented the very useful [bordered polygon2D](https://github.com/arypbatista/godot-bordered-polygon-2d) for the polygons. This will save a LOT of time and looks great even with the default tileset.
- Proof of concept of a parallax background

# v0.0.14-alpha | Aug 5 2019 | 61d7388

Adds the ability to have one-way platforms and loop-de-loops for ultimate fun.

This was accomplished by programmatically having 3 "Platform Zones". Platform Zone 1 is the "always on" default platform mode, with platform zones 2 & 3 being able to be turned "on" and "off" by colliding the player with the "ZoneCollider" objects. In the case of 1 way platforms, it is on zone 2. Right below the platform, there is a "turn off zone 2" collider. Right above the platform, there is a "turn on all zones" collider. This makes a 1 way platform. Loop-de-loops are accomplished similarly. The right half of the loop is zone 2, the left half is zone 3. On the top of the loop, you turn off the other half of the loop so you can pass by it on your way down. The below picture visualizes this: When our player touches a "star" (the collider), the action in the legend is performed, and the players CollisionMask (what zones they can interact with) is updated.

Also in this release:
- Minor bug fixes
- New speed boost art
- Needing to press "action" when going over a speed boost to activate (much smoother)

# v0.1.0-alpha | Nov 25 2019 | c2ecee4

One fully functional level, ready to download and play! https://alvariusgames.com/blog/bears-on-atvs/alpha-0-1-0-released/

This repository contains all the code/scenes/etc from the public release, but does not contain many of the graphics, and the music/sounds.

# v0.1.1-alpha | Mar 13 2020 | 940cbe0

Download and play v0.1.1-alpha here! https://alvariusgames.com/blog/bears-on-atvs/alpha-1-1-released/

- Added tutorial level to walk users through basic mechanics
- Added 4 directional attack/movement, joystick on android
- Updated inputs to allow for 3 different button presses (attack, forage, use item)
- Changed the 'over interactable' behavior to a magnifying glass above the head (user feedback)
- Added a "smoke explosion" effect when eating items (user feedback)
- Made camera drag less jumpy (user feedback)
- Added basic "life" system, proper dying animation
- Added stage spring item, 1up item
- Finished all major components of the "hub world", easy & intuitive to use now
- Added volume controls to pause menu
- Added user key remapping
- Drastically improved mobile performance by adjusting polygon buffer/index size (see [this github issue](https://github.com/godotengine/godot/issues/19943#issuecomment-581697886))
- Added "long press down" to fall through platforms
- Using `[Export]` tags instead of previous hack for level-specific values for items
- Using Godot's underlying TranslationServer for handling of locales and different languages
- Fixed dynamic "loading bar" behavior

# v0.1.2-alpha | Sep 24 2020 | 

A fed bear is a dead bear... Play now! https://alvariusgames.com/blog/bears-on-atvs/alpha-1-2-released/

- Upgraded to Godot Mono 3.2.3
- Added UI for selecting/deleting of slots
- Refactored FSM base classes
- Improved 1 way platforms to depend on player velocity, not ZoneColliders used for loops
- Added moving platforms to carry a player around
- Added wooden arrow signs for directing players in levels
- Multiple items can now be picked up and added to arsenal
- Added rocket booster item to be included in later level (playable in this alpha if you beat the boss ;) )
- Made sprites of minimized bear crisper
- Added boss fight music, boss interaction leadup jingle, 1up jingle
- Added dialogue system to the LevelFrame, including typing sounds
- Added `BossFightManager` abstract class that all future boss fights (should hopefully) inherit from
- Game works on iOS simulator well, no sound and hacky deploys to physical iOS device over Mac...
- Made a "Park Ranger" human cutout from the `gBot` project Andreas Esau, customized with a diversity procedural generator, different animations and artwork, etc
- Added a generic `Spawner` base class for spawning of rangers (and future items/enemies)
- Created `PursuingEnemy` base class with a very simple AI that can pursue a player, attack, run away, etc. This, when combined with the ranger cutout, creates an OK human for a boss fight (needs some work still)
- Explosions happen on a global scale bug fix
- Fixed the jittery bear rotation when she fell off the TV
- Fixed pause menu sound bug
- Finished level1 zone2
- Added a `DropItemZone` that prevents a character from smuggling items where they're not supposed to
- Added an `ITrackable` interface that puts moving arrows around the level frame when that node is off the screen. Very useful for boss fight and for getting the character to move in the direction you want them to.
- Added spawning animation of swirling smoke
- Fixed Windows crash bug where it would errenously think a slot database didn't exist
- Addressed playtest feedback for various areas of improvement -- boss fight platforms, blueberry bush clarity, etc. Small details on how well a game can be figured out on its own
