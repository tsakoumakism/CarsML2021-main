import matplotlib.pyplot as plt
import csv
import math
from array import *
import numpy as np
import os, glob

path = os.getenv('LOCALAPPDATA')
path = path + "\..\LocalLow\DefaultCompany\CarsML2021-main"
print (path)
os.chdir(path)

directoryCounter = len(glob.glob1(path,"Heuristic-*")) #make sure we know how many csv files we have
print(directoryCounter)

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
                i=i + 1
                tempArr = np.append(tempArr,[i,(float(row[0]))])
    print("Adding {} to array".format(tempArr))
    array.append(tempArr)        
    print(array)
    tempArr = []
    os.chdir(path)



#==================Plot Combined Tester Performance=============
legendArr = []
fig, ax = plt.subplots()
i = 0
for arr in array:
    ax.plot(arr)
    i = i + 1
    legendArr.append(['Tester ' + str(i)])
ax.legend(legendArr)
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="Performance of each Human Tester")
ax.grid()

fig.savefig("resultsCombinedHumans.png")
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
    ax.plot(array[i],c=rgb)
    i = i + 1
    ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',title="Tester " + str(i+1))
    ax.grid()
plt.show()

#==============================================================

#================Plot isolated=================================


i = 0
for arr in array:
    fig, ax = plt.subplots(1)
    rgb = np.random.rand(3,)
    ax.plot(arr, c=rgb)
    ax.set(title="Tester " + str(i+1),xlabel='Episode', ylabel='Best Lap Time(s)')
    fig.savefig("Test" + str(i+1) + ".png")
    i += 1
    plt.show()

#==============================================================