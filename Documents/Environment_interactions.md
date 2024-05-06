## Door Interaction
The [DoorInteraction](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Environment/DoorInteraction.cs) script manages the interaction between players and doors within the game environment. It controls the opening and closing of doors and triggers visual effects related to cheese presence.

### Overview
The `DoorInteraction` script provides the following functionalities:
* Player Proximity Detection: Detects when players are near the door and displays interaction prompts.
* Door Opening: Allows players to open and close doors by pressing a designated input key.
* Audio Feedback: Plays sound effects when the door is opened.
* Visual Effects: Triggers visual effects when the door is opened and cheese is present inside.

## Check Cheese Inside
The [CheckCheeseInside](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Environment/CheckCheeseInside.cs) script detects the presence of cheese within a specified area and updates a boolean flag accordingly.

### Overview
The CheckCheeseInside script provides the following functionality:
* Cheese Detection: Detects when cheese enters or exits a specified trigger zone.
* Boolean Flag Update: Updates a boolean flag to indicate whether cheese is present inside the trigger zone.
### Key Components
* `isCheeseInside`: Boolean flag indicating whether cheese is present inside the trigger zone.

## Exhaust Fan
The [ExhaustFan](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Environment/ExhaustFan.cs) script controls the interaction of the exhaust fan with the cheese smell. When a cheese player enters the trigger zone of the exhaust fan, it activates the fan effect in the `CheeseSmellController`, simulating the dispersal of cheese smell.

### Overview
The ExhaustFan script provides the following functionality:
* Fan Activation: Activates the fan effect in the CheeseSmellController when a cheese GameObject enters the trigger zone.
* Fan Deactivation: Deactivates the fan effect when the cheese GameObject exits the trigger zone.
### Key Components
* `_cheeseSmellController`: Reference to the `CheeseSmellController` script attached to the cheese GameObject.

## Wind Affect
The [WindAffect](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Environment/WindAffect.cs) script simulates the effect of wind on the cheese smell. When a cheese player enters the trigger zone of the wind affect area, it triggers wind effect in the `CheeseSmellController`, altering the behavior of the cheese smell particles.

### Overview
The WindAffect script provides the following functionality:
* Wind Activation: Activates the wind effect in the CheeseSmellController when a cheese GameObject enters the trigger zone.
* Wind Deactivation: Deactivates the wind effect when the cheese GameObject exits the trigger zone.
### Key Components
* `_cheeseSmellController`: Reference to the `CheeseSmellController` script attached to the cheese GameObject.