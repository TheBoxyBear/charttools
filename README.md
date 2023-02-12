# ChartTools
ChartTools is a .NET library with the purpose of modelling song files for plastic guitar video games like Guitar Hero, Rock Band and Clone Hero. It currently supports reading of .chart and .ini files, with .mid support currently in development. BChart will also be supported once the format is finalised by mdsitton and Midi is implemented.

In order to future-proof the library for features added to Clone Hero, improve code readability, and ensure long-term support, future development will be done in .NET 7 with a temporary .NET 6 compatibility layer. New projects should target .NET 7 using the matching build. Future development may make use of .NET 7 and C# 11 features, with .NET 6 impmenentations included throuh compiler directives.

If you find any bugs, you can report them in the [Issues section](https://github.com/TheBoxyBear/ChartTools/issues) of the repository. Make sure to use the "bug" label.

## Getting Started
For an overview on installation and taking your first steps with ChartTools, see [Getting Started](https://theboxybear.github.io/charttools/articles/GettingStarted.html). A new website is currently in the works with more detailed articles and full API documentation.

## Contributing
If you like to contribute to the development of ChartTools, feel free to comment on an issue, submit a pull request or submit your own issues. To test your code, create a project named `Debug` and it will be automatically excluded from commits.

## License and Attribution
This project is licensed under the GNU General Public License 3.0. See [LICENSE](LICENSE) for details.

This project makes use of one or more third-party libraries to aid in functionality, see [attribution.txt](attribution.txt) for details.

## Special Thanks
- [FireFox](https://github.com/FireFox2000000) for making the Moonscraper editor open-source
- [TheNathannator](https://github.com/TheNathannator) for their direct contributions.
- [mdsitton](https://github.com/mdsitton), lead developer of Clone Hero for sharing their in-depth knowledge and general programming wisdom.
- Members of the [Clone Hero Discord](https://discord.gg/clonehero) and [Moonscraper Discord](https://discord.gg/wdnD83APhE), including but not limited to DarkAngel2096, drumbs (TheNathannator), FireFox, Kanske, mdsitton, Spachi, and XEntombmentX for their help in researching.