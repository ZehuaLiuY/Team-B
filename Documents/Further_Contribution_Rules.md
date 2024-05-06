## Code Script Section:
### New issue Guidelines
When submitting a new issue, please adhere to the following guidelines to ensure effective communication and efficient issue resolution:

1. Clear Description: Provide a clear and concise description of the issue you are encountering. Include relevant details such as error messages, steps to reproduce the issue, and expected behavior.
2. Reproducible Steps: If possible, outline step-by-step instructions to reproduce the issue. This helps the development team understand the problem and investigate it more effectively.
3. Environment Details: Specify relevant information about your environment, such as operating system, software versions, and any other configurations that may be pertinent to the issue.
4. Screenshots or Logs: If applicable, attach screenshots or logs that illustrate the issue. This visual aid can provide valuable context and assist in diagnosing the problem.
5. Priority and Labels: Assign appropriate priority levels and labels to the issue to help prioritize and categorize it correctly within the project management system.

By following these guidelines, you contribute to the overall efficiency of issue tracking and resolution, facilitating collaboration and enhancing the project development process.

### Pull Request Guidelines
1. Clear Description: Provide a descriptive title and summary for your pull request. Clearly explain the purpose of the changes and any relevant context.
Scope of Changes: Detail the scope of your changes and how they address specific issues or enhance existing functionality. This helps reviewers understand the intent behind the pull request.
2. Code Quality: Ensure that your code adheres to established coding standards and best practices. Reviewers may provide feedback on code readability, maintainability, and efficiency.
3. Testing: Describe any testing performed to validate the changes. This may include unit tests, integration tests, or manual testing procedures.
Documentation Updates: If applicable, document any changes to project documentation or README files to reflect the updates introduced by the pull request.
4. Review Requests: Assign relevant team members as reviewers to solicit feedback on your changes. This encourages collaboration and ensures thorough review before merging.

By following these guidelines, you contribute to the project's codebase in a structured and collaborative manner, fostering a culture of quality and continuous improvement.

### Pull Request for New Skill Guidelines
1. Make sure you understand the exist skills code structure.
2. Skill Description: Clearly define the new skill, including its name, skill icon, function, and how it is activated within the game. Explain how this skill interacts with existing game elements and mechanics. Include any dependencies or conflicts with other skills or game systems.
3. Balance Assurance: Assess the potential impacts of the new skill on game balance.  Consider how it might affect player strategies and overall gameplay dynamics. Describe any necessary adjustments or tweaks to the skill to maintain balance.  This might include limitations on usage, cooldowns, or modifications to its potency in different contexts.
4. Testing and Validation: Outline detailed plans for testing the new skill in various game scenarios.   Specify both automated and manual testing strategies to cover functionality, integration, and balance.   Provide results from initial tests, highlighting any issues encountered and the adjustments made to resolve them.   Ensure the skill functions as intended without introducing bugs or exploits.


## Modeling section:
### Modelling rule
1. Clear the history of all modeling and bake the high model with the low model to reduce the scene size.
2. If the new areas integrated to the exist game model, you should update the mini-map source map by replace the [image](https://github.com/Jordan-Teaching-Technologist/Team-B/tree/main/Assets/Resources/Images) and update the new world size in the [controller](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Gamemanager/MiniMapController.cs) line 22 & 23.
