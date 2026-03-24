
# Santa Boss

A Unity boss fight featuring Santa as an aggressive AI opponent with multiple attack patterns.

## Features

- **Chase Behavior**: Boss pursues the player when within range (8 units)
- **Leap Attack**: Performs acrobatic leaps when player is at certain heights or distances
- **Teleport Punch**: Executes a powerful punch attack with cooldown management
- **Stuck Detection**: Automatically recovers if the boss gets trapped or falls off the map
- **State Machine**: Organized behavior using Idle, Chasing, Leaping, and TeleportPunching states

## Configuration

Customize boss difficulty via Inspector parameters:
- `Chase Range`: Distance to trigger pursuit
- `Leap Trigger Height/Distance`: Conditions for leap attacks
- `Punch Cooldown`: Time between punch attacks
- `Stuck Time Limit`: How long before auto-recovery triggers

## Dependencies

Requires companion scripts:
- `BossMovement`
- `BossLeap`
- `BossPunch`
