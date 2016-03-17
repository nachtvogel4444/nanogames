Creating a nanoGame
===================

This guide intends to give a rough outline of where to start when creating a new nanoGame.

For reference, there's a commented "Example" game in the project NanoGames.Games that you can use as a starting point.

Warning: The API for writing nanoGames is not completely finalized at this point.


General contribution guidelines
-------------------------------

  - Every new source file *must* contain the correct license header. Just copy it from another file, or run "./fixheaders.pl" in the root directory, which automatically writes the correct header to all C# files.

  - Please add your name to AUTHORS.txt in the root directory.

  - Code should always compile without warnings, for example those produced by the StyleCop analyzer when it detects style issues.

  - If you install the CodeMaid extension for Visual Studio, many style issues will be fixed automatically when saving.

  - Submit your code as a pull request on GitHub.


Design considerations for nanoGames
-----------------------------------

  - After a game is selected, the action starts immediately, so the ideal nanoGame should be a very simple or familiar idea that players can instantly understand and jump into.

  - A round shouldn't last longer than maybe 2 minutes.

  - The graphics style attempts to emulate the look of CRT vector displays, which could only display very simple line shapes. "Asteroids" is perhaps the most well-known game of this era.

  - Game input is limited to six keys: Up, Down, Left, Right, Fire (Space) and AltFire (Ctrl).


Basic structure of a nanoGame
-----------------------------

All nanoGames are in the NanoGames.Games project. To create a new game called (for example) "Foo", you need to:

  - Create a new folder named "Foo".

      * Create a class FooMatch : Match<FooPlayer>. This class represents the current state of a single match.

      * Create a class FooPlayer : Player<FooMatch>. This class represents the current state of a player in the match.

  - Add your nanoGame as a new "discipline" to the class DisciplineDirectory in the root folder of NanoGames.Games.

After this, the game will be available in the practice menu and also (randomly) as a voting option in multiplayer games.


Match lifecycle
---------------

At the start of a round, the framework will perform these tasks:

  - The match instance and player instances are created.

  - The match and player member variables are filled:

      * Match.Random contains a random number generator that's synced between clients.

      * Match.Players contains a list of all player instances.

      * For every player instance,

          - Player.Match points back to the match instance.

          - Player.Index is the index of the player in the list of players.

          - Player.Color contains the player's chosen color. You can use this to draw the player's character/spaceship etc.

  - Match.Initialize() is called. This is an abstract method that performs initialization specific to the game.

  - For every player instance, Player.Initialize() is called. This is an abstract method that performs initialization specific to the game.


The, for every frame, the framework will do this:

  - For every player instance:

      * Player.Input is set to the current input state of the player (which buttons the player has currently pressed).

      * Player.Graphics is set to the current render output.

  - Match.Update() is called. This is an abstract method that performs update and/or rendering code specific to the game.

  - For every player, Player.Update() is called. This is an abstract method that performs update and/or rendering code specific to the game.

  - Everything drawn onto a player's Graphics object is shown on the real screen.


The game ends when Match.IsCompleted is set to true by an Update() method. In this case:

  - The match ends and no further frames are updated/rendered.

  - Players are ranked according to the Player.Score property, higher scores are better.

      * This score is purely used for ranking/ordering and not shown anywhere. It can be an artificial value just used for ranking.

      * Players get tournament points according to their ranking.

      * Ties are allowed and result in several players getting the same number of tournament points.


Determinism and Netcode
-----------------------

To ensure smooth gameplay, the netcode will sometimes "predict" the game state even when it hasn't received the input from all players. To do this, the netcode saves a copy of the game state, computes the next frames with the best known input, and when a correction arrives, "rolls back" to fix the mistake retroactively.

When coding a game, you don't usually need to worry about any of this, but you do have to make sure that you game runs deterministically, that is, produces the same results when receiving the same input on every machine. This is not hard, but you have to avoid any construct or API call that would depend on the specific machine or time of execution.

For example:

  - Don't store any match state in static variables, only in instance variables. (Static readonly variables for constants are perfectly okay.)

  - Don't access the system time in any way.

  - Only use random numbers provided by Match.Random; this Random instance is synchronized between clients.

  - Don't do anything that depends on .NET-generated object hashcodes, because those are different on every run. For example, the entries in a Dictionary are not guaranteed to be sorted consistently.


Graphics
--------

  - A game has to draw the complete screen content for every frame and for every player.

  - The Graphics object represents a player's screen. It has methods like Line, Rectangle and Circle that can be used to draw.

  - As the screen emulates a CRT vector display, you can only draw lines and points, not fill any shapes.

  - The virtual screen size is always 320*200. This will never change and can be hard-coded into games.

  - The screen origin (0, 0) is in the top-left corner.
