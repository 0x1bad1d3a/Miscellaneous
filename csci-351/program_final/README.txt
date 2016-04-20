3rd party .dlls used are:

Priority Queue - since .NET for god knows what reason doesn't have one built in.
WpfAnimatedGif - since .gifs aren't animated by default in WPF.

The basic data structure for my application was a graph of evenly spaced nodes, connected in 8 directions on all of its neighbours. A* search was used to select a path in the graph.

The .NET version used was 4.5 and Visual Studio 2013.