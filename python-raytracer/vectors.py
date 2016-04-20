import numpy as np

# return a numpy.array with type numpy.float
def vec(x, y, z):
    return np.array([np.float(x),
                  np.float(y),
                  np.float(z)])                      

# return a normalized vector in direction v
def normalize(v):
    length = np.sqrt(np.sum(np.power((v[0], v[1], v[2]), 2)))
    return vec(np.divide(v[0], length),
               np.divide(v[1], length),
               np.divide(v[2], length))

# return v reflected through normal
def reflect(v, n):
    # 2 * n * np.dot(v, n) - v
    return np.subtract(np.multiply(np.multiply(2,np.dot(v, n)), n), v)
