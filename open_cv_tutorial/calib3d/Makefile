#Compile and link flags
CXX = g++
CFLAGS = -Wall -g
#Compilation
CXXFLAGS += `pkg-config opencv --cflags`

#Linking
LDFLAGS += -lm `pkg-config opencv --libs`

#Name your target executables here
all = Chessboard Capture	

all: $(all)

# Program dependencies (.o files will be compiled by implicit rules)
Chessboard: 
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ chessboard.cpp

Capture: 
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ capture.cpp


clean:
	rm -f $(all) *.o
