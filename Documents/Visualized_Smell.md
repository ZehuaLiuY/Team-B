In our game, we enhance immersion and realism by visualizing smells within the game world. This section outlines how we achieve the visual representation of smells.

## Smell emitted by Cheese Player
In this part, we focus on visualizing the smell emitted by the Cheese player and its interaction with the game environment.

### Implementation Principle
Visualized smell is simulated by generating particle effects to mimic the diffusion and propagation of smells within the game world. We utilize Unity's Particle System component to create smell particle effects and trigger and control their generation and presentation based on specific conditions and scenarios.

### Implementation Details
In the  [CheeseSmellController](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Player/Cheese/CheeseSmellController.cs) script, we define the following key functionalities to implement visualized smells:

* `smellParticlePrefab`: The prefab for odor particles used to generate smell effects under normal circumstances.
* `smellEverywhere`: The particle system used to generate smell effects when wind is present.
* `GenerateSmellRPC` method: A remote procedure call method to generate smell effects based on different scenarios.
* `setFan` method: Used to set whether there is an exhaust fan effect and specify the position of the exhaust fan.
* `SetFanEffect` method: When an exhaust fan is nearby, adjust the behavior of smell particles based on the position of the exhaust fan.

### Usage
In the game, the generation of Cheese's smell effect is automatically managed by the CheeseSmellController script. Depending on different conditions and scenarios, such as wind or proximity to an exhaust fan, the game will automatically call corresponding methods to generate and control the presentation of smell effects.

### Considerations
When using visualized smell effects, it's important to consider the following:

* Control the frequency and range of smell effect generation to avoid excessive performance consumption and impact on gameplay experience.
* Consider the interaction of smell effects with other game elements, such as the player's perception range and the speed of smell diffusion, to enhance the game's realism and playability.

By considering these points, you can better understand and utilize visualized smell effects to add more fun and experience to the game.

## Interaction with Human Players
In this part, we discuss the interaction between the visualized smell and Human players in the game.

### Implementation Principle
Visualized smell interacts with Human players by providing cues. The smell effect, captured by an additional camera and overlaid onto the main camera to indicate the direction of the Cheese player.

### Implementation Details
The interaction between visualized smell and Human players is managed through game logic and player feedback mechanisms. The [SmellGuide](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Player/Human/SmellGuide.cs) script attached to the Human player, the activation and deactivation of smell indicators based on the player's proximity to the Cheese player.

`SmellGuide` Script: This script is responsible for managing the visual indicators of smell direction displayed on the UI. It utilizes triggers to detect when Human players enter the proximity of a Cheese player (tagged as "Target"). Upon entering the trigger zone, if the local player's camera is not already displaying any other smell indicators (`_leftSmell`, `_rightSmell`, `_bottomSmell`), the corresponding smell element representing the current smell direction (`_currentSmellEffect`) is activated to provide feedback to the player.
`OnTriggerEnter(Collider other)`: This method is called when another collider enters the trigger zone. In this case, it checks if the collider belongs to a target object. If so, and if the current player owns this `photonView` and no other smell indicators are active, it activates the UI element representing the current smell direction.
`OnTriggerExit(Collider other)`: This method is called when another collider exits the trigger zone. Similarly, it checks if the collider belongs to a target object and if the current player owns this `photonView`. If conditions are met, it deactivates the UI element representing the current smell direction.

### Considerations
When implementing visualized smell effects for both the Cheese player and Human players, consider factors such as performance optimization, player feedback, and overall game balance to ensure an immersive and enjoyable gameplay experience.

By addressing these aspects, you can effectively integrate visualized smell mechanics into the game to enhance immersion and interaction.