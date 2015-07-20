from setuptools import find_packages, setup


setup(
    name='ObjReader',
    version=1,
    description="objreader",
    packages=find_packages(),
    install_requires=['psutil', 'win32ui', 'win32api', 'win32con']
)