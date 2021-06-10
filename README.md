# SpherePathfinding_v2
My second attempt at Pathfinding on a sphere. This time it actually works and is fast (enough).
It uses a Icosphere that was subdivided (7 times) as a Mesh.
Every triangle of the mesh represents a node for A* Pathfinding. The grid gets created automatically from the Mesh.

![img](https://i.imgur.com/OOou7Sz.png)
Blue tiles: Start and end node  
Green tiles: Path  
Red tiles: unwalkable nodes / obstacles  

<b> How to use: </b>    
Left click: find path from Player object to Mouse position  
Middle click: toggle clicked node unwalkable status  
Hold right click: rotate sphere  
