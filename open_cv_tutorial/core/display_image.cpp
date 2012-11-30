#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int main( int argc, char** argv )
{
    if( argc != 2)
    {
     cout <<" Usage: display_image ImageToLoadAndDisplay" << endl;
     return -1;
    }

    Mat image;
	//CV_LOAD_IMAGE_UNCHANGED (<0)
	//CV_LOAD_IMAGE_GRAYSCALE (0)
	//CV_LOAD_IMAGE_COLOR (>0)
    image = imread(argv[1], -1);//CV_LOAD_IMAGE_COLOR);	// Read the file

    if(! image.data )                              // Check for invalid input
    {
        cout <<  "Could not open or find the image" << std::endl ;
        return -1;
    }

	Mat M(200, 200, CV_8UC3, Scalar(255,0,0));

    namedWindow( "Display window", CV_WINDOW_AUTOSIZE );// Create a window for display.
    imshow( "Display window", image );                   // Show our image inside it.
    //imshow( "Display window", M );                   // Show our image inside it.

    waitKey(0);											 // Wait for a keystroke in the window

    return 0;
}
