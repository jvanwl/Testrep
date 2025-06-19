# Contributing to Advanced Civilization Building Game

First off, thank you for considering contributing to our game! It's people like you that make this project such a great tool.

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check [this list](https://github.com/yourusername/civilization-game/issues) as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

* Use a clear and descriptive title
* Describe the exact steps which reproduce the problem
* Provide specific examples to demonstrate the steps
* Describe the behavior you observed after following the steps
* Explain which behavior you expected to see instead and why
* Include screenshots and animated GIFs if possible
* Include your Unity version and build settings

### Suggesting Enhancements

Enhancement suggestions are tracked as [GitHub issues](https://github.com/yourusername/civilization-game/issues). When creating an enhancement suggestion, please include:

* Use a clear and descriptive title
* Provide a step-by-step description of the suggested enhancement
* Provide specific examples to demonstrate the steps
* Describe the current behavior and explain which behavior you expected to see instead
* Explain why this enhancement would be useful
* List some other games or applications where this enhancement exists

### Pull Requests

* Fill in the required template
* Do not include issue numbers in the PR title
* Include screenshots and animated GIFs in your pull request whenever possible
* Follow our [coding standards](docs/CODING_STANDARDS.md)
* Document new code
* End all files with a newline

## Styleguides

### Git Commit Messages

* Use the present tense ("Add feature" not "Added feature")
* Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
* Limit the first line to 72 characters or less
* Reference issues and pull requests liberally after the first line
* Consider starting the commit message with an applicable emoji:
    * 🎮 when adding game features
    * 🐛 when fixing a bug
    * 📝 when adding documentation
    * 🎨 when improving the format/structure of the code
    * ⚡️ when improving performance
    * 🔒 when dealing with security
    * ⬆️ when upgrading dependencies
    * ⬇️ when downgrading dependencies

### C# Styleguide

* Use PascalCase for class names
* Use camelCase for method arguments and local variables
* Use PascalCase for public members and methods
* Use readonly where possible
* Use var only when the type is obvious
* Place using directives at the top of the file
* Place the most specific using directives first
* Add XML documentation for public APIs

### Unity Specific Guidelines

* Use [SerializeField] instead of public for inspector variables
* Keep MonoBehaviours as lightweight as possible
* Use scriptable objects for configuration
* Follow the Unity naming conventions for special methods
* Optimize for mobile performance

## Project Structure

```
Assets/
├── Scripts/          # C# source files
├── Resources/        # Game resources
├── Scenes/          # Unity scenes
├── Prefabs/         # Prefab assets
├── Materials/       # Material assets
├── Textures/        # Texture assets
└── Tests/           # Test files
```

## Development Process

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Testing

* Write unit tests for new code
* Ensure all tests pass before submitting PR
* Include integration tests for new features
* Test on multiple Android devices if possible

## Documentation

* Update README.md with details of changes to the interface
* Update API documentation
* Update CHANGELOG.md
* Comment your code
* Write meaningful commit messages

## Questions?

Feel free to open an issue or join our [Discord community](https://discord.gg/yourdiscord).
