import numpy as np
from vectors import *
from material import *

EPSILON = 1.0e-10

class GeometricObject(object):

    def __init__(self, shadow = True):
        self.shadow = shadow

    def castShadows(self):
        return self.shadow

    # Returns (t, point, normal, object) if hit and t > EPSILON
    # Returns None if didn't hit
    def hit(self, ray):
        raise NotImplementedError()

class Plane(GeometricObject):
    
    def __init__(self,
                 point = (0,0,0),
                 normal = (0,1,0),
                 material = None,
                 shadow = True):

        GeometricObject.__init__(self, shadow)

        self.point = vec(*point)
        self.normal = normalize(vec(*normal))
        self.material = material
        if not material:
            self.material = Flat()

    def __repr__(self):
        return "Plane: " + repr(self.point) + repr(self.normal)

    def hit(self, ray):

        VdN = np.dot(ray.vector, self.normal)
        if VdN == 0:
            pass
        elif VdN < 0:
            t = np.dot(self.point - ray.point, self.normal) / VdN
            if t > EPSILON:
                intersect = ray.pointAt(t)
                return (t, intersect, self.normal, self)
            
        return None

class Sphere(GeometricObject):

    def __init__(self,
                 point    = (0, 0, 0),
                 radius   = 1,
                 material = None,
                 shadow = True):

        GeometricObject.__init__(self, shadow)

        self.point    = vec(*point)
        self.radius   = radius
        self.material = material
        if not material:
            self.material = Phong()

    def hit(self, ray):

        # ray's origin and vector
        p  = np.subtract(ray.point, self.point) # solve in object coordinates
        v  = ray.vector

        # quadratic formula time
        a  = 1 # because all Ray objects have vectors normalized
        b  = 2 * np.dot(p, v)
        c  = np.dot(p, p) - self.radius**2
        dt = b**2 - 4*a*c

        # derivitive is negative, so didn't hit
        if dt < 0:
            return None
        # got a hit
        else:
            dt = np.sqrt(dt)

        t0 = (-b + dt) / 2*a
        t1 = (-b - dt) / 2*a

        t = [t0, t1]
        t = [x for x in t if x >= 0]
        if t:
            if len(t) == 2:
                if t[0] < t[1]:
                    t = t[0]
                else:
                    t = t[1]
            else:
                t = t[0]
        else:
            return None

        if t > EPSILON:
            intersect = ray.pointAt(t)
            normal    = normalize(np.subtract(intersect, self.point))
            return (t, intersect, normal, self)
        else:
            return None

class PlaneBlob(GeometricObject):
    
    def __init__(self, planes, shadow = True):
        GeometricObject.__init__(self, shadow)
        self.planes = planes
        
    def hit(self, ray):

        front = []
        back  = []

        for p in self.planes:
            
            VdN = np.dot(ray.vector, p.normal)
            if VdN == 0:
                pass
            else:
                t = np.dot(p.point - ray.point, p.normal) / VdN
                intersect = ray.pointAt(t)
                if VdN < 0:
                    front.append((t, intersect, p.normal, p))
                else:
                    back.append((t, intersect, p.normal, p))

        front.sort(key=lambda x: x[0])
        back.sort(key=lambda x: x[0])
        
        if front[-1][0] < back[0][0]:
            front = [x for x in front if x[0] > 0 and x[0] > EPSILON]
            if front:
                return front[-1]

        return None

class HollowPlaneBlob(GeometricObject):
    
    def __init__(self, planes, shadow = True):
        GeometricObject.__init__(self, shadow)
        self.planes = planes

    def hit(self, ray):

        front = []
        back  = []
        
        for p in self.planes:
            
            VdN = np.dot(ray.vector, p.normal)
            if VdN == 0:
                pass
            else:
                t = np.dot(p.point - ray.point, p.normal) / VdN
                intersect = ray.pointAt(t)
                if VdN < 0:
                    front.append((t, intersect, p.normal, p))
                else:
                    back.append((t, intersect, p.normal, p))

        front.sort(key=lambda x: x[0])
        back.sort(key=lambda x: x[0])
        
        if front[-1][0] < back[0][0]:
            front = [x for x in front if x[0] > 0 and x[0] > EPSILON]
            back  = [x for x in back if x[0] > 0 and x[0] > EPSILON]
            if front and back:
                if type(front[-1][3].material) == Phong:
                    if type(back[0][3].material) == Phong:
                        return None
                    return back[0]
                return front[-1]
        return None
