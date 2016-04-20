import os
import numpy as np
import pygame
from ray import *
from vectors import *

class Material(object):

    def __init__(self,
                 color = (0.25,0.5,1.0),
                 ambient = 0.2):
        self.color   = vec(*color)
        self.ambient = ambient

    def inShadow(self, ray, world):
        dist, point, normal, obj = ray.closestHit(world)
        if obj:
            if obj.shadow:
                return True
        return False

    def colorAt(self, point, normal, ray, world):
        raise NotImplementedError()
        
class Flat(Material):

    def __init__(self,
                 color   = (0.25,1.0,1.0),
                 ambient = 0.2):
        
        Material.__init__(self, color, ambient)

    def flatColorAt(self, color, point, normal, ray, world):
        
        final_color = self.ambient * color

        for light in world.lights:
            if self.inShadow(Ray(point, light.direction()), world):
                continue
            final_color = color
            
        return final_color

    def colorAt(self, point, normal, ray, world):
        return self.flatColorAt(self.color, point, normal, ray, world)

class Image(Flat):

    def __init__(self,
                 imageFile,
                 imageScale = 10):

        Flat.__init__(self)
        
        self.imageScale = imageScale
        self.img        = pygame.image.load(imageFile)
        self.imgWidth   = self.img.get_width()
        self.imgHeight  = self.img.get_height()

    def colorAt(self, point, normal, ray, world):
        
        p = point * self.imageScale
        x = int(p[0]) % self.imgWidth
        y = int(p[2]) % self.imgHeight
        
        color = vec(*self.img.get_at((x, y))[:3]) / 255
        
        return Flat.flatColorAt(self, color, point, normal, ray, world)
        
class Phong(Material):

    def __init__(self,
                 color         = (0.5,0.5,0.5),
                 specularColor = (1,1,1),
                 ambient       = 0.2,
                 diffuse       = 1.0,
                 specular      = 0.5,
                 shiny         = 64):

        Material.__init__(self, color, ambient)
        
        self.specularColor = vec(*specularColor)
        self.diffuse       = diffuse
        self.specular      = specular
        self.shiny         = shiny

    def phongAt(self, color, specularColor, point, normal, ray, world):
        
        final_color = self.ambient * color

        if np.dot(normal, -ray.vector) < 0:
            normal = -normal
        
        for light in world.lights:
            
            if self.inShadow(Ray(point, light.direction()), world):
                continue
            
            v_L = light.direction()
            v_N = normal
            v_R = reflect(v_L, v_N)
            v_V = -ray.vector

            LdN = max(0, np.dot(v_L, v_N))
            RdV = max(0, np.dot(v_R, v_V))

            Id = light.color() * self.diffuse * color * LdN
            Is = light.color() * self.specular * self.specularColor * RdV**self.shiny
            
            final_color += Id + Is
            
        return np.minimum(final_color, (1,1,1))

    def colorAt(self, point, normal, ray, world):
         return self.phongAt(self.color, self.specularColor, point, normal, ray, world)

class Reflector(Phong):

    def __init__(self):
        Phong.__init__(self)
    
    def colorAt(self, point, normal, ray, world):
        
        newRay = Ray(point, reflect(-ray.vector, normal), ray.depth+1)
        d, p, n, obj = newRay.closestHit(world)

        if obj:
            rcolor = vec(*obj.material.colorAt(p, n, newRay, world))
            return Phong.phongAt(self, rcolor, self.specularColor, point, normal, ray, world)
        else:
            return Phong.phongAt(self, self.color, self.specularColor, point, normal, ray, world)

