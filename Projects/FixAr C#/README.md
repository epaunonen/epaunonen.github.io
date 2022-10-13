# FixAr

FixAr is a fixed point arithmetic library written in C# that is designed for fast paced simulations which require platform independent, deterministic calculations, in situations where lower level code cannot be utilized. 

This library has been created as a hobbyist project in 2018, as a base for experimenting with simple and completely deterministic physics simulations in C#. The original motivation was to allow for deterministic calculations in Unity game engine so that the same exact results would be obtained independent of platform.

Determinism is achieved by simulating decimal numbers using integers. Additionally, look-up-tables and other tricks are used to keep the calculations as fast as possible.
While this library performs well and provides accurate results, the calculations are obviously not completely accurate. The precision of the number representations is configurable, but it should be noted that increasing the precision lowers the maximum values that the library can handle.
 
## Features:
 * Fully implemented basic math library
    - Basic arithmetic operations
    - Trigonometry
    - Powers, logarithms
 * Vectors (2D and 3D)
 * Quaternions (partially)

