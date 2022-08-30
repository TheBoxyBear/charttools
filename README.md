---
layout: default
---

# ChartTools

ChartTools is a .NET library with the purpose of modelling song files for plastic guitar video games like Guitar Hero, Rock Band and Clone Hero. It currently supports reading of .chart and .ini files, with .mid support currently in development. BChart will also be supported once the format is finalised by mdsitton and Midi is implemented.

In order to future-proof the library for features added to Clone Hero, improve code readability, and ensure long-term support, future development will be done in .NET 6 with a temporary .NET 5 compatibility layer. New projects should target .NET 6 using the matching build. Future development will be made using .NET 7 upon its release, with the .NET 6 version becoming the compatibility layer and .NET 5 support being discontinued.

Special thanks to [FireFox](https://github.com/FireFox2000000) for making the Moonscraper editor open-source, [TheNathannator](https://github.com/TheNathannator) for their direct contributions, and to members of the [Clone Hero Discord](https://discord.gg/clonehero) and [Moonscraper Discord](https://discord.gg/wdnD83APhE), including but not limited to DarkAngel2096, drumbs (TheNathannator), FireFox, Kanske, mdsitton, Spachi, and XEntombmentX for their help in researching.

## Installation

To add ChartTools to your project, you must first compile the assembly. The repository contains two main projects: Net5 and Net6. Compile the project that matches your target .NET version with either the Debug or Release profile. The compiled files can be found in the build folder of the repository.

Visual Studio: Right-click on your from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll that was generated.

If you find any bugs, you can report them in the [Issues section](https://github.com/TheBoxyBear/ChartTools/issues) of the repository. Make sure to use the "bug" label.

## Getting Started

For an overview on getting started, see [Getting Started](docs/GettingStarted.md).

## License and Attribution

This project is licensed under the GNU General Public License 3.0. See [LICENSE](LICENSE) for details.

This project makes use of one or more third-party libraries to aid in functionality, see [attribution.txt](attribution.txt) for details.
