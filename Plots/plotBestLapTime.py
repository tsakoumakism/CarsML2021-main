import matplotlib.pyplot as plt
import csv
import math
import numpy as np
import os, glob
from datetime import datetime

# THIS IS SAC AND PPO ONLY

bestLapTimePPO = []
meanLapTimePPO = []

bestLapTimeSAC = [] 
meanLapTimeSAC = []


path = os.getenv('LOCALAPPDATA')

path = path + "\..\LocalLow\DefaultCompany\CarsML2021-main"

try:
       os.mkdir(path + "\ResultsAgents")
except OSError as error:
              print(error)


print (path )
os.chdir(path)

csvCounter = len(glob.glob1(path,"*.csv")) #make sure we know how many csv files we have


#Create data for PPO BestLapTimes
for file in glob.glob("*PPO*-BestLapTimes.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            bestLapTimePPO.append(float(row[0]))

print(bestLapTimePPO)

#Create data for PPO meanLapTimes
for file in glob.glob("*PPO*-MeanLapTimes.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            meanLapTimePPO.append(float(row[0]))

print(meanLapTimePPO)


#Create data for SAC BestlapTimes
for file in glob.glob("*SAC*-BestLapTimes.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            bestLapTimeSAC.append(float(row[0]))

print(bestLapTimeSAC)

#Create data for SAC BestlapTimes
for file in glob.glob("*SAC*-MeanLapTimes.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            meanLapTimeSAC.append(float(row[0]))

print(meanLapTimeSAC)

            
            

# Plot each of them in different plots
# ==================PPO=====================
fig, ax = plt.subplots()
ax.plot(range(1,len(bestLapTimePPO) + 1), bestLapTimePPO)
#ax.legend(['Best Lap Times', 'Mean Lap Times'])
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO's Agent Best Lap Times for each Episode")
ax.grid()

fig.savefig("ResultsAgents/resultsPPO-Best.png")
plt.show()


fig, ax = plt.subplots()
ax.plot(range(1,len(meanLapTimePPO) + 1), meanLapTimePPO)
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO's Agent Mean Lap Times for each Episode")
ax.grid()
fig.savefig("ResultsAgents/resultsPPO-Mean.png")

plt.show()

#=================End of PPO================

# #===================SAC=====================
fig, ax = plt.subplots()
ax.plot(range(1,len(bestLapTimeSAC) + 1), bestLapTimeSAC)
#ax.legend(['Best Lap Times', 'Mean Lap Times'])
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC's Agent Best Lap Times for each Episode")
ax.grid()

fig.savefig("ResultsAgents/resultsSAC-Best.png")
plt.show()


fig, ax = plt.subplots()
ax.plot(range(1,len(meanLapTimeSAC) + 1), meanLapTimeSAC)
ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC's Agent Mean Lap Times for each Episode")

ax.grid()
fig.savefig("ResultsAgents/resultsSAC-Mean.png")
plt.show()
# #=================End of SAC================

# # Plot each of them in the same plot with shared Y axis, in a row
fig2, (ax,ax2) =  plt.subplots(ncols=2, sharex = True, sharey=True)


ax.plot(range(1,len(meanLapTimePPO) + 1), meanLapTimePPO)
ax2.plot(range(1,len(meanLapTimeSAC) + 1), meanLapTimeSAC)


ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO Agent Mean Lap Times")



ax2.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC Agent Mean Lap Times")


ax.grid()
ax2.grid()

fig2.savefig("ResultsAgents/sidebysideResults-Mean.png")

plt.show()
# #=================End of Share Y Axis================


# # Plot each of them in the same plot with shared Y axis, in a row
fig2, (ax,ax2) =  plt.subplots(ncols=2, sharex = True, sharey=True)

ax.plot(range(1,len(bestLapTimePPO) + 1), bestLapTimePPO)
ax2.plot(range(1,len(bestLapTimeSAC) + 1), bestLapTimeSAC)



ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO Agent Best Lap Times")



ax2.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC Agent Best Lap Times")


ax.grid()
ax2.grid()

fig2.savefig("ResultsAgents/sidebysideResults-Best.png")

plt.show()
# #=================End of Share Y Axis================

# #=================Plot all in one====================
fig, ax = plt.subplots()

ax.plot(range(1,len(bestLapTimePPO) + 1), bestLapTimePPO)
ax.plot(range(1,len(bestLapTimeSAC) + 1), bestLapTimeSAC)

ax.legend(['PPO Agent Best','SAC Agent Best'])

ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="Best Lap Times per Episode")

ax.grid()

fig.savefig("ResultsAgents/combinedResults-Best.png")
plt.show()
# #=============End of Plot all in one===============

# #=================Plot all in one====================
fig, ax = plt.subplots()


ax.plot(range(1,len(meanLapTimePPO) + 1), meanLapTimePPO)
ax.plot(range(1,len(meanLapTimeSAC) + 1), meanLapTimeSAC)

ax.legend(['PPO Agent Best','PPO Agent Mean'])



ax.set(xlabel='Episode', ylabel='Mean Lap Time(s)',
       title="Mean Lap Times per Episode")

ax.grid()

fig.savefig("ResultsAgents/combinedResults-Mean.png")
plt.show()
# #=============End of Plot all in one===============

# #=================Plot all in one====================
fig, ax = plt.subplots()

ax.plot(range(1,len(bestLapTimePPO) + 1), bestLapTimePPO)
ax.plot(range(1,len(meanLapTimePPO) + 1), meanLapTimePPO)

ax.plot(range(1,len(bestLapTimeSAC) + 1), bestLapTimeSAC)
ax.plot(range(1,len(meanLapTimeSAC) + 1), meanLapTimeSAC)

ax.legend(['PPO Agent Best','PPO Agent Mean','SAC Agent Best','SAC Agent Mean'])

#new_list = range(math.floor(min(indicesPPO)), math.ceil(max(indicesPPO))+1)

ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="Best Lap Times per Episode")

ax.grid()

fig.savefig("ResultsAgents/combinedResults-Mixed.png")
plt.show()
# #=============End of Plot all in one===============