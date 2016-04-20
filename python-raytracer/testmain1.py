# main.py for raytracer2014
# Geoffrey Matthews

import os, pygame
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
    myCamera = Camera(ll = (-8,-8,-10),
                      ur = (8,8,-10))
    myLights = [Light((1,1,1),(1,0,0)),
                Light((-1,1,1),(0,1,0)),
                Light((0,1,1),(0,0,1))]
    world = World([Sphere((2,2,.5),3,Phong(color=(1,1,1))),
                   Sphere((2,-2,0),3,Phong(color=(0,1,0))),
                   Sphere((-2,0,-.5),3,Phong(color=(0,0,1)))],
                  myLights,
                  myCamera)
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
