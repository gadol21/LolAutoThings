from setuptools import setup, find_packages

setup(
    name='league',
    version='1',
    description='league of Legends swag',
    packages=find_packages(),
	data_files = ['league/_objreader.pyd', 'league/InjectedDll.dll']
)