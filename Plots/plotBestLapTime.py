import matplotlib.pyplot as plt
import csv
import math
import numpy as np
import os, glob


# TODO
# Make a script that finds all 'heuristic' directories and takes csv data with 'heuristic' in it

# Plot each of them in different plots
# Plot each of them in the same plot with shared Y axis, in a row
# Plot all of them in one plot with different colors and a legend
# Find the mean best lap time of each agent


#exw thn entypwsh oti mporei na einai la8os ta best lap times opws einai twra prepei na to checkarw


# THIS IS SAC AND PPO ONLY

arrayPPO = []
arraySAC =[] 
arrayHeuristic = []
indicesPPO = [] 
indicesSAC = [] 
indicesHeuristic = []

path = os.getenv('LOCALAPPDATA')

path = path + "\..\LocalLow\DefaultCompany\CarsML2021-main"
print (path )
os.chdir(path)

csvCounter = len(glob.glob1(path,"*.csv")) #make sure we know how many csv files we have


#Create data for PPO
for file in glob.glob("*PPO*.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            arrayPPO.append(float(row[0]))
            i=i + 1
            indicesPPO.append(i)
#Create data for PPO
for file in glob.glob("*SAC*.csv"):
    print(file)
    i = 0
    with open(file, 'r') as csvfile:
        plots = csv.reader(csvfile)

        for row in plots:
            arraySAC.append(float(row[0]))
            i=i + 1
            indicesSAC.append(i)
# #if csvCounter is 3 then it means we also have heuristic data
# if(csvCounter  == 3):
#     #Create data for Heuristic
#     for file in glob.glob("*Heuristic*.csv"):
#         print(file)
#         i = 0
#         with open(file, 'r') as csvfile:
#             plots = csv.reader(csvfile)

#             for row in plots:
#                 arrayHeuristic.append(float(row[0]))
#                 i=i + 1
#                 indicesHeuristic.append(i)
            
            

# Plot each of them in different plots
# ==================PPO=====================
fig, ax = plt.subplots()
ax.plot(indicesPPO,arrayPPO)

new_list = range(math.floor(min(indicesPPO)), math.ceil(max(indicesPPO))+1)
plt.xticks(new_list)

ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO's agent best of lap times each Episode")
ax.grid()

fig.savefig("resultsPPO.png")
plt.show()

#=================End of PPO================

#===================SAC=====================
fig, ax = plt.subplots()
ax.plot(indicesSAC,arraySAC)

new_list = range(math.floor(min(indicesSAC)), math.ceil(max(indicesSAC))+1)
plt.xticks(new_list)

ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC's agent best of lap times each Episode")
ax.grid()

fig.savefig("resultsSAC.png")
plt.show()

#=================End of SAC================

# Plot each of them in the same plot with shared Y axis, in a row
fig2, (ax,ax2) =  plt.subplots(ncols=2, sharex = True, sharey=True)

ax.plot(indicesPPO, arrayPPO)
ax2.plot(indicesSAC, arraySAC)

ax.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="PPO Agent")

ax2.set(xlabel='Episode', ylabel='Best Lap Time(s)',
       title="SAC Agent")



new_list = range(math.floor(min(indicesSAC)), math.ceil(max(indicesSAC))+1)
plt.xticks(new_list)

ax.grid()
ax2.grid()

fig2.savefig("sidebysideResults.png")

plt.show()
#=================End of Share Y Axis================

#=================Plot all in one====================
fig, ax = plt.subplots()
ax.plot(indicesPPO,arrayPPO)



ax.plot(indicesSAC,arraySAC)
ax.legend(['PPO Agent','SAC Agent'])

ax.grid()

maxIndexArray = indicesPPO if max(indicesPPO) > max(indicesSAC) else indicesSAC
new_list = range(math.floor(min(maxIndexArray)), math.ceil(max(maxIndexArray))+1)
plt.xticks(new_list)


fig.savefig("combinedResults.png")
plt.show()
#=============End of Plot all in one===============