import csv
import math
import os, glob
import numpy as np

path = os.getenv('LOCALAPPDATA')
path = path + "\..\LocalLow\DefaultCompany\CarsML2021-main"
print (path)
os.chdir(path)

csvCounter = len(glob.glob1(path,"*.csv")) #make sure we know how many csv files we have
print(csvCounter)

arrayPPO = []
arraySAC = []



tempArr = []

#Create data for PPO
for file in glob.glob("*PPO*.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        data = csv.reader(csvfile)
        for row in data:
            arrayPPO.append(float(row[0]))
        

meanPPO = sum(arrayPPO) / len(arrayPPO)

#Create data for PPO
for file in glob.glob("*SAC*.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        data = csv.reader(csvfile)
        for row in data:
            arraySAC.append(float(row[0]))


meanSAC = sum(arraySAC) / len(arraySAC)

directoryCounter = len(glob.glob1(path,"Heuristic-*")) #make sure we know how many csv files we have

tempArr = []
array = []
for subPath in glob.glob("Heuristic-*"):
    print(subPath)
    os.chdir(subPath)
    for file in glob.glob("*Heuristic*.csv"):
        print(file)
        i = 0
        with open(file, 'r') as csvfile:
            plots = csv.reader(csvfile)
            for row in plots:
                tempArr.append(float(row[0]))
    print("Adding {} to array".format(tempArr))
    array.append(tempArr)        
    print(array)
    tempArr = []
    os.chdir(path)

meanOfHeuristic = []

for arr in array:
    meanOfHeuristic.append(sum(arr) / len(arr))

print("Heuristic")
print(meanOfHeuristic)