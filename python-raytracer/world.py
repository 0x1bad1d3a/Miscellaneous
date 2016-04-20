import numpy as np

class AbstractWorld():

    def colorAt(self, x, y):
        raise NotImplementedError()

class World(AbstractWorld):

    def __init__(self,
                 objects  = None,
                 lights   = None,
                 camera   = None,
                 maxDepth = 0,
                 neutral  = (0.5,0.5,0.5),
                 nsamples = 1,
                 gamma    = 1):
        
        self.objects  = objects
        self.lights   = lights
        self.camera   = camera
        self.maxDepth = maxDepth
        self.neutral  = neutral
        self.nsamples = nsamples
        self.gamma    = gamma

    def colorAt(self, x, y):
        
        ray = self.camera.ray(x, y)
        dist, point, normal, obj = ray.closestHit(self)
        if obj:
            return obj.material.colorAt(point, normal, ray, self)
        else:
            return self.neutral
