# Bears on ATVs

2D vehicle platformer about rogue bears that learn to ride ATVs. Written in Godot.

# Dev Notekeeping

## Collision Layer High Level

- Treat "Mask" and "Layer" as the same thing.
- Wheel" is on level 0, 2, 4, etc. "Bear" is on level 1,3,5, etc. 
     - This is 0 based indexing
- This is not true for the last bit. The last bit is the "AttackWindow"'s only active layer
- Platforms are everything but the AttackWindow last bit
- NPCs are whatever they need to be plus the AttackWindow last bit
