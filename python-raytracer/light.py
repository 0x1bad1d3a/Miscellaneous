from vectors import *

class Light():

    def __init__(self, direction, color = (1,1,1)):
        self.dir = normalize(vec(*direction))
        self.col = vec(*color)

    def direction(self, point=(0,0,0)):
        return self.dir

    def color(self):
        return self.col

class DistantLight(Light):
    
    def __init__(self, direction=(1,1,1), color=(1,1,1)):
        Light.__init__(self, direction, color)

class PointLight():
    
    def __init__(self, point=(0,0,0), color=(1,1,1)):
        self.point = vec(point)
        self.col   = vec(color)
        
    def direction(self, point=(0,0,0)):
        return normalize(self.point - point)
        
    def color(self):
        return self.col
