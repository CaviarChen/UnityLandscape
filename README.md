#### UnityLandscape

[All details in code comments]

Terrain Generation:  
    1. Using Diamond Square Algorithm generate a height map which is a two dimension float array storing the height of each point.  
    2. Based on the height map, build vertices and triangles arrays.  

Terrain colouring:  
    1. Based on the height map, calculate the max min and avg height, then decide the sea level.  
    2. Using those values to determine the height between each colour.  
    3. For each vertex, add the right colour based on height. (We also uses Random to add some noise)  

Sea Generation:  
    1. Generate a flat huge square at the sea level.  

Collision:  
    1. Adding an invisible sphere outside the camera.  
    2. Using Unity built-in physics engine to prevent two objects penetrate.  
    3. Locking camera rotation and keep resetting the velocity of the camera to prevent the terrain bounce off the camera.  

Shader:  
    1. Modified based on the Phong Shader from the workshop.  
    (Since this is a terrain, we were trying to reduce the specular part. However, according to the suggestion from a tutor we asked, we left a quite obvious specular highlight)  
