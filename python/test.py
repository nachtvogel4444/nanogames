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
    return np.sqrt(arr)


def invcdf_sq(arr):
    return cube(arr)

def invcdf_sqrt(arr):
    return arr**(2./3.)


n = int(1e5)
x = np.linspace(0, 1, 1000)


number_uniform_dist = np.asarray([random.random() for idx in range(n)])
number_lin_dist = invcdf_lin(number_uniform_dist)
number_quad_dist = invcdf_sq(number_uniform_dist)
number_sqrt_dist = invcdf_sqrt(number_uniform_dist)

plt.subplot(4, 3, 1)
plt.title("Uniform")
plt.plot(number_uniform_dist, 'r.', markersize=0.2)
plt.subplot(4, 3, 2)
plt.hist(number_uniform_dist, 100)
plt.subplot(4, 3, 3)

plt.subplot(4, 3, 4)
plt.title("Linear")
plt.plot(number_lin_dist, 'b.', markersize=0.2)
plt.subplot(4, 3, 5)
plt.hist(number_lin_dist, 100)
plt.subplot(4, 3, 6)
plt.plot(x, invcdf_lin(x))

plt.subplot(4, 3, 7)
plt.title("Quadratic")
plt.plot(number_quad_dist, 'b.', markersize=0.2)
plt.subplot(4, 3, 8)
plt.hist(number_quad_dist, 100)
plt.subplot(4, 3, 9)
plt.plot(x, invcdf_sq(x))

plt.subplot(4, 3, 10)
plt.title("Quadratic")
plt.plot(number_sqrt_dist, 'b.', markersize=0.2)
plt.subplot(4, 3, 11)
plt.hist(number_sqrt_dist, 100)
plt.subplot(4, 3, 12)
plt.plot(x, invcdf_sqrt(x))

plt.show()

