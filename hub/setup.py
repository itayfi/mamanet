#!/usr/bin/env python

from setuptools import setup, find_packages

setup(name='mamanet_hub',
      version='1.0',
      install_requires=['expiringdict', 'requests', 'flask'],
      packages = find_packages()
     )
