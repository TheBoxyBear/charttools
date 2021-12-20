# ChartTools

ChartTools is a C# library with the purpose of modelling song files for the video game Clone Hero. It currently supports reading of .chart and .ini files, with .mid support currently in development.

In order to future-proof the library for features added to Clone Hero, improve code readability, and ensure long-term support, future development will be done in .NET 6. While .NET 6 is in preview, both .NET versions will be supported, with the solution containing separate projects for .NET 5 and 6.

Special thanks to [FireFox](https://github.com/FireFox2000000) for making the Moonscraper editor open-source, [TheNathannator](https://github.com/TheNathannator) for their direct contributions, and to members of the [Clone Hero Discord](https://discord.gg/clonehero) and [Moonscraper Discord](https://discord.gg/wdnD83APhE), including but not limited to DarkAngel2096, drumbs (TheNathannator), FireFox, Kanske, mdsitton, Spachi, and XEntombmentX for their help in researching.

This library is an independent project not affiliated with Clone Hero or Moonscraper.

## Installation

To add ChartTools to your project using Visual Studio, right-click on Dependencies under your project from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll from the build folder of the repository. All classes are located under the ChartTools namespace.

If you find any bugs, you can report them in the [Issues section](https://github.com/TheBoxyBear/ChartTools/issues) of the repository. Make sure to use the "bug" label.

## Getting Started

For an overview on getting started, see [GettingStarted.md](docs/GettingStarted.md).

## License and Attribution

This project is licensed under the GNU General Public License 3.0. See [LICENSE](LICENSE) for details.

This project makes use of one or more third-party libraries to aid in functionality, see [attribution.txt](attribution.txt) for details.
