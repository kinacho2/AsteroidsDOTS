
								Asteroids by Joaquin Gaviot



====================================================================================================================
	Design and Mechanics
====================================================================================================================

---------------------------------------------------------------------------------------------------------------------
Player controls:

W: forward
S: backward
A: rotate counter-clockwise
D: rotate clockwise
Space (can keep pressed): shoot projectile
E (keep pressed): hyperspace travel

---------------------------------------------------------------------------------------------------------------------
Player stats:

I decided to give health to player cause the difficult of the game could be to high for a single life
I replaced the temporary shield for a health based permanent shield
The health will be reduced in 1 foreach collision or shoot that the player receives, this includes the shield health too
The player can get different levels of shoot when it get the weapon power-up

* Health: 10
* Shield Health: 4
* Weapons:
	+ Level 1: one single shoot
	+ Level 2: 3 shots to the front
	+ Level 3: 6 shots, 3 to the fron and 3 for backward 
	+ Level 4: 10 shoots, 3 to the fron, 3 for backward and 2 foreach side
* Invulnerability: when the player receives damage it activates an invulnerability timer

---------------------------------------------------------------------------------------------------------------------
Asteroids:

There are 4 sizes of asteroids
* Bigger
* Medium
* Small
* Tiny

Each size as his own attributes that are: 
* max speed 
* size (used for explosions) 
* different shapes 
* mass
* health 

When an asteroid is destroyed will spawn 3 asteroids from the inmediate next smaller level, except for the Tiny level that is the smallest.
The asteroids can collide each other and change their linear speed and angular speed depending on physics.
The tiny asteroids will be destroyed if they collide with a bigger asteroid, this is for avoid the entropy caused by the collisions.
When a player or another ship collides with an asteroid, the ship will be stunned for a little amount of time and it will be pushed away,
in the case of player it will receive 1 point of damage. Enemies will not receive damage form asteroid collisions.

---------------------------------------------------------------------------------------------------------------------
Enemies:

The enemies controls are very similar to player controls, most of the systems are shared between them. The main difference is on the system 
that controls the input
The enemies have an AI system based on a state machine with 4 states
Idle: 
	The enemy will moves forward and check every frame
	* obstacles in the front: in this case the state changes to Avoiding
	* distance to player: in this case the state changes to Aggro
Avoiding: 
	The enemy checks the obstacle velocity and take the decision to rotate 90 degrees in order to avoid the obstacle. 
	This state is executed during a time calculated according the enemy angular speed.
	After finish the timer it will return to Idle state
Aggro: 
	The enemy puts the ship in a position perpendicular to the player direction and changes the state to Attacking
	The shoot has a couldown and this state checks and reduces that couldown before change to Attacking
	If the enemy detect an obstacle in the front before reach the position to attack the state will be changed to Avoiding
	If the distance to player is bigger than a value the state will be changed to Idle
Attacking: 
	The enemy aims the player and shoot a bullet to the player direction, after that the state is changed to Aggro

The enemy bullets can collide with other enemies and asteroids that do the game funniest.

States Diagram:

                          ┌───────────────┐
                          │detect obstacle│
      ╔══════════╗        └──────┬────────┘           ╔══════════╗
      ║          ║═══════════════╧═══════════════════►║          ║
   ╔══║   Idle   ║◄════════════════╤══════════════════║ Avoiding ║
   ║  ║          ║◄═════╗    ┌─────┴──────┐     ╔════►║          ║
   ║  ╚══════════╝      ║    │end avoiding│     ║     ╚══════════╝
   ║                    ║    └────────────┘     ║                             ╔═════════════╗
   ║ ┌───────────┐      ║ ┌───────────────┐     ║                             ║Symbols Used ║
   ║ │player dist│      ║ │  player dist  │     ║                             ║═════════════║
   ╟─┤    <      │      ╟─┤      >        │     ║                             ║ ► ◄         ║
   ║ │ view dist │      ║ │view dist * 1.5│     ║						      ║ ╗ ╝ ╚ ╔ ║ ═ ║
   ║ └───────────┘      ║ └───────────────┘     ║                             ║ ┐ ┘ └ ┌ │ ─ ║
   ║                    ║   ┌───────────────┐   ║						      ║ ╟ ╢ ╧ ╤     ║
   ║       ╔═════════╗  ║   │detect obstacle│   ║                             ║ ┤   ┴ ┬     ║
   ╚══════►║         ║══╝   └──────┬────────┘   ║    ╔═══════════╗            ╚═════════════╝
	       ║  Aggro  ║═════════════╧════════════╝    ║           ║
	   ╔══►║         ║════════════╤═════════════════►║ Attacking ║
	   ║   ╚═════════╝       ┌────┴────┐             ║           ║═══╗
	   ║                     │can shoot│             ╚═══════════╝   ║
       ║                     └─────────┘                             ║
	   ╚══════════════════════════╤══════════════════════════════════╝
	                         ┌────┴──────┐
							 │after shoot│
	                         └───────────┘
  
---------------------------------------------------------------------------------------------------------------------
Powers:

The powers are spawned in the same position where an asteroid or enemy were destroyed. The asteroids that can spawn powers are Bigger and Medium only
Powers dissapears when the player collides with them and give to the player something
The powers the player can gen are 4
* Weapon: 
	Increase the current weapon level, the max level is 4
* Health: 
	Increase the current health in 2, the max health is 10
* Shield: 
	Create a shield with 4 points of health, if it exist just restore the shield health
* Bomb: 
	Explodes in the same time the player get this power. 
	The radius increases during a few seconds
	It will destroy every asteroid and enemy that collides with the bomb circle
	The spawned asteroids after the destruction of their parent will not receive damage from the bomb cause saves an unique bomb id
	This can cause inconsistency when 2 or more bombs explode simultaneously but this case is not so frecuent
	
	
====================================================================================================================
	Graphics
====================================================================================================================

The graphics are line shapes with a single color
I created a shader that computes the alpha channel value depending on the uv coordinates in the next way
	alpha = 1 if uv.y < threshold
    alpha = 0 if uv.y >= threshold
I called it line shader.

I used the Unity PolygonCollider2D component to draw the vertices of the shapes according to the sprite models shape, this taking into account that the 
ECS physics 
support a maximun of 16 points per collider, then I used it to create a plane mesh with the idea of draw only the shape borders.
The idea is create a mesh with uv.y = 0 on the vertices that should be drawed and uv.y = 1 on a center point common to all triangles, this causes the result 
of draw only a portion from the base of the triangle.
First of all I should compute the center point, I called it center of mass. This is the sum of all vertices of the shape divided by the vertices count.
Using this center of mass I can start to create the vertices and uvs array
The vertices are the same vertices than the shape but adding the center of mass on the end.
The uvs are all (0,0) except by the uv corresponding to the center of mass vertex that is (1,1)
Then, I start to create the indices array, suppose I have an N vertices shape and I created an N+1 vertices array that contains the vertices of the shape 
and the center of mass
I get 2 consecutive indices, for example 0 and 1 and put it on the indices then put the N index that correspond to the center of mass 
to create a triangle between 2 shape points and the center of mass. In the case the first point is the last shape point (N), the next point index will be 0.
We need to have careful with the draw triangles direction here, taking into account that the shape vertices order should follow a right hand rule and the 
Unity face culling winnding order is clockwise then the triangle indice should be posted like (0,N,1),(1,N,2),...(N-1,N,0)
Other point to take into account is that the shape should be a little bit regulat to automatize this idea into a simple algorithm that creates the meshes 
using only the shape points
When the line shader draws the mesh and the ovs are computed, there is an interpolation between the uv values of each triangle resulting that the fragments 
now contains a 0 to 1 number according to the distance from the traingle's base (shape points) to the triangle's top (center of mass), 
if I set a value to the alpha channel according to this uv value, then it will draw a line in the base of the triangle

Uvs example: 
Suppose this shape and the X is the center of mass, here I have 4 triangles.
(0,X,1) - (1,X,2) - (2,X,3) - (3,X,0)
The numbers between () are the uvs
And suppose two points A and B
if we interpolate the uvs between point 1 and 2 the result is alwas (0,0), then the uv of the point B has a value close to 0
if we interpolate the uvs between the middle of segment 1,2 and the point X, the result indicates that the uvs of the point A is close to 1.
Now using a threshold to determine the alpha channel value we can see that the B point should have a value of 1 and A point should have a value of 0.

										 (0,0)             (0,0)         
										   3╔───────────────╗2       
											│               │                                           
											│               │
											│     (1,1)     │
											│       X *    *│
											│         A    B│
											│               │
											│               │
										   0╚───────────────╝1                
										 (0,0)             (0,0)


====================================================================================================================
	Physics
====================================================================================================================









====================================================================================================================
	ECS Design
====================================================================================================================

Data Models






Archetypes








Systems

Spawn Systems



Movement Systems



Collision Systems




Checks Systems




Player Input System




Enemies Systems




Game State System




Other systems


====================================================================================================================
	Event System
====================================================================================================================















====================================================================================================================
	Sound System
====================================================================================================================