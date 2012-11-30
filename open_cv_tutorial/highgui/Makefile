#Compile and link flags
CXX = g++
CFLAGS = -Wall -g
#Compilation
CXXFLAGS += `pkg-config opencv --cflags`

#Linking
LDFLAGS += -lm `pkg-config opencv --libs`

#Name your target executables here
all = ClickPlane Boxes	

all: $(all)

# Program dependencies (.o files will be compiled by implicit rules)
ClickPlane: 
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ click_plane.cpp

Boxes: 
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ mouse_events.cpp

clean:
	rm -f $(all) *.o
