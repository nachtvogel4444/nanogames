import random
import numpy as np
import matplotlib.pyplot as plt

def cube(x):
    out = []
    for xx in x:
        if 0 <= xx:
            out.append(xx**(1./3.))
        else:
            out.append(-(-xx)**(1./3.))
    return np.asarray(out)

def invcdf_lin(arr):
    return np.sqrt(4 * arr) / 2

def invcdf_sq(arr):
    return (-0.2 - cube(1 - 3 * (arr + 0.335))) / 1.24


n = int(1e5)
x = np.linspace(0, 1, 100)


number_uniform_dist = np.asarray([random.random() for idx in range(n)])
number_lin_dist = invcdf_lin(number_uniform_dist)
number_quad_dist = invcdf_sq(number_uniform_dist)

plt.subplot(3, 3, 1)
plt.title("Uniform")
plt.plot(number_uniform_dist, 'r.', markersize=0.2)
plt.subplot(3, 3, 2)
plt.hist(number_uniform_dist, 100)
plt.subplot(3, 3, 3)

plt.subplot(3, 3, 4)
plt.title("Linear")
plt.plot(number_lin_dist, 'b.', markersize=0.2)
plt.subplot(3, 3, 5)
plt.hist(number_lin_dist, 100)
plt.subplot(3, 3, 6)
plt.plot(x, invcdf_lin(x))

plt.subplot(3, 3, 7)
plt.title("Quadratic")
plt.plot(number_quad_dist, 'b.', markersize=0.2)
plt.subplot(3, 3, 8)
plt.hist(number_quad_dist, 100)
plt.subplot(3, 3, 9)
plt.plot(x, invcdf_sq(x))

plt.show()

