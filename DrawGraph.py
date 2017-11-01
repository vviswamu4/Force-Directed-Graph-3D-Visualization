import sys
#modify this to add networkx installation directory
#sys.path.append("C:\PYTHON\lib\site-packages")

import networkx as nx
import matplotlib.pyplot as plt
import csv

class DrawGraph(object):
    def __init__(self):
        self.G = nx.Graph()
        self.mapDict = {}
        self.edges = []

    def normalize_weights(self, filepath):
        weights = []
        with open(filepath, "r") as csvfile:
            csvreader = csv.reader(csvfile)
            next(csvreader,None)
            for lines in csvreader:
                weights.append(float(lines[2]))
        minWeight = min(weights)
        maxWeight = max(weights)

        for i in range(0, len(weights)):
            weights[i] = round((weights[i]-minWeight) / (maxWeight - minWeight),4)

        return weights                
        

    def getNameStream(self, filepath):
        nameStream = ''
        i=0
        with open(filepath,"r") as csvfile:
            csvreader = csv.reader(csvfile)
            next(csvreader,None)
            for lines in csvreader:
                nameStream += lines[0]+";"+lines[1]+";"
                self.mapDict[lines[0]] = i
                self.G.add_node(i)
                i+=1
        nameStream = nameStream[:-1]
        streamLength = str(len(nameStream))
        nameStream = "n" + streamLength.rjust(8,'0') + nameStream
        #print(nameStream)
        return nameStream

    def getLinkStream(self, filepath):
        linkStream = ''
        weights = self.normalize_weights(filepath)
        with open(filepath,"r") as csvfile:
            csvreader = csv.reader(csvfile)
            next(csvreader,None)
            i = 0
            for lines in csvreader:
                #temp = (self.mapDict[lines[0]], self.mapDict[lines[1]],lines[2])
                temp = (self.mapDict[lines[0]], self.mapDict[lines[1]],weights[i])
                self.edges.append(temp)
                #linkStream += str(self.mapDict[lines[0]]) + ";" + str(self.mapDict[lines[1]]) + ";" + lines[2] + ";"
                linkStream += str(self.mapDict[lines[0]]) + ";" + str(self.mapDict[lines[1]]) + ";" + str(weights[i]) + ";"
                i += 1
        linkStream = linkStream[:-1]
        streamLength = str(len(linkStream))
        linkStream = "l" + streamLength.rjust(8,'0') + linkStream
        #print(linkStream)
        return linkStream

    def getPositionStream(self):
        positionStream = ''
        self.G.add_weighted_edges_from(self.edges)
        pos = nx.spring_layout(self.G, dim=3, weight='weight', scale=3)

        #for debugging!
        with open("positions.csv", "w") as csvfile:
            csvwriter = csv.writer(csvfile)
            for elem in pos:
                csvwriter.writerow(pos[elem])
            
        for elem in pos:
            for val in pos[elem]:
                positionStream += str(val) + ";"

                
        positionStream = positionStream[:-1]
        streamLength = str(len(positionStream))
        positionStream = "p" + streamLength.rjust(8,'0') + positionStream
        #print(positionStream)
        return positionStream
        


if __name__ == "__main__":
    dg = DrawGraph.DrawGraph()
    print(dg.getNameStream("data/nodes.csv"))
    print(dg.getLinkStream("data/links.csv"))
    print(dg.getPositionStream())
    #print(G.edges())
    #w=nx.get_edge_attributes(G, 'weight')
    #print(w)
    #print(pos)
    #nx.draw(G,pos,node_size=10)
    #plt.show()

    
    

    
            


                
