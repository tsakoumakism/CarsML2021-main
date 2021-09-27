import matplotlib.pyplot as plt
import csv
import math
from array import *
import numpy as np
import os, glob

path = os.getenv('LOCALAPPDATA')
path = path + "\..\LocalLow\DefaultCompany\CarsML2021-main"


try:
       os.mkdir(path + "\ResultsHeuristic")
except OSError as error:
              print(error)


print (path)
os.chdir(path)

directoryCounter = len(glob.glob1(path,"Heuristic-*")) #make sure we know how many csv files we have
print(directoryCounter)

tempArr = []
arrayBestTimes = []
arrayMeanTimes = []


for subPath in glob.glob("Heuristic-*"):
    print(subPath)
    os.chdir(subPath)
    for file in glob.glob("*Heuristic*-BestLapTimes.csv"):
        print(file)
        i = 0
        with open(file, 'r') as csvfile:
            plots = csv.reader(csvfile)
            for row in plots:
                tempArr.append(float(row[0]))
    print("Adding {} to array".format(tempArr))
    arrayBestTimes.append(tempArr)        
    print(arrayBestTimes)
    tempArr = []
    os.chdir(path)

for subPath in glob.glob("Heuristic-*"):
    print(subPath)
    os.chdir(subPath)
    for file in glob.glob("*Heuristic*-MeanLapTimes.csv"):
        print(file)
        i = 0
        with open(file, 'r') as csvfile:
            plots = csv.reader(csvfile)
            for row in plots:
                tempArr.append(float(row[0]))
    print("Adding {} to array".format(tempArr))
    arrayMeanTimes.append(tempArr)        
    print(arrayMeanTimes)
    tempArr = []
    os.chdir(path)


## ======================= Best Times ==========================

#==================Plot Combined Tester Performance=============
legendArr = []
fig, ax = plt.subplots()
i = 0
for arr in arrayBestTimes:
    ax.plot(arr)
    i = i + 1
    legendArr.append(['Tester ' + str(i)])
ax.legend(legendArr)
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="Performance of each Human Tester")
ax.grid()

fig.savefig("ResultsHeuristic/resultsCombinedHumans-Best.png")
plt.show()


#===============================================================

#==============Plot them in a row==============================
Tot = directoryCounter
Cols = int(Tot**0.5)

Rows = Tot//Cols
Rows += Tot % Cols # add rows if needed
fig, axs = plt.subplots(Cols, Rows, constrained_layout=True)
i = 0
for ax in axs.flat:
    rgb = np.random.rand(3,)
    ax.plot(arrayBestTimes[i],c=rgb)
    i = i + 1
    ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',title="Tester " + str(i))
    ax.grid()
fig.savefig("ResultsHeuristic/resultsSideBySide-Best.png")
plt.show()

#==============================================================

#================Plot isolated=================================


i = 0
for arr in arrayBestTimes:
    fig, ax = plt.subplots(1)
    rgb = np.random.rand(3,)
    ax.plot(arr, c=rgb)
    ax.set(title="Tester " + str(i+1),xlabel='Episode', ylabel='Best Lap Time(s)')
    fig.savefig("ResultsHeuristic/Test" + str(i+1) + "-Best.png")
    i += 1
    plt.show()

#==============================================================



## ======================== END OF BEST TIMES ==================

## =========================MEAN TIMES =========================


#==================Plot Combined Tester Performance=============
legendArr = []
fig, ax = plt.subplots()
i = 0
for arr in arrayMeanTimes:
    ax.plot(arr)
    i = i + 1
    legendArr.append(['Tester ' + str(i)])
ax.legend(legendArr)
ax.set(xlabel='Episode', ylabel='Mean Lap Time(s)',
       title="Performance of each Human Tester")
ax.grid()

fig.savefig("ResultsHeuristic/resultsCombinedHumans-Mean.png")
plt.show()


#===============================================================

#==============Plot them in a row==============================
Tot = directoryCounter
Cols = int(Tot**0.5)

Rows = Tot//Cols
Rows += Tot % Cols # add rows if needed
fig, axs = plt.subplots(Cols, Rows, constrained_layout=True)
i = 0
for ax in axs.flat:
    rgb = np.random.rand(3,)
    ax.plot(arrayMeanTimes[i],c=rgb)
    i = i + 1
    ax.set(xlabel='Episode', ylabel='Mean Lap Time(s)',title="Tester " + str(i))
    ax.grid()
fig.savefig("ResultsHeuristic/resultsSideBySide-Mean.png")
plt.show()

#==============================================================

#================Plot isolated=================================


i = 0
for arr in arrayMeanTimes:
    fig, ax = plt.subplots(1)
    rgb = np.random.rand(3,)
    ax.plot(arr, c=rgb)
    ax.set(title="Tester " + str(i+1),xlabel='Episode', ylabel='Mean Lap Time(s)')
    fig.savefig("ResultsHeuristic/Test" + str(i+1) + "-Mean.png")
    i += 1
    plt.show()

#==============================================================





## ======================END OF MEAN TIMES======================