# Diamond

Diamond is a reverse-engineering and enhancement project for Martial Heroes, named after the game’s original engine/framework. It provides tooling, parsers, and client-side improvements built around the game’s data formats and runtime behavior.

# Templates
In the templates folder you will find various mapped out file format structures in 010-editor binary template format.

# Subprojects
## Diamond

A .NET library containing binary parsers for Martial Heroes file formats.
Used as the core dependency for all other tools.

## Diamond.VfsCmd

A command-line utility for unpacking data.inf / VFS archives.

## Diamond.WikiGenCmd

A multitool that extracts, normalizes, and correlates game data into Wiki-ready tables.

## Diamond.MotCmd

An experimental animation parser.
Currently non-functional and used only for research.

## Diamond.Fury

A C++ ASI module that injects client-side enhancements into the game.

### Building
- Add 'MARTIALHEROES_DIR' environment variable and set it to the full path of the game install.
- Copy 'Diamond.Fury.ini' to the game directory and change settings to your liking
- Build the solution
  - Optionally: run the solution; it will boot the game with the debugger and module attached
