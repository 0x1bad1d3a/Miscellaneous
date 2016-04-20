# main.py for raytracer2014
# Geoffrey Matthews

import os
import time
import numpy
import pygame
import random
from pygame.locals import *
from camera import *
from light import *
from material import *
from ray import *
from shape import *
from world import *

if __name__ == "__main__":
    main_dir = os.getcwd() 
else:
    main_dir = os.path.split(os.path.abspath(__file__))[0]
data_dir = os.path.join(main_dir, 'data')

def handleInput(screen):
    for event in pygame.event.get():
        if event.type == QUIT:
            return True
        elif event.type == KEYDOWN:
            if event.key == K_ESCAPE:
                return True
            elif event.key == K_s:
                pygame.event.set_blocked(KEYDOWN|KEYUP)
                fname = raw_input("File name?  ")
                pygame.event.set_blocked(0)
                pygame.image.save(screen,fname)
    return False

def main():
    pygame.init()
    screen = pygame.display.set_mode((640,480))
    pygame.display.set_caption('Raytracing!')

    background = pygame.Surface(screen.get_size())
    background = background.convert()
    background.fill((1,0,0))

    screen.blit(background, (0, 0))
    pygame.display.flip()

    going = True
    pixelsize = 128 # power of 2
    width, height = screen.get_size()
    myLights = [Light(direction=(1,1,1),color=(1,1,1))]
    refl = Reflector()
    cubeplanes = [ Plane((1,0,0),(1,0,0),refl),
                   Plane((-1,0,0),(-1,0,0),refl ),
                   Plane((0,1,0),(0,1,0),refl ),
                   Plane((0,-1,0),(0,-1,0),refl ),
                   Plane((0,0,1),(0,0,1),refl ),
                   Plane((0,0,-1),(0,0,-1),refl)]
    blob = PlaneBlob(cubeplanes)
    world = World([blob,
                   Sphere((-3,-2,6),1,Phong(color=(1,1,0))),
                   Sphere((3,-3,-4),2,Phong(color=(1,0.5,0.25))),
                   Sphere((-4,-1,-.5),2,Reflector()),
                   Plane((0,10,0), (0,-1,0), Image('sky.jpg'), shadow=False),
                   Plane((0,-3,0), (0,1,0), Image('ground.jpg'), shadow=False)],
                  myLights,
                  Camera().lookat(eye=(4,3,8), focus=(-10,0,-30), fovy=45.0, aspect=480.0/640.0))
    while going:
        going = not(handleInput(screen))
        while pixelsize > 0:  
            for x in range(0,width,pixelsize):
                xx = x/float(width)
                for y in range(0,height,pixelsize):
                    #clock.tick(2)
                    yy = y/float(height)
                    # draw into background surface
                    color = world.colorAt(xx,yy)
                    #print color
                    if color == None:
                        print "color none"
                        color = world.neutral
                    color = [int(255*c) for c in color]
                    r,g,b = color
                    color = pygame.Color(r,g,b,255)#.correct_gamma(1/2.2)
                    background.fill(color, ((x,y),(pixelsize,pixelsize)))
                if handleInput(screen):
                    return          
               
            #draw background into screen
            screen.blit(background, (0,0))
            pygame.display.flip()
            
            print(pixelsize)
            pixelsize /= 2

#if this file is executed, not imported
if __name__ == '__main__':
    try:
        main()
    finally:
        pygame.quit()
