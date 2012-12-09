#Compile and link flags
CXX = g++
CFLAGS = -Wall -g

#Compilation
CXXFLAGS += `pkg-config opencv --cflags` -D ENABLE_OPENGL=YES

#Linking
LDFLAGS += -lm `pkg-config opencv --libs` -lsfml-window -lsfml-system -lGL -lGLU -lGLEW

all = AugmentedReality
all: $(all)

AugmentedReality:
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ main.cpp

clean:
	rm -f $(all) *.o
