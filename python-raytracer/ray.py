import numpy as np
from vectors import *

class AbstractRay():

    # return the point at distance along the ray
    def pointAt(self, distance):
        raise NotImplementedError()

    # return (dist, point, normal, obj)
    def closestHit(self, world):
        raise NotImplementedError()
        
class Ray(AbstractRay):

    def __init__(self, point, vector, depth = 0):
        self.point  = vec(*point)
        self.vector = normalize(vec(*vector))
        self.depth  = depth

    def pointAt(self, distance):
        return np.add(self.point, np.multiply(self.vector, distance))

    def closestHit(self, world):

        if self.depth == 5:
            return (None, None, None, None)

        hit_objects = [o.hit(self) for o in world.objects if o.hit(self) is not None]
        
        if hit_objects:
            return min(hit_objects, key=lambda o: o[0]) # closest hit
        else:
            return (None, None, None, None)
