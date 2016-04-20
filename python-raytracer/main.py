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

from teapotmaterials import *

if __name__ == "__main__":
    main_dir = os.getcwd()
else:
    main_dir = os.path.split(os.path.abspath(__file__))[0]
data_dir = os.path.join(main_dir, "data")

def handleInput(screen):
    for event in pygame.event.get():
        if event.type == QUIT:
            return True
        elif event.type == KEYDOWN:
            if event.key == K_ESCAPE:
                return True
            elif event.key == K_s:
                pygame.event.set_blocked(KEYDOWN|KEYUP)
                fname = raw_input("File name? ")
                pygame.event.set_blocked(0)
                pygame.image.save(screen,fname+".png")
    return False
    
def main():
    pygame.init()
    screen = pygame.display.set_mode((640, 640))
    pygame.display.set_caption("Raytracing!")
    
    background = pygame.Surface(screen.get_size())
    background = background.convert()
    background.fill((1,0,0))
    
    screen.blit(background, (0,0))
    pygame.display.flip()

    pixelsize = 128 # power of two
    width, height = screen.get_size()
    world = World([hollowPlaneBlob(),
                   Sphere((20,0,-10), 4, material=Reflector()),
                   Sphere((-20,0,-10), 4, material=Reflector()),
                   Sphere((0,20,-10), 4, material=Reflector()),
                   Sphere((0,0,-10), 8, material=Reflector()),
                   Plane((0,-40,0), (0,1,0), material=Image("ground.jpg"), shadow=False),
                   Plane((0,40,0), (0,-1,0), material=Image("sky.jpg"), shadow=False)],
                  [Light((1,1,1))],
                  Camera().lookat(eye=(0,0,20), fovy=45.0, aspect=480.0/640.0))
    world = checkersWorld()

    going = True
    while going:
        going = not handleInput(screen)
        while pixelsize > 0:
            for x in range(0, width, pixelsize):
                xx = x/float(width)
                for y in range(0, height, pixelsize):
                    yy = y/float(height)
                    # find color of the world at pixel on screen
                    color = world.colorAt(xx, yy)
                    color = [int(255*c) for c in color]
                    r,g,b = color
                    color = pygame.Color(r, g, b, 255)
                    background.fill(color, ((x,y), (pixelsize,pixelsize)))
                if handleInput(screen):
                    return

                # draw background into screen
                screen.blit(background, (0,0))
                pygame.display.flip()
            
            print(pixelsize)
            pixelsize /= 2

def cylindricalPlanes(radius      = 4,
                      height      = 5,
                      center      = (0,0,0),
                      material    = Reflector(),
                      topmaterial = Phong(),
                      degrees     = 40):
    
    center = vec(*center)
    planes = []

    for theta in range(0, 360, degrees):
        
        # convert to radians
        t = np.radians(theta)

        # find point in model space
        x = radius * np.cos(t)
        y = radius * np.sin(t)
        p = vec(x,0,y)
        
        # transform to world space
        planept = p + center
        planenorm = normalize(planept - center + vec(0,2,0)) # make it a cone
        planes.append(Plane(planept, planenorm, material))

    # find top and bottom planes
    Hd2 = vec(0, height/2, 0)

    top = center + Hd2
    bot = center - Hd2
    topn = normalize(vec(0,1,0))
    botn = normalize(vec(0,-1,0))
    
    planes.append(Plane(bot, botn, topmaterial))
    planes.append(Plane(top, topn, topmaterial))

    return planes

def cylinderPlaneBlob(center, color):
    
    return PlaneBlob(cylindricalPlanes(center=center, topmaterial=color))

def sphericalPlanes(radius      = 12,
                       center   = (0,0,-10),
                       material = Reflector(),
                       degrees  = 10):

    center = vec(*center)
    planes = []
    
    skip = 0
    for inc in range(degrees, 180, degrees):
        for azi in range(degrees, 360, degrees):

            # convert to radians
            i = np.radians(inc)
            a = np.radians(azi)
            
            # find point in model space
            x = radius * np.sin(i) * np.cos(a)
            y = radius * np.sin(i) * np.sin(a)
            z = radius * np.cos(i)
            p = vec(x,z,y)

            # transform to world space
            planept = p + center
            planenorm = normalize(planept - center)
            skip+=1
            if skip % 2 == 0:
                planes.append(Plane(planept, planenorm, material))
            else:
                planes.append(Plane(planept,
                                    planenorm,
                                    Phong(color = (random.random(),
                                                   random.random(),
                                                   random.random()))))
    return planes

def spherePlaneBlob():
    
    return PlaneBlob(sphericalPlanes())

def hollowPlaneBlob():
    
    return HollowPlaneBlob(sphericalPlanes())
            

def randomPlaneBlob(radius   = 4,
                    center   = (0,0,-10),
                    material = Reflector(),
                    sides    = 24):

    def randpt(radius):
        x = (random.random() - 0.5) * radius * 2
        y = (random.random() - 0.5) * radius * 2
        z = (random.random() - 0.5) * radius * 2
        return vec(x,y,z)
    
    center = vec(*center)
    planes = []

    for i in range(sides):
        planept = center + randpt(radius)
        planenorm = normalize(planept - center)
        planes.append(Plane(planept, planenorm, material))
    return PlaneBlob(planes)

def checkersWorld():

    RED = Flat((1,0.2,0.2))
    WHITE = Flat((1,1,1))

    return World([
                  # row 1
                  cylinderPlaneBlob((35,0,35), RED),
                  cylinderPlaneBlob((15,0,35), RED),
                  cylinderPlaneBlob((-5,0,35), RED),
                  cylinderPlaneBlob((-25,0,35), RED),
                  # row 2
                  cylinderPlaneBlob((25,0,25), RED),
                  cylinderPlaneBlob((5,0,25), RED),
                  cylinderPlaneBlob((-15,0,25), RED),
                  cylinderPlaneBlob((-35,0,25), RED),
                  # row 3                  
                  cylinderPlaneBlob((35,0,15), RED),
                  cylinderPlaneBlob((15,0,15), RED),
                  cylinderPlaneBlob((-5,0,15), RED),
                  cylinderPlaneBlob((-25,0,15), RED),
                  
                  # row 1
                  cylinderPlaneBlob((-35,0,-35), WHITE),
                  cylinderPlaneBlob((-15,0,-35), WHITE),
                  cylinderPlaneBlob((5,0,-35), WHITE),
                  cylinderPlaneBlob((25,0,-35), WHITE),

                  # row 2
                  cylinderPlaneBlob((-25,0,-25), WHITE),
                  cylinderPlaneBlob((-5,0,-25), WHITE),
                  cylinderPlaneBlob((15,0,-25), WHITE),
                  cylinderPlaneBlob((35,0,-25), WHITE),

                  # row 3
                  cylinderPlaneBlob((-35,0,-15), WHITE),
                  cylinderPlaneBlob((-15,0,-15), WHITE),
                  cylinderPlaneBlob((5,0,-15), WHITE),
                  cylinderPlaneBlob((25,0,-15), WHITE),
                  
                  Plane((0,0,0), (0,1,0), material=Image("checkerboard.png"), shadow=False),
                  Plane((0,40,0), (0,-1,0), material=Image("sky.jpg"), shadow=False)],
                 [Light((1,1,1))],
                 Camera().lookat(eye=(0,40,0), focus=(0,-1000,-1)))
                 #Camera().lookat(eye=(40,10,0), focus=(0,0,-10)))

def ringWorld(nloops=2, nspheres=100, nspirals=3):
    myLights = [Light((1,1,1))]
    myObjects = []
    floops = float(nloops) + 1.0/nspirals
    radius1 = 500
    radius2 = 100
    for i in range(nspheres):
        angle1 = 2*numpy.pi*nspirals*i/float(nspheres)
        angle2 = floops*angle1
        s1 = numpy.sin(angle1)
        s2 = numpy.sin(angle2)
        c1 = numpy.cos(angle1)
        c2 = numpy.cos(angle2)
        w = radius1 + c2*radius2
        h = s2*radius2
        x = c1*w
        y = s1*w
        z = h
        pct = 6.0*float(i)/float(nspheres)
        if 0 <= pct and pct < 1:
            color = (1,pct,0)
        elif 1 <= pct and pct < 2:
            pct = pct - 1
            color = (1-pct, 1, 0)
        elif 2 <= pct and pct < 3:
            pct = pct - 2
            color = (0,1,pct)
        elif 3 <= pct and pct < 4:
            pct = pct - 3
            color = (0,1-pct,1)
        elif 4 <= pct and pct < 5:
            pct = pct - 4
            color = (pct,0,1)
        else:
            pct = pct - 5
            color = (1,0,1-pct)
        myObjects.append(Sphere((x, y, z - 2000),
                                60,
                                Phong(color)))
    myEye = vec(0,0,150)
    myCamera = Camera(eye = myEye,
                      ul = (-50,50,0)-myEye,
                      ur = (50,50,0)-myEye,
                      ll = (-50,-50,0)-myEye,
                      lr = (50,-50,0)-myEye
                      )
    return World(myObjects, myLights, myCamera)

def teapotWorld():
    myLights = [Light((1,1,1))]
    myObjects = []
    myMaterials = teapotMaterials()
    for x in range(4):
        for y in range(6):
            i = x*6 + 5 - y
            myObjects.append(Sphere((x*3, y*2, 0),
                                    1,
                                    myMaterials[i]
                                    ))
    myEye = vec(3,5,100)
    myCamera = Camera(eye = myEye,
                      ul = (-1,11,0)-myEye,
                      ur = (10,11,0)-myEye,
                      ll = (-1,-1,0)-myEye,
                      lr = (10,-1,0)-myEye)
    return World(myObjects, myLights, myCamera)
                    
    
if __name__ == "__main__":
    try:
        main()
    finally:
        pygame.quit()
