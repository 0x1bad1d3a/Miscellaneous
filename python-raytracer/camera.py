import numpy as np
from ray import *
from vectors import *

class AbstractCamera():

    def ray(self, x, y):
        raise NotImplementedError()
        
class Camera(AbstractCamera):

    def __init__(self,
                 eye = (0,0,10),
                 ul  = (-10,10,-10),
                 ur  = (10,10,-10),
                 ll  = (-10,-10,-10),
                 lr  = (10,-10,-10)):

        self.eye = vec(*eye)
        self.ul = vec(*ul)
        self.ur = vec(*ur)
        self.ll = vec(*ll)
        self.lr = vec(*lr)

    def ray(self, x, y):

        eye = self.eye
        ul = self.ul
        ur = self.ur
        ll = self.ll
        lr = self.lr

        x1 = ul * (1-x) + ur * x
        x2 = ll * (1-x) + lr * x

        point = x1 + eye
        dist = x2 - x1
        p = point + dist * y
        ray_direc = p - eye

        return Ray(self.eye, ray_direc)

    def lookat(self,
               eye    = (0,0,10),
               focus  = (0,0,-10),
               up     = (0,1,0),
               fovy   = 90.0, # in degrees
               aspect = 1.0):
        
        eye   = vec(*eye)
        focus = vec(*focus)
        up    = vec(*up)
        fovd2 = np.radians(fovy/2)
        ar    = aspect
    
        # find three orthogonal vectors for camera (normalized)
        # | u,v,n = x,y,z |
        n = normalize(eye-focus/np.linalg.norm(eye-focus))
        u = normalize(np.cross(up,n)/np.linalg.norm(np.cross(up,n)))
        v = np.cross(n,u)

        # Find image plane
        D = np.linalg.norm(focus-eye)
        H = np.tan(fovd2) * D * 2
        W = H * ar
        C = eye - n * D

        HH = v * W/2
        WW = u * H/2

        ul = C - WW + HH
        ur = C + WW + HH
        ll = C - WW - HH
        lr = C + WW - HH

        self.eye = eye
        self.ul = ul
        self.ur = ur
        self.ll = ll
        self.lr = lr

        return self
